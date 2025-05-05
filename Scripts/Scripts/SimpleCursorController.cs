using UnityEngine;

public class SimpleCursorController : MonoBehaviour
{
 
    
    void Start()
    {
     
        Cursor.visible = true;
      
        Cursor.lockState = CursorLockMode.None;
        
    }
}