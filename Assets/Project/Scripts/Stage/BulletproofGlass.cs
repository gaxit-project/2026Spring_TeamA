using UnityEngine;

public class BulletproofGlass : MonoBehaviour
{
    [SerializeField] private GameObject crackPrefab;

    public void AddCrack(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (crackPrefab != null)
        {
            Vector3 spawnPos = hitPoint + hitNormal * 0.001f;

            Quaternion spawnRot = Quaternion.LookRotation(-hitNormal);

            GameObject crack = Instantiate(crackPrefab, spawnPos, spawnRot, transform);

            crack.transform.Rotate(0, 0, Random.Range(0f, 360f));

            Debug.Log("Glass crack");
        }
    }
}
