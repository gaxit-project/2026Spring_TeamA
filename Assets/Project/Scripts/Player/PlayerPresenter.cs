using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerView view;

    [SerializeField] private GunData gunData;
    [SerializeField] private GunView gunView;

    private PlayerModel model;
    private GunModel gunModel;

    private float lastFireTime; // 最後に撃った時刻を記録する変数
    private CancellationTokenSource reloadCts; // リロード中断用

    private void Awake()
    {
        // Model に ScriptableObject を渡して初期化
        model = new PlayerModel(playerData);
        gunModel = new GunModel(gunData);

        // Viewの入力イベントを購読し、Modelのデータへ反映させる
        // 移動・視点入力
        view.OnMoveInputReceived += (input) => model.MoveInput = input;
        view.OnLookInputReceived += (look) =>
        {
            model.CurrentPan += look.x * playerData.rotationSensitivity;

            model.currentPitch -= look.y * playerData.rotationSensitivity;
            model.currentPitch = Mathf.Clamp(model.currentPitch, playerData.minPitch, playerData.maxPitch);
        };

        // 攻撃入力
        view.OnFireInputReceived += () =>
        {
            if (!gunModel.CanShoot()) return;

            if (Time.time >= lastFireTime + gunData.fireRate) ExecuteFire();
        };

        view.OnReloadInputReceived += () =>
        {
            ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();
        };
    }

    private void FixedUpdate()
    {
        // Modelに移動量を計算させる
        Vector3 movement = model.CalcMove(Time.fixedDeltaTime);
        // 計算結果をViewに渡して移動を実行させる
        view.Move(movement);

        view.UpdateBodyRotation(model.CurrentPan);
        view.SetUpperBodyPitch(model.currentPitch);
    }

    private void ExecuteFire()
    {
        lastFireTime = Time.time;

        gunModel.ConsumeAmmo();

        Vector3 fireDirection = Camera.main.transform.forward;
        gunView.LaunchBullet(fireDirection, gunData.speed);
        Debug.Log($"[Fire] Damage: {gunModel.Damage}, Remaining Ammo: {gunModel.CurrentAmmo}");
    }

    private async UniTaskVoid ReloadAsync(CancellationToken token)
    {
        if (gunModel.CurrentAmmo == gunData.maxAmmo || gunModel.IsReloading) return;

        Debug.Log("Reloading started...");
        gunModel.IsReloading = true;

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
