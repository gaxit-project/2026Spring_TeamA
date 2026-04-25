using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerPresenter : MonoBehaviour
{
    public static PlayerPresenter Instance { get; private set; }

    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerView view;

    [SerializeField] private GunData[] inventoryGuns;
    [SerializeField] private GunView gunView;

    [SerializeField] private AmmoView ammoView;
    [SerializeField] private WeaponHUD weaponHUD;
    [SerializeField] private HPView hpView;
    [SerializeField] private SoundDetectionView soundView;


    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject defaultGunPrefab;

    private bool _isInputBlocked = false;
    
    private PlayerModel model;
    
    private GunData gunData;
    private GunModel gunModel;

    // GunDataをキーにして、GunModelを保存する辞書
    private Dictionary<GunData, GunModel> gunStatus = new Dictionary<GunData, GunModel>();

    private Vector2 rawLookInput; // 視点移動の入力を保持
    
    private int currentGunIndex = 0; // 今構えている銃のインデックス
    private bool isFiring; // ボタンが押されているかどうかの状態
    private float lastFireTime; // 最後に撃った時刻を記録する変数

    private CancellationTokenSource fireCts;
    private CancellationTokenSource reloadCts; // リロード中断用

    private IInteractable currentInteractable;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // 60FPS固定
        Application.targetFrameRate = 60;

        if (inventoryGuns.Length > 0)
        {
            SetupWeapon(inventoryGuns[currentGunIndex]);
        }

        // Model に ScriptableObject を渡して初期化
        model = new PlayerModel(playerData);
        hpView.UpdateHpDiaplay(model.CurrentHP);

        // Viewの入力イベントを購読し、Modelのデータへ反映させる
        // 移動・視点入力
        view.OnMoveInputReceived += (input) => model.MoveInput = input;
        view.OnDashInputReceived += (isDash) =>model.IsDashing = isDash;

        view.OnLookInputReceived += (look) =>
        {
            rawLookInput = look;
        };

        view.OnWeaponDirectSelect += (index) =>
        {
            Debug.Log($"[WeaponSelect] Index: {index} が押されました");
            SwapWeapon(index);
        };


        // 攻撃系
        view.OnAimInputReceived += (IsAiming) =>
        {
            view.SetAiming(IsAiming);
        };

        view.OnFireInputReceived += (pressed) =>
        {
            if (isFiring == pressed) return;
            // 押しっぱなしの状態を記録
            isFiring = pressed;

            if (pressed)
            {
                if (gunData.isFullAuto)
                {
                    if (fireCts == null || fireCts.IsCancellationRequested)
                    {
                        fireCts?.Cancel();
                        fireCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
                        FireLoopAsync(fireCts.Token).Forget();
                    }
                }
                else
                {
                    TryFire();
                }
            }
            else
            {
                fireCts?.Cancel();
                fireCts?.Dispose();
                fireCts = null;
            }
        };

        view.OnFireEffectTiming += () =>
        {
            Debug.Log("--- Raycast Triggered ---"); // これが1回の射撃で何回出るか確認

            // カメラの中心からレイを飛ばす準備
            Transform camTransform = Camera.main.transform;
            Vector3 rayOrigin = camTransform.position;
            Vector3 rayDirection = camTransform.forward;

            // Playerレイヤー(Layer 6)以外に当たるようにマスクを作成
            int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

            float maxDistance = 100; // 射程距離

            // レイを飛ばして当たった場所を特定
            // 第1引数:起点, 第2:方向, 第3:当たった情報, 第4:最大距離
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxDistance, layerMask))
            {
                // 当たった対象の名前をログに出す
                Debug.Log($"Hit: {hit.collider.name} / Layer: {hit.collider.gameObject.layer}");
                // 当たった場所まで赤い線を引く（Sceneビューで確認可能）
                Debug.DrawLine(rayOrigin, hit.point, Color.red, 1.0f);

                // EnemyBodyPartがあるか確認
                var bodyPart = hit.collider.GetComponent<EnemyBodyPart>();

                if (bodyPart != null)
                {
                    // 部位がある場合は、部位に通知を送る
                    bodyPart.NotifyHit(gunData.damage);
                }
                else
                {
                    // 部位がない場合、オブジェクトそのものがIDamageableか確認
                    var damageable = hit.collider.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(gunData.damage);
                    }

                    var glass = hit.collider.GetComponent<BulletproofGlass>();
                    if (glass != null)
                    {
                        // ガラスに座標と向きのデータを渡す
                        glass.AddCrack(hit.point, hit.normal);
                    }
                }

                // 3. 着弾地点を弾の目標にする
                Vector3 targetPoint = hit.point;
                Vector3 fireDirection = (targetPoint - gunView.muzzlePoint.position).normalized;
                gunView.LaunchBullet(fireDirection, gunData.speed);
            }
            else
            {
                // 何にも当たっていない場合、射程限界まで緑の線を引く
                Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.green, 1.0f);

                // 何も当たらなかった場合
                Vector3 targetPoint = rayOrigin + (rayDirection * maxDistance);
                Vector3 fireDirection = (targetPoint - gunView.muzzlePoint.position).normalized;
                gunView.LaunchBullet(fireDirection, gunData.speed);
            }
        };

        view.OnReloadInputReceived += () =>
        {
            ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();
        };

        view.OnWeaponSwitchInputRecieved += (direction) => RotateWeapon(direction);

        // Viewにゾンビのタグを教える
        view.SetTargetTag(playerData.zombieTag);
        // ダメージ処理
        view.OnHitByEnemy += (enemyData) =>
        {
            if (enemyData != null)
            {
                model.TakeDamage(enemyData.enemyAttackPower);
                hpView.UpdateHpDiaplay(model.CurrentHP);
            }
        };

        model.OnHpChanged += (currentHp) =>
        {
            // HP表示を更新する
            hpView.UpdateHpDiaplay(currentHp);

            // 0以下ならゲームオーバー処理を呼ぶ
            if (currentHp <= 0)
            {
                DisableInput(false);
                view.PlayDieAnim();

                GamePresenter.Instance.TriggerGameOver();
            }
        };

        view.OnTriggerEnterEvent += (other) =>
        {
            if (other.CompareTag("Goal"))
            {
                GamePresenter.Instance.TriggerGameClear();
            }
            else if (other.CompareTag("NextLevel"))
            {
                GamePresenter.Instance.TriggerNextLevel();
            }

            var interactable = other.GetComponentInParent<IInteractable>();

            Debug.Log($"[TriggerEnter] 接触: {other.name}, IInteractableあり: {interactable != null}");

            if (interactable != null)
            {
                currentInteractable = interactable;

                // 文字をもらって表示する
                string promptText = currentInteractable.GetInteractPrompt();
                UIManager.Instance.ShowInteractPrompt(promptText);
            }
        };

        view.OnTriggerExitEvent += (other) =>
        {
            var interactible = other.GetComponentInParent<IInteractable>();
            if (interactible != null && currentInteractable == interactible)
            {
                Debug.Log($"[TriggerExit] {other.name} から離れました");

                currentInteractable = null;
                UIManager.Instance.HideInteractPrompt();
            }
        };

        view.OnInteractInputReceived += () =>
        {
            if (_isInputBlocked) return;

            if (currentInteractable != null)
            {
                currentInteractable.Interact(this.gameObject);
                UIManager.Instance.HideInteractPrompt();
            }
        };
    }

    public void DisableInput(bool isTimeOut)
    {
        if (_isInputBlocked) return;
        _isInputBlocked = true;
        // 撃っている最中なら射撃を強制キャンセル
        if (fireCts != null)
        {
            fireCts.Cancel();
            fireCts.Dispose();
            fireCts = null;
        }
        // 足を強制的に止める
        model.MoveInput = Vector2.zero;
        view.Move(Vector3.zero);
        if (isTimeOut)
        {
            view.PlayTimeOutAnim(); // 時間切れ
        }
        else
        {
            view.PlayDieAnim(); // 死亡
        }
    }

    public void SetInputBlocked(bool isBlocked)
    {
        _isInputBlocked = isBlocked;
        if (isBlocked)
        {
            model.MoveInput = Vector2.zero;
            view.Move(Vector3.zero);

            if (fireCts != null)
            {
                fireCts.Cancel();
                fireCts.Dispose();
                fireCts = null;
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Title");
        }

        if (_isInputBlocked)
        {
            view.UpdateBodyRotation(model.CurrentPan);
            view.SetUpperBodyPitch(model.currentPitch);
            return;
        }

        // Modelに移動量を計算させる
        Vector3 velocity = model.CalcVelocity();
        // 計算結果をViewに渡して移動を実行させる
        view.Move(velocity);

        if (rawLookInput.sqrMagnitude > 0.001f)
        {
            float sensitivity = playerData.rotationSensitivity * 100f * Time.deltaTime;

            model.CurrentPan += rawLookInput.x * sensitivity;
            model.currentPitch -= rawLookInput.y * sensitivity;
            model.currentPitch = Mathf.Clamp(model.currentPitch, playerData.minPitch, playerData.maxPitch);
        }

        view.UpdateBodyRotation(model.CurrentPan);
        view.SetUpperBodyPitch(model.currentPitch);
        view.SetDashAnimation(model.IsDashing && model.MoveInput.sqrMagnitude > 0.001f);
    }

    private void RotateWeapon(int direction)
    {
        int nextIndex = currentGunIndex + direction;

        if (nextIndex < 0) nextIndex = inventoryGuns.Length - 1;
        if (nextIndex >= inventoryGuns.Length) nextIndex = 0;

        SwapWeapon(nextIndex);
    }

    private void SwapWeapon(int index)
    {
        if (index < 0 || index >= inventoryGuns.Length || index == currentGunIndex) return;

        currentGunIndex = index;
        SetupWeapon(inventoryGuns[currentGunIndex]);
    }

    private void SetupWeapon(GunData data)
    {
        isFiring = false;

        if (reloadCts != null)
        {
            reloadCts.Cancel();
            reloadCts.Dispose();
            reloadCts = null;
        }

        if (fireCts != null)
        {
            fireCts?.Cancel();
            fireCts?.Dispose();
            fireCts = null;
        }

        foreach (Transform child in weaponHolder) Destroy(child.gameObject);

        GameObject gunObj = Instantiate(data.gunPrefab, weaponHolder);

        if (data.animatorOverride != null)
        {
            view.SetAnimatorController(data.animatorOverride);
        }

        gunObj.transform.localPosition = Vector3.zero;
        gunObj.transform.localRotation = Quaternion.identity;

        // この武器が初めて使うものなら、新しくModelを作って辞書に登録する
        if (!gunStatus.ContainsKey(data)) gunStatus.Add(data, new GunModel(data));

        // 辞書から現在の武器の状態を取得する
        gunModel = gunStatus[data];

        gunView = gunObj.GetComponent<GunView>();
        this.gunData = data;

        if (data.drawSound != null)
        {
            gunView.PlayShotSound(data.drawSound);
        }

        weaponHUD.UpdateWeaponUI(data);

        ammoView.UpdateAmmoDisplay(gunModel.CurrentAmmo, gunModel.ReserveAmmo);
    }

    private async UniTaskVoid FireLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (view.GetComponent<Animator>().GetBool("IsAiming") && gunModel.CanShoot())
            {
                TryFire();
            }

            // 次の発射まで待機する
            await UniTask.Delay((int)(gunData.fireRate * 1000), cancellationToken: token);
        }
    }

    private void TryFire()
    {
        if (_isInputBlocked) return;
        if (!isFiring) return;
        if (!gunModel.CanShoot()) return;
        if (Time.time < lastFireTime + gunData.fireRate) return;
        if (!view.GetComponent<Animator>().GetBool("IsAiming")) return;

        if (gunModel.CurrentAmmo <= 0 && !gunModel.IsReloading)
        {
            gunView.PlayShotSound(gunData.emptySound);
            return;
        }

        ExecuteFire();
    }

    private void ExecuteFire()
    {
        lastFireTime = Time.time;

        gunModel.ConsumeAmmo();
        ammoView.UpdateAmmoDisplay(gunModel.CurrentAmmo, gunModel.ReserveAmmo);

        view.OnShoot();
        view.PlayFireAnim();

        soundView.SoundSource(transform.position);
        gunView.PlayShotSound(gunData.fireSound);

        Debug.Log($"[Fire] Damage: {gunModel.Damage}, Remaining Ammo: {gunModel.CurrentAmmo}");
    }

    private async UniTaskVoid ReloadAsync(CancellationToken token)
    {
        if (gunModel.CurrentAmmo == gunData.maxAmmo || gunModel.IsReloading) return;

        Debug.Log("Reloading started...");
        gunModel.IsReloading = true;
        view.PlayReloadAnim();
        gunView.PlaySimpleSound(gunData.reloadSound);

        float startTime = Time.time;
        float duration = gunData.reloadTime;

        try
        {
            while (Time.time - startTime < duration)
            {
                float progress = (Time.time - startTime) / duration;
                ammoView.SetReloadProgress(progress);

                await UniTask.Yield(token);
            }

            ammoView.SetReloadProgress(0f); // バーを隠す
            gunView.StopSound();
            gunModel.Reload();

            if (ammoView != null)
            {
                ammoView.UpdateAmmoDisplay(gunModel.CurrentAmmo, gunModel.ReserveAmmo);
            }
            Debug.Log($"Reload complete! Ammo: {gunModel.CurrentAmmo}");
        }
        catch (System.OperationCanceledException)
        {
            ammoView.SetReloadProgress(0f);
            gunView.StopSound();
        }
        finally
        {
            gunModel.IsReloading = false;
        }
    }

    public PlayerView PlayerView => view;
}
