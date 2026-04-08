using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeButton : MonoBehaviour
{
    [SerializeField] private string _SceneName;

    public void ChangeScene()
    {
        SceneManager.LoadScene(_SceneName);
    }
}
