using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;

public class PasswordDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorTransform;
    public float openPosition;
    public float openDuration = 1f;
    
    [Header("UI Elements")]
    public GameObject passwordCanvas;
    public TMP_InputField passwordInputField;
    public TMP_Text passwordDisplayText; 
    public Transform canvasTransform;
    
    [Header("Password Settings")]
    public string correctPassword = "1234";
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public AudioClip wrongPasswordSound;
    public AudioClip correctPasswordSound;
    public AudioClip keyPressSound;
    
    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeStrength = 20f;
    public int shakeVibrato = 15;
    
    private bool _isPasswordCorrect = false;
    private bool _isPlayerInTrigger = false;
    private Vector3 _closedPosition;
    private bool _isShaking = false;
    private string _lastInputValue = "";

    void Start()
    {
        _closedPosition = doorTransform.position;
        passwordCanvas.SetActive(false);
        
        if (canvasTransform == null && passwordCanvas != null)
            canvasTransform = passwordCanvas.GetComponent<RectTransform>();
        
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
    
        if (passwordInputField != null)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            passwordInputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
            
     
            passwordInputField.onValueChanged.AddListener(OnPasswordInput);
            passwordInputField.onSubmit.AddListener(CheckPassword);
        }

      
        if (passwordDisplayText != null)
            passwordDisplayText.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isPasswordCorrect)
        {
            _isPlayerInTrigger = true;
            ShowPasswordCanvas();
        }
        else if (other.CompareTag("Player") && _isPasswordCorrect)
        {
            OpenDoor();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
            if (!_isPasswordCorrect)
            {
                HidePasswordCanvas();
            }
            else
            {
                CloseDoor();
            }
        }
    }

    void ShowPasswordCanvas()
    {
        passwordCanvas.SetActive(true); 
        
        if (passwordInputField != null)
        {
            passwordInputField.text = "";
            _lastInputValue = "";
            passwordInputField.Select();
            passwordInputField.ActivateInputField();
        }

        if (passwordDisplayText != null)
            passwordDisplayText.text = "";
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HidePasswordCanvas()
    {
        passwordCanvas.SetActive(false);
        
        if (passwordInputField != null)
        {
            passwordInputField.text = "";
            passwordInputField.DeactivateInputField();
        }

        if (passwordDisplayText != null)
            passwordDisplayText.text = "";
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnPasswordInput(string value)
    {
      
        if (passwordDisplayText != null)
        {
            passwordDisplayText.text = new string('*', value.Length);
        }

        
        if (value.Length != _lastInputValue.Length)
        {
            PlaySound(keyPressSound);
        }
        
        _lastInputValue = value;
    }

    void CheckPassword(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;

        if (input == correctPassword)
        {
            _isPasswordCorrect = true;
            PlaySound(correctPasswordSound);
            HidePasswordCanvas();
            StartCoroutine(OpenDoorAfterSound());
        }
        else
        {
            PlaySound(wrongPasswordSound);
            ShakeCanvas();
            
          
            if (passwordInputField != null)
            {
                passwordInputField.text = "";
                passwordInputField.ActivateInputField();
            }
            
           
            if (passwordDisplayText != null)
            {
                passwordDisplayText.text = "";
            }
        }
    }

    IEnumerator OpenDoorAfterSound()
    {
        yield return new WaitForSeconds(0.5f);
        OpenDoor();
    }

    void ShakeCanvas()
    {
        if (canvasTransform != null && !_isShaking)
        {
            _isShaking = true;
            
            canvasTransform.DOShakeRotation(shakeDuration, new Vector3(0, 0, shakeStrength), shakeVibrato, 90, true)
                .OnComplete(() => {
                    canvasTransform.rotation = Quaternion.identity;
                    _isShaking = false;
                });
        }
    }

    void OpenDoor()
    {
        PlaySound(doorOpenSound);
        doorTransform.DOMoveX(openPosition, openDuration).SetEase(Ease.InOutQuad);
    }

    void CloseDoor()
    {
        PlaySound(doorCloseSound);
        doorTransform.DOMove(_closedPosition, openDuration).SetEase(Ease.InOutQuad);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void Update()
    {
        if (passwordCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HidePasswordCanvas();
        }
    }
}