using UnityEngine;

public class Level2ObjectActivator : MonoBehaviour
{
    [Header("Object References")]
    public GameObject targetObject;
    
    void Start()
    {
        CheckAndSetObjectState();
    }
    
    void OnEnable()
    {
        CheckAndSetObjectState();
    }
   
    public void CheckAndSetObjectState()
    {
        if (targetObject != null)
        {
            bool isLevel2Completed = PlayerPrefs.GetInt("Level2_Completed", 0) == 1;
            targetObject.SetActive(!isLevel2Completed);
        }
    }
}