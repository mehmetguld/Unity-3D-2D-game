using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class LevelDialogue
    {
        public int levelNumber;
        [TextArea(3, 10)]
        public List<string> dialogues;
    }
    
    public List<LevelDialogue> levelDialogues;
}