using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CheckNotifier : MonoBehaviour
{
    [Header("Check Text")] [SerializeField]
    private TMP_Text checkText;

    [SerializeField] private TMP_Text checkmateText;
    [SerializeField] private TMP_Text stalemateText;

    [SerializeField] private string checkMessage = "CHECK!";
    [SerializeField] private string checkmateMessage = "CHECKMATE!";
    [SerializeField] private string stalemateMessage = "DRAW!!!";

    [Header("End Game UI")] [SerializeField]
    private GameObject endGamePanel;

    [SerializeField] private TMP_Text endGameMessage;

    private Coroutine activeCoroutine;

    private void Awake()
    {
        if (checkText != null) checkText.gameObject.SetActive(false);
        if (checkmateText != null) checkmateText.gameObject.SetActive(false);
        if (stalemateText != null) stalemateText.gameObject.SetActive(false);
        if (endGamePanel != null) endGamePanel.SetActive(false);
    }

    public void ShowCheck(float duration = 2f)
    {
        ShowMessage(checkText, checkMessage, duration);
    }

    public void ShowCheckmate()
    {
        ShowMessage(checkmateText, checkmateMessage, 0);
        ShowEndGame(checkmateMessage);
    }

    public void ShowStalemate()
    {
        ShowMessage(stalemateText, stalemateMessage, 0);
        ShowEndGame(stalemateMessage);
    }

    public void HideAll()
    {
        if (checkText != null) checkText.gameObject.SetActive(false);
        if (checkmateText != null) checkmateText.gameObject.SetActive(false);
        if (stalemateText != null) stalemateText.gameObject.SetActive(false);
    }

    private void ShowMessage(TMP_Text target, string message, float duration)
    {
        if (target == null) return;

        target.text = message;
        target.gameObject.SetActive(true);

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        if (duration > 0)
            activeCoroutine = StartCoroutine(HideAfterDelay(target, duration));
    }


    private IEnumerator HideAfterDelay(TMP_Text target, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.gameObject.SetActive(false);
    }

    public void ShowEndGame(string message)
    {
        if (endGameMessage != null)
            endGameMessage.text = message;

        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        Time.timeScale = 0f; // dừng game
    }

    // ✅ Nút Restart
    public void OnRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ✅ Nút Quit
    public void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}