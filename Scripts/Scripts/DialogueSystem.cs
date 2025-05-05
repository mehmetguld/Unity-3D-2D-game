using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    
    [Header("Dialogue Data")]
    public DialogueData dialogueData;
    
    private bool _isInTrigger = false;
    private bool _isDialogueActive = false;
    private int _currentDialogueIndex = 0;
    private List<string> _currentDialogues;
    private int _currentLevel = 0; 
    
    void Start()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
    
    void Update()
    {
        if (_isDialogueActive && Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextDialogue();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Assistant"))
        {
            _isInTrigger = true;
            StartDialogue();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Assistant"))
        {
            _isInTrigger = false;
            EndDialogue();
        }
    }
    
    void StartDialogue()
    {
        int lastCompletedLevel = GetLastCompletedLevel();
        _currentLevel = lastCompletedLevel; 
        
       
        var levelDialogue = dialogueData.levelDialogues.Find(ld => ld.levelNumber == lastCompletedLevel);
        
        if (levelDialogue != null && levelDialogue.dialogues.Count > 0)
        {
            _currentDialogues = levelDialogue.dialogues;
            dialoguePanel.SetActive(true);
            _isDialogueActive = true;
            _currentDialogueIndex = 0;
            dialogueText.text = _currentDialogues[_currentDialogueIndex];
        }
        else
        {
            Debug.LogWarning($"Dialogue not found for level {lastCompletedLevel}!");
        }
    }
    
    void DisplayNextDialogue()
    {
        _currentDialogueIndex++;
        
        if (_currentDialogueIndex < _currentDialogues.Count)
        {
            dialogueText.text = _currentDialogues[_currentDialogueIndex];
        }
        else
        {
            EndDialogue();
        }
    }
    
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        _isDialogueActive = false;
        _currentDialogueIndex = 0;
        dialogueText.text = "";
        
        
        CheckAndLoadNextLevel();
    }
    
    void CheckAndLoadNextLevel()
    {
        
        if (_currentLevel == 0)
        {
            SceneManager.LoadScene("Level1");
        }
      
        else if (_currentLevel == 1)
        {
            SceneManager.LoadScene("Level2");
        }
       
        else if (_currentLevel >= 2)
        {
            
            Debug.Log("All levels are completed!");
        }
    }
    
    int GetLastCompletedLevel()
    {
        int lastLevel = 0;
        
        for (int i = 1; i <= 10; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i + "_Completed", 0) == 1)
            {
                lastLevel = i;
            }
        }
        
        return lastLevel;
    }
}
