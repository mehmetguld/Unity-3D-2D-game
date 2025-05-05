using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderTrigger : MonoBehaviour
{
    public int levelNumber; 
    public string memories = "Memories"; 
    public float delayBeforeLoad = 0.5f;

    private bool _isLoading = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isLoading)
        {
            _isLoading = true;
            PlayerPrefs.SetInt("Level" + levelNumber + "_Completed", 1);
            PlayerPrefs.Save();
            LoadLevel();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isLoading)
        {
            _isLoading = true;
            PlayerPrefs.SetInt("Level" + levelNumber + "_Completed", 1);
            PlayerPrefs.Save();
            LoadLevel();
        }
    }

    void LoadLevel()
    {
        if (!string.IsNullOrEmpty(memories))
        {
            Invoke(nameof(LoadScene), delayBeforeLoad);
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(memories);
    }
}