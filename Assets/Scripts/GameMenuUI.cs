using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private TMP_Text muteButtonText;
    [SerializeField] private Board chessBoard;
    [SerializeField] private GameObject themePanel;
    [SerializeField] private ChessManager2D chessManager;
    public bool isMuted = false;

    private void Start()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
        if (menuButton != null)
            menuButton.SetActive(true);
        if (muteButtonText != null)
            muteButtonText.text = "Mute";
        if (themePanel != null)
            themePanel.SetActive(false);
    }
    void Update()
    {
        if (menuPanel != null && menuPanel.activeSelf && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            // Kiểm tra click vào UI
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                CloseMenu(); // nếu click không trúng UI → đóng menu
            }
        }
    }

    public void OpenThemePanel()
    {
        if (themePanel != null)
            themePanel.SetActive(true);
        if (menuPanel != null) 
            menuPanel.SetActive(false);
    }

    public void CloseThemePanel()
    {
        if (themePanel != null)
            themePanel.SetActive(false);
        if (menuButton != null) 
            menuButton.SetActive(true);
    }
    
    public void SelectThemeAndClose(int index)
    {
        chessManager.SelectTheme(index);
        if (themePanel != null) themePanel.SetActive(false);
        if (menuButton != null) 
            menuButton.SetActive(true);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.pause = isMuted;

        if (muteButtonText != null)
            muteButtonText.text = isMuted ? "Sound" : "Mute";
    }

    // Bấm nút Menu để bật/tắt panel
    public void ToggleMenu()
    {
        if (menuButton != null) menuButton.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (menuButton != null) menuButton.SetActive(true);
    }

    // Nút Restart
    public void OnRestart()
    {
        Time.timeScale = 1f;
        Debug.Log("🔁 Restart pressed, current theme = " + PlayerPrefs.GetInt("BoardTheme", -1));
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Nút Quit
    public void OnQuit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}