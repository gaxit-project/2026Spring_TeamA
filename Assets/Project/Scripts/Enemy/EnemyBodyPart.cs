using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    public EnemyView _parentView;
    public bool isHead = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            _parentView.HitContact?.Invoke(other, this.GetComponent<Collider>());
        }
    }
}
