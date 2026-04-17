using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
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

    private PlayerModel model;
    
    private GunData gunData;
    private GunModel gunModel;

    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject defaultGunPrefab;

    [SerializeField] private GameOverView gameOverView;
    [SerializeField] private NextLevelView nextLevelView;

    // GunDataをキーにして、GunModelを保存する辞書
    private Dictionary<GunData, GunModel> gunStatus = new Dictionary<GunData, GunModel>();

    private int currentGunIndex = 0; // 今構えている銃のインデックス
    private Vector2 rawLookInput; // 視点移動の入力を保持
    private bool isFiring; // ボタンが押されているかどうかの状態
    private float lastFireTime; // 最後に撃った時刻を記録する変数
    private CancellationTokenSource fireCts;
    private CancellationTokenSource reloadCts; // リロード中断用

    private bool _isGameOver = false;
    private bool _isLevelCleared = false;

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
            if (currentHp <= 0 && !_isGameOver)
            {
                _isGameOver = true;
                TriggerGameOver();
            }
        };

        view.OnTriggerEnterEvent += (other) =>
        {
            if (other.CompareTag("Goal") && !_isLevelCleared && !_isGameOver)
            {
                _isLevelCleared = true;
                TriggerLevelClear();
            }
        };
    }

    private void Start()
    {
        model.OnHpChanged += CheckDeath;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Title");
        }

        // Modelに移動量を計算させる
        Vector3 movement = model.CalcMove(Time.deltaTime);
        // 計算結果をViewに渡して移動を実行させる
        view.Move(movement);

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

    private void CheckDeath(int currentHp)
    {
        if (currentHp <= 0 && !_isGameOver)
        {
            _isGameOver = true;
            TriggerGameOver();
        }
    }

    private void TriggerLevelClear()
    {
        _isGameOver = true;

        // 準備中フェード開始
        nextLevelView.PlayComingSoonSequence().Forget();
    }

    private void TriggerGameOver()
    {
        // マウスカーソルを表示する（ボタンを押せるように）
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // フェード演出開始
        gameOverView.PlayGameOverSequence().Forget();
    }

    public PlayerView PlayerView => view;
}
