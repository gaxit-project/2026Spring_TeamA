using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    public EnemyView _parentView;
    public bool isHead = false;

    public void NotifyHit(int damage)
    {
        /*if (_parentView == null)
        {
            _parentView.HitContact.Invoke(damage, this.GetComponent<Collider>());
        }*/

        if (_parentView != null)
        {
            Debug.Log($"NotifyHit: Sending damage {damage} to {_parentView.name}");
            _parentView.ReceiveDamage(damage, this.GetComponent<Collider>());
        }
        else
        {
            Debug.LogError($"NotifyHit failed: _parentView is still null on {gameObject.name}");
        }
    }
}
