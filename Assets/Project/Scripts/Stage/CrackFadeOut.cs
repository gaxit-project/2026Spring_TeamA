using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(MeshRenderer))]
public class CrackFadeOut : MonoBehaviour
{
    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.DOFade(0f, 1.0f)
                .SetDelay(1.0f)
                .OnComplete(() => Destroy(gameObject));

            if (meshRenderer.material.HasProperty("_EmissionColor"))
            {
                meshRenderer.material.DOColor(Color.black, "_EmissionColor", 1.0f)
                    .SetDelay(1.0f);
            }
        }
    }
}
