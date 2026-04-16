using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    public EnemyView _parentView;
    public bool isHead = false;

    public void NotifyHit(Collider bullet)
    {
        if (_parentView == null)
        {
            _parentView = GetComponentInParent<EnemyView>();
        }
    }
}
