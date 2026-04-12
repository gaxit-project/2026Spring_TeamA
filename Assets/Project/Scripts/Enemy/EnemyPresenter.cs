using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class EnemyPresenter : MonoBehaviour
{
    [SerializeField] private EnemyView view;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private GunData gunData;
    [SerializeField] private List<EnemyBodyPart> bodyParts;

    private EnemyModel model;

    private GameObject _target;     // 追跡対象
    private bool _isDead = false;

    private void Awake()
    {
        model = new EnemyModel(enemyData);  // Modelに ScriptableObject を渡して初期化
         
        view.OnContactStay += (other) => Contact(other, true);
        view.OffContactExit += (other) => Contact(other, false);
        view.HitContact += (bullet, hitCollider) => OnHit(bullet, hitCollider);
    }

    private void Start()
    {
        _target = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        if(_isDead)
        {
            return;
        }

        view.MoveTo(_target.transform.position);
    }

    private void Contact(Collider other, bool isAttacking)
    {
        // 攻撃範囲
        if (other.gameObject.tag == "Player")
        {
            if (isAttacking)
            {
                view.OnAttack();

                var playerView = other.gameObject.GetComponent<PlayerView>();
                if (playerView != null)
                {
                    playerView.OnHitByEnemy?.Invoke(enemyData);
                }
            }
            else
            {
                view.OffAttack();
            }
        }
    }

    private void OnHit(Collider bullet, Collider hitCollider)
    {
        var hitPart = bodyParts.Find(x => x.GetComponent<Collider>() == hitCollider);

        if(hitPart == null)
        {
            return;
        }

        int takeDamage = hitPart.isHead ? gunData.damage * 2 : gunData.damage;

        model.TakeDamage(takeDamage);

        view.Hit().Forget();
        Destroy(bullet.gameObject);

        if (model.CullentHP <= 0 && !_isDead)
        {
            _isDead = true;
            view.Die();

        }
    }
}
