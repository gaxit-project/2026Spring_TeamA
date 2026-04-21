using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField] private string NextScene;

    public void ChangeScene()
    {
        SceneManager.LoadScene(NextScene);
    }
}
