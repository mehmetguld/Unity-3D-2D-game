using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button restartButton;
    public Button continueButton;
    public Button closePanelButton;
    public Button mainMenuButton;
    
    private bool _isPaused = false;
    
    void Start()
    {
       
        pauseMenuPanel.SetActive(false);
        
        
        restartButton.onClick.AddListener(RestartLevel);
        continueButton.onClick.AddListener(ContinueGame);
        closePanelButton.onClick.AddListener(ContinueGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }
    
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; 
        _isPaused = true;
        
       
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ContinueGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; 
        _isPaused = false;
        
      
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
    
    
    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}