using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    [SerializeField] private Transform destination;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (destination != null)
            {
                // 直接ワープさせず、演出管理クラスにお願いする
                GamePresenter.Instance.TriggerWarp(destination);
            }
            else
            {
                Debug.LogWarning("ワープ先の Destination が未設定です！");
            }
        }
    }
}
