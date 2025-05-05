using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Level Buttons")]
    public Button level0Button;
    public Button level1Button;
    public Button level2Button;
    public Button intermediateSceneButton; 
    public Button cleanLevelButton; 
    public Button resetButton;
    
    void Start()
    {
       
        level0Button.onClick.AddListener(() => LoadScene("Level0"));
        level1Button.onClick.AddListener(() => LoadScene("Level1"));
        level2Button.onClick.AddListener(() => LoadScene("Level2"));
        intermediateSceneButton.onClick.AddListener(() => LoadScene("Memories"));
        cleanLevelButton.onClick.AddListener(() => LoadScene("CleanLevel"));
        resetButton.onClick.AddListener(ResetAllProgress);
        
       
        UpdateButtonStates();
    }
    
    void UpdateButtonStates()
    {
       
        level0Button.interactable = true;
        intermediateSceneButton.interactable = true;
        resetButton.interactable = true;
        
       
        level1Button.interactable = PlayerPrefs.GetInt("Level0_Completed", 0) == 1;
        
       
        level2Button.interactable = PlayerPrefs.GetInt("Level1_Completed", 0) == 1;
        
        
        cleanLevelButton.interactable = PlayerPrefs.GetInt("Level2_Completed", 0) == 1;
    }
    
    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    void ResetAllProgress()
    {
       
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        
        UpdateButtonStates();
        
        Debug.Log("All progress has been reset!");
    }
}