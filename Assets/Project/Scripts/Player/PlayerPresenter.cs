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

    // このPresenterが紐付いている PlayerView コンポーネントを取得するプロパティ。
    public PlayerView PlayerView => view;

    /// <summary>
    /// 現在の階層を取得
    /// </summary>
    public int CurrentFloor => model?.CurrentFloor ?? 0;

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

    // GunDataをキーにして、GunModelを保存する辞書
    private Dictionary<GunData, GunModel> gunStatus = new Dictionary<GunData, GunModel>();

    private int currentGunIndex = 0; // 今構えている銃のインデックス
    private Vector2 rawLookInput; // 視点移動の入力を保持
    private bool isFiring; // ボタンが押されているかどうかの状態
    private float lastFireTime; // 最後に撃った時刻を記録する変数
    private CancellationTokenSource fireCts;
    private CancellationTokenSource reloadCts; // リロード中断用

    private bool _isDead = false;
    private bool _isInputBlocked = false;
    public bool IsInvincible { get; set; } = false;
    private IInteractable currentInteractable;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // 60FPS固定
        Application.targetFrameRate = 60;

        // 初期データのセットアップ
        InitPlayerState();
        
        // Viewからの入力イベントなどの紐付け（購読）
        SetupMovementInput();
        SetupCombat();
        SetupDamageHandling();
        SetupInteraction();
    }

    private void OnEnable()
    {
        UIEvents.OnCutsceneStateChanged += SetInputBlocked;
    }
    private void OnDestroy()
    {
        UIEvents.OnCutsceneStateChanged -= SetInputBlocked;
    }

    private void Update()
    {
        if (_isInputBlocked)
        {
            view.UpdateBodyRotation(model.CurrentPan);
            view.SetUpperBodyPitch(model.CurrentPitch);
            return;
        }

        Vector3 movement = model.CalcMove(Time.deltaTime);
        view.Move(movement);

        if (rawLookInput.sqrMagnitude > 0.001f)
        {
            float sensitivity = playerData.rotationSensitivity * 100f * Time.deltaTime;

            model.CurrentPan += rawLookInput.x * sensitivity;
            model.CurrentPitch -= rawLookInput.y * sensitivity;
            model.CurrentPitch = Mathf.Clamp(model.CurrentPitch, playerData.minPitch, playerData.maxPitch);
        }

        view.UpdateBodyRotation(model.CurrentPan);
        view.SetUpperBodyPitch(model.CurrentPitch);
        view.SetDashAnimation(model.IsDashing && model.MoveInput.sqrMagnitude > 0.001f);
    }

    /// <summary>
    /// 外部（ボスなど）から数値でダメージを受け取る
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (_isDead || IsInvincible) return;

        model.TakeDamage(damage);
        hpView?.UpdateHpDisplay(model.CurrentHP);

        Debug.Log($"[Player] Damaged: {damage}, Current HP: {model.CurrentHP}");
    }

    /// <summary>
    /// モデルや武器などの初期化
    /// </summary>
    private void InitPlayerState()
    {
        if (inventoryGuns.Length > 0)
        {
            SetupWeapon(inventoryGuns[currentGunIndex]);
        }
        // Model に ScriptableObject を渡して初期化
        model = new PlayerModel(playerData);
        hpView?.UpdateHpDisplay(model.CurrentHP);
    }

    /// <summary>
    /// 移動・視点移動の入力設定
    /// </summary>
    private void SetupMovementInput()
    {
        view.OnMoveInputReceived += (input) => 
        {
            if (_isInputBlocked) return;
            model.MoveInput = input;
        };
        view.OnDashInputReceived += (isDash) => 
        {
            if (_isInputBlocked) return;
            model.IsDashing = isDash;
        };
        view.OnLookInputReceived += (look) => 
        {
            if (_isInputBlocked) return;
            rawLookInput = look;
        };
    }

    /// <summary>
    /// 射撃・武器切り替えの入力設定
    /// </summary>
    private void SetupCombat()
    {
        view.OnWeaponDirectSelect += (index) =>
        {
            if (_isInputBlocked) return;
            Debug.Log($"[WeaponSelect] Index: {index} が押されました");
            SwapWeapon(index);
        };
        view.OnWeaponSwitchInputRecieved += (direction) => 
        {
            if (_isInputBlocked) return;
            RotateWeapon(direction);
        };
        view.OnAimInputReceived += (isAiming) =>
        {
            if (_isInputBlocked) return;
            model.IsAiming = isAiming;
            view.SetAiming(isAiming);
        };
        view.OnFireInputReceived += (pressed) =>
        {
            if (_isInputBlocked) return;
            if (isFiring == pressed) return;
            isFiring = pressed; // 押しっぱなしの状態を記録
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
        view.OnFireEffectTiming += ExecuteRaycastHit;
        view.OnReloadInputReceived += () =>
        {
            if (_isInputBlocked) return;
            
            // 既にリロード中なら何もしない
            if (gunModel.CurrentAmmo == gunData.maxAmmo || gunModel.ReserveAmmo <= 0 || gunModel.IsReloading) return;
            
            reloadCts?.Cancel();
            reloadCts?.Dispose();
            reloadCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            ReloadAsync(reloadCts.Token).Forget();
        };
    }

    /// <summary>
    /// 着弾判定（Raycast）の実行
    /// </summary>
    private void ExecuteRaycastHit()
    {
        Debug.Log("--- Raycast Triggered ---");
        Transform camTransform = Camera.main.transform;
        Vector3 rayOrigin = camTransform.position;
        Vector3 rayDirection = camTransform.forward;
        int layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        float maxDistance = 100;
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxDistance, layerMask))
        {
            gunView.ProcessHit(hit, gunData.damage);

            Vector3 targetPoint = hit.point;
            Vector3 fireDirection = (targetPoint - gunView.muzzlePoint.position).normalized;
            gunView.LaunchBullet(fireDirection, gunData.speed);
        }
        else
        {
            Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.green, 1.0f);
            Vector3 targetPoint = rayOrigin + (rayDirection * maxDistance);
            Vector3 fireDirection = (targetPoint - gunView.muzzlePoint.position).normalized;
            gunView.LaunchBullet(fireDirection, gunData.speed);
        }
    }

    /// <summary>
    /// 死亡時やタイムアップ時に呼び出され、進行中のアクションを強制キャンセルしてプレイヤーの操作を完全に無効化する。
    /// </summary>
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

    /// <summary>
    /// 外部（階段のトリガーなど）からプレイヤーの階層を更新する
    /// </summary>
    public void SetFloor(int floor)
    {
        model?.SetFloor(floor);
    }

    /// <summary>
    /// 被ダメージ・HPの監視設定
    /// </summary>
    private void SetupDamageHandling()
    {
        view.OnHitByEnemy += (enemyData) =>
        {
            if (enemyData != null)
            {
                TakeDamage(enemyData.enemyAttackPower);
            }
        };
        model.OnHpChanged += (currentHp) =>
        {
            hpView?.UpdateHpDisplay(currentHp);
            if (currentHp <= 0)
            {
                DisableInput(false);
                view.PlayDieAnim();
                GamePresenter.Instance.TriggerGameOver();
            }
        };
    }

    /// <summary>
    /// インタラクト（目標地点、NPC会話など）の設定
    /// </summary>
    private void SetupInteraction()
    {
        view.OnTriggerEnterEvent += (other) =>
        {
            var interactable = other.GetComponentInParent<IInteractable>();
            Debug.Log($"[TriggerEnter] 接触: {other.name}, IInteractableあり: {interactable != null}");
            if (interactable != null)
            {
                currentInteractable = interactable;
                string promptText = currentInteractable.GetInteractPrompt();

                UIEvents.OnShowInteractPrompt?.Invoke(promptText);
            }
        };

        view.OnTriggerExitEvent += (other) =>
        {
            var interactible = other.GetComponentInParent<IInteractable>();
            if (interactible != null && currentInteractable == interactible)
            {
                Debug.Log($"[TriggerExit] {other.name} から離れました");
                currentInteractable = null;

                UIEvents.OnHideInteractPrompt?.Invoke();
            }
        };

        view.OnInteractInputReceived += () =>
        {
            if (_isInputBlocked) return;
            if (currentInteractable != null)
            {
                currentInteractable.Interact(this.gameObject);
                UIEvents.OnHideInteractPrompt?.Invoke();
            }
        };
    }

    /// <summary>
    /// NPCの救出イベント中など、一時的にプレイヤーの操作をブロック（または解除）する。
    /// </summary>
    public void SetInputBlocked(bool isBlocked)
    {
        _isInputBlocked = isBlocked;
        if (isBlocked)
        {
            model.MoveInput = Vector2.zero;
            view.Move(Vector3.zero);
            rawLookInput = Vector2.zero;
            model.IsAiming = false;
            view.SetAiming(false);
            isFiring = false;

            if (fireCts != null)
            {
                fireCts.Cancel();
                fireCts.Dispose();
                fireCts = null;
            }

            if (reloadCts != null)
            {
                reloadCts.Cancel();
                reloadCts.Dispose();
                reloadCts = null;
            }
        }
    }

    /// <summary>
    /// プレイヤーの向き（水平回転・垂直回転）をモデル状態を含めて強制的に設定します。
    /// </summary>
    /// <param name="rotation">設定したい回転（クォータニオン）</param>
    public void SetRotation(Quaternion rotation)
    {
        if (model == null) return;

        // 水平回転（Pan）を更新
        model.CurrentPan = rotation.eulerAngles.y;

        // 垂直ピッチ（Pitch）を更新し、許容範囲内に収める
        float xRot = rotation.eulerAngles.x;
        if (xRot > 180f) xRot -= 360f;
        model.CurrentPitch = Mathf.Clamp(xRot, playerData.minPitch, playerData.maxPitch);

        // 即座にViewの回転に反映する
        if (view != null)
        {
            view.UpdateBodyRotation(model.CurrentPan);
            view.SetUpperBodyPitch(model.CurrentPitch);
        }
    }

    /// <summary>
    /// 現在の武器インデックスから指定された方向へインデックスをずらし、次の武器への持ち替えを要求する。
    /// </summary>
    private void RotateWeapon(int direction)
    {
        int nextIndex = currentGunIndex + direction;

        if (nextIndex < 0) nextIndex = inventoryGuns.Length - 1;
        if (nextIndex >= inventoryGuns.Length) nextIndex = 0;

        SwapWeapon(nextIndex);
    }

    /// <summary>
    /// 指定されたインデックスの武器が存在し、現在と違う武器であれば実際に持ち替え処理を実行する。
    /// </summary>
    private void SwapWeapon(int index)
    {
        if (index < 0 || index >= inventoryGuns.Length || index == currentGunIndex) return;

        // リロード中は切り替え不可
        if (gunModel != null && gunModel.IsReloading) return;

        currentGunIndex = index;
        SetupWeapon(inventoryGuns[currentGunIndex]);
    }

    /// <summary>
    /// 指定された銃データ（GunData）を基に新しい武器の3Dモデルを生成し、状態やHUDの表示を初期化する。
    /// </summary>
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

        weaponHUD?.UpdateWeaponUI(data);

        ammoView?.UpdateAmmoDisplay(gunModel.CurrentAmmo, gunModel.ReserveAmmo);
    }

    /// <summary>
    /// フルオート武器の射撃時に呼び出され、入力がキャンセルされるまで設定された連射間隔で発砲を繰り返す非同期処理。
    /// </summary>
    private async UniTaskVoid FireLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (model.IsAiming && gunModel.CanShoot())
            {
                TryFire();
            }

            // 次の発射まで待機する
            await UniTask.Delay((int)(gunData.fireRate * 1000), cancellationToken: token);
        }
    }


    /// <summary>
    /// 現在の状態（操作ブロック、弾薬の有無、連射間隔、エイム状態など）を確認し、すべての条件を満たしていれば発砲処理を呼び出す。
    /// </summary>
    private void TryFire()
    {
        if (_isDead) return;
        if (!isFiring) return;
        if (!gunModel.CanShoot()) return;
        if (Time.time < lastFireTime + gunData.fireRate) return;
        if (!model.IsAiming) return;

        if (gunModel.CurrentAmmo <= 0 && !gunModel.IsReloading)
        {
            gunView.PlayShotSound(gunData.emptySound);
            return;
        }

        ExecuteFire();
    }

    /// <summary>
    /// 実際の弾薬消費、発砲アニメーションの再生、および発砲音の再生を実行する。
    /// </summary>
    private void ExecuteFire()
    {
        lastFireTime = Time.time;

        gunModel.ConsumeAmmo();
        ammoView?.UpdateAmmoDisplay(gunModel.CurrentAmmo, gunModel.ReserveAmmo);

        view.OnShoot();
        view.PlayFireAnim();

        soundView.SoundSource(transform.position);
        gunView.PlayShotSound(gunData.fireSound);

        Debug.Log($"[Fire] Damage: {gunModel.Damage}, Remaining Ammo: {gunModel.CurrentAmmo}");
    }

    /// <summary>
    /// 武器の規定リロード時間だけ待機し、UIのプログレスバーを更新した後に弾薬を最大まで補充する非同期処理。
    /// </summary>
    private async UniTaskVoid ReloadAsync(CancellationToken token)
    {
        // 弾薬が最大、予備弾数が0、または既にリロード中の場合は処理を行わない
        if (gunModel.CurrentAmmo == gunData.maxAmmo || gunModel.ReserveAmmo <= 0 || gunModel.IsReloading) return;

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
                ammoView?.SetReloadProgress(progress);

                await UniTask.Yield(token);
            }

            ammoView?.SetReloadProgress(0f); // バーを隠す
            gunView.StopSound();
            gunModel.Reload();

            if (ammoView != null)
            {
                ammoView?.UpdateAmmoDisplay(gunModel.CurrentAmmo, gunModel.ReserveAmmo);
            }
            Debug.Log($"Reload complete! Ammo: {gunModel.CurrentAmmo}");
        }
        catch (System.OperationCanceledException)
        {
            ammoView?.SetReloadProgress(0f);
            gunView.StopSound();
        }
        finally
        {
            gunModel.IsReloading = false;
        }
    }
}
