using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPresenter : MonoBehaviour
{
    [SerializeField] private EnemyView view;
    private GameObject _target;     // 追跡対象

    private void Awake()
    {
        view.OnContactStay += (other) => Contact(other, true);
        view.OffContactExit += (other) => Contact(other, false);
    }

    private void Start()
    {
        _target = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        view.MoveTo(_target.transform.position);
    }

    private void Contact(Collider other, bool isAttacking)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isAttacking)
            {
                view.OnAttack();
            }
            else
            {
                view.OffAttack();
            }
        }
    }
}
