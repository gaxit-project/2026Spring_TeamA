using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerView view;

    [SerializeField] private GunData gunData;
    [SerializeField] private GunData[] inventoryGuns;
    [SerializeField] private GunView gunView;

    private PlayerModel model;
    private GunModel gunModel;

    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject defaultGunPrefab;

    private int currentGunIndex = 0; // 今構えている銃のインデックス
    private Vector2 rawLookInput; // 視点移動の入力を保持
    private bool isFiring; // ボタンが押されているかどうかの状態
    private float lastFireTime; // 最後に撃った時刻を記録する変数
    private CancellationTokenSource reloadCts; // リロード中断用

    private void Awake()
    {
        // 60FPS固定
        Application.targetFrameRate = 60;

        if (inventoryGuns.Length > 0)
        {
            SetupWeapon(inventoryGuns[currentGunIndex]);
        }

        // Model に ScriptableObject を渡して初期化
        model = new PlayerModel(playerData);

        // Viewの入力イベントを購読し、Modelのデータへ反映させる
        // 移動・視点入力
        view.OnMoveInputReceived += (input) => model.MoveInput = input;
        /*view.OnLookInputReceived += (look) =>
        {
            float stickSmoothing = Time.deltaTime * 100f; // 100は調整用の倍率
            model.CurrentPan += look.x * playerData.rotationSensitivity * stickSmoothing;

            model.currentPitch -= look.y * playerData.rotationSensitivity;
            model.currentPitch = Mathf.Clamp(model.currentPitch, playerData.minPitch, playerData.maxPitch);
        };*/

        view.OnLookInputReceived += (look) =>
        {
            rawLookInput = look;
        };


        // 攻撃
        view.OnAimInputReceived += (IsAiming) =>
        {
            view.SetAiming(IsAiming);
        };

        view.OnFireInputReceived += (pressed) =>
        {
            // 押しっぱなしの状態を記録
            isFiring = pressed;

            // セミオートは押された瞬間だけ発射する
            if (pressed && !gunData.isFullAuto) TryFire();

            //if (!gunModel.CanShoot()) return;

            //if (Time.time >= lastFireTime + gunData.fireRate) ExecuteFire();
        };

        view.OnFireEffectTiming += () =>
        {
            // カメラの中心からレイを飛ばす準備
            Transform camTransform = Camera.main.transform;
            Vector3 rayOrigin = camTransform.position;
            Vector3 rayDirection = camTransform.forward;

            // Playerレイヤー(Layer 6)以外に当たるようにマスクを作成
            int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

            Vector3 targetPoint;
            float maxDistance = 100; // 射程距離

            // レイを飛ばして当たった場所を特定
            // 第1引数:起点, 第2:方向, 第3:当たった情報, 第4:最大距離
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxDistance, layerMask))
            {
                targetPoint = hit.point; // 何かに当たった場所
            }
            else
            {
                targetPoint = rayOrigin + (rayDirection * maxDistance); // 何もなければ遠くの空中
            }

            // 銃口からターゲット地点への方向を計算
            Vector3 fireDirection = (targetPoint - gunView.muzzlePoint.position).normalized;
            // その方向へ弾を発射
            gunView.LaunchBullet(fireDirection, gunData.speed);
        };

        view.OnReloadInputReceived += () =>
        {
            ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();
        };

        view.OnWeaponSwitchInputRecieved += (direction) => RotateWeapon(direction);
    }

    private void Update()
    {
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

        if (isFiring && gunData != null && gunData.isFullAuto)
        {
            if (view.GetComponent<Animator>().GetBool("IsAiming"))
            {
                TryFire();
            }
            else
            {
                isFiring = false;
            }
        }
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

        foreach (Transform child in weaponHolder) Destroy(child.gameObject);

        GameObject gunObj = Instantiate(data.gunPrefab, weaponHolder);

        if (data.animatorOverride != null)
        {
            view.SetAnimatorController(data.animatorOverride);
        }

        gunObj.transform.localPosition = Vector3.zero;
        gunObj.transform.localRotation = Quaternion.identity;

        gunView = gunObj.GetComponent<GunView>();

        this.gunData = data;
        gunModel = new GunModel(data);
    }

    private void TryFire()
    {
        if (!gunModel.CanShoot()) return;
        if (Time.time < lastFireTime + gunData.fireRate) return;
        if (!view.GetComponent<Animator>().GetBool("IsAiming")) return;

        ExecuteFire();
    }

    private void ExecuteFire()
    {
        lastFireTime = Time.time;

        gunModel.ConsumeAmmo();

        view.PlayFireAnim();

        if (gunData.isFullAuto) view.OnShoot();

        Debug.Log($"[Fire] Damage: {gunModel.Damage}, Remaining Ammo: {gunModel.CurrentAmmo}");
    }

    private async UniTaskVoid ReloadAsync(CancellationToken token)
    {
        if (gunModel.CurrentAmmo == gunData.maxAmmo || gunModel.IsReloading) return;

        Debug.Log("Reloading started...");
        gunModel.IsReloading = true;

        view.PlayReloadAnim();

        try
        {
            await UniTask.Delay((int)(gunData.reloadTime * 1000), cancellationToken: token);
            gunModel.Reload();
            Debug.Log($"Reload complete! Ammo: {gunModel.CurrentAmmo}");
        }
        finally
        {
            gunModel.IsReloading = false;
        }
    }
}
