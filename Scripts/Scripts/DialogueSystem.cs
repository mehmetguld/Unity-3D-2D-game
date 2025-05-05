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
    private bool _dialogueCompleted = false;
    
   
    private const int IntroDıalogueLevel = 99;
    private const int FınaleDıalogueLevel = 100;
    
    void Start()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        _dialogueCompleted = false;
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
         
            CloseDialogueUI();
        }
    }
    
    void StartDialogue()
    {
        // If dialogue was already completed, don't start again
        if (_dialogueCompleted)
            return;
        
        int lastCompletedLevel = GetLastCompletedLevel();
        
       
        DialogueData.LevelDialogue levelDialogue;
        
        if (lastCompletedLevel == -1) 
        {
        
            levelDialogue = dialogueData.levelDialogues.Find(ld => ld.levelNumber == IntroDıalogueLevel);
            _currentLevel = IntroDıalogueLevel;
        }
        else if (lastCompletedLevel == 2)
        {
            
            bool isFirstTimeLevel2Completion = PlayerPrefs.GetInt("Level2_DialogueShown", 0) == 0;
            
            if (isFirstTimeLevel2Completion)
            {
                
                levelDialogue = dialogueData.levelDialogues.Find(ld => ld.levelNumber == 2);
                _currentLevel = 2;
             
                PlayerPrefs.SetInt("Level2_DialogueShown", 1);
                PlayerPrefs.Save();
                
            }
            else
            {
               
                levelDialogue = dialogueData.levelDialogues.Find(ld => ld.levelNumber == FınaleDıalogueLevel);
                _currentLevel = FınaleDıalogueLevel;
            }
        }
        else
        {
            levelDialogue = dialogueData.levelDialogues.Find(ld => ld.levelNumber == lastCompletedLevel);
            _currentLevel = lastCompletedLevel;
           
        }
        
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
            Debug.LogWarning($"Dialogue not found for level {_currentLevel}!");
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
            CompleteDialogue();
        }
    }
    
    // New method to just close the UI without completing the dialogue
    void CloseDialogueUI()
    {
        dialoguePanel.SetActive(false);
        _isDialogueActive = false;
        // We don't reset _currentDialogueIndex so the dialogue can resume
       
    }
    
    // New method for when dialogue is fully completed
    void CompleteDialogue()
    {
        dialoguePanel.SetActive(false);
        _isDialogueActive = false;
        _dialogueCompleted = true;
        dialogueText.text = "";
        
       
        if (_currentLevel == 2)
        {
            PlayerPrefs.SetInt("Level2_DialogueShown", 1);
            PlayerPrefs.Save();
         
        }
        
      
        if (_currentLevel == FınaleDıalogueLevel)
        {
            
            Debug.Log("Finale dialogue completed. No level to load.");
        }
        else
        {
          
            LoadNextLevel();
        }
    }
    
    void LoadNextLevel()
    {
        if (_currentLevel == IntroDıalogueLevel)
        {
            SceneManager.LoadScene("Level0");
        }
        else if (_currentLevel == 0)
        {
            SceneManager.LoadScene("Level1");
        }
        else if (_currentLevel == 1)
        {
            SceneManager.LoadScene("Level2");
        }
        else if (_currentLevel == 2)
        {
            Debug.Log("Level 2 completed! Game finished.");
        }
    }
    
    int GetLastCompletedLevel()
    {
        int lastLevel = -1;
        
      
        if (PlayerPrefs.GetInt("Level0_Completed", 0) == 1)
        {
            lastLevel = 0;
        }
        
      
        if (PlayerPrefs.GetInt("Level1_Completed", 0) == 1)
        {
            lastLevel = 1;
        }
        
      
        if (PlayerPrefs.GetInt("Level2_Completed", 0) == 1)
        {
            lastLevel = 2;
        }
        
        return lastLevel;
    }
    
    bool IsAllLevelsCompleted()
    {
       
        return PlayerPrefs.GetInt("Level2_Completed", 0) == 1;
    }
}