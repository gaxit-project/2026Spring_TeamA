using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPresenter : MonoBehaviour
{
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private ResultView view;

    private void Start()
    {
        view.UpdateKillCountDisplay(SessionData.KillCount);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}
