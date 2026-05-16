using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    public EnemyView _parentView;
    public enum HitPartType { Default, Head, Body, Arm, Leg }
    public HitPartType partType = HitPartType.Default;

    public void NotifyHit(int damage)
    {
        if (_parentView != null)
        {
            Debug.Log($"NotifyHit: Sending damage {damage} to {_parentView.name}");
            _parentView.ReceiveDamage(damage, this.GetComponent<Collider>(), partType);
        }
        else
        {
            Debug.LogError($"NotifyHit failed: _parentView is still null on {gameObject.name}");
        }
    }
}
