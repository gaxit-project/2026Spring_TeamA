using UnityEditorInternal;
using UnityEngine;

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] private PlayerView view;
    [SerializeField] private PlayerData playerData;

    private PlayerModel model;

    private void Awake()
    {
        // Model に ScriptableObject を渡して初期化
        model = new PlayerModel(playerData);

        // Viewの入力イベントを購読し、Modelのデータへ反映させる
        view.OnMoveInputReceived += (input) => model.MoveInput = input;
        view.OnLookInputReceived += (look) =>
        {
            model.CurrentPan += look.x * playerData.rotationSensitivity;

            model.currentPitch -= look.y * playerData.rotationSensitivity;
            model.currentPitch = Mathf.Clamp(model.currentPitch, playerData.minPitch, playerData.maxPitch);
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
}
