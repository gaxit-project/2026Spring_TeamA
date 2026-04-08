using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField] private string _NextScene;

    public void ChangeScene()
    {
        SceneManager.LoadScene(_NextScene);
    }
}
