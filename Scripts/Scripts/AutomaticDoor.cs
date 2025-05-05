using UnityEngine;
using DG.Tweening;

public class AutomaticDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorTransform;
    public float openPosition;
    public float openDuration = 1f;
    public bool xSide;
    
    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    
    private Vector3 _closedPosition;
    private bool _isOpen = false;

    void Start()
    {
       
        _closedPosition = doorTransform.position;
        
       
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isOpen)
        {
            OpenDoor();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _isOpen)
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        _isOpen = true;
       
        if (audioSource != null && doorOpenSound != null)
            audioSource.PlayOneShot(doorOpenSound);

        if (!xSide)
        {
            doorTransform.DOMoveZ(openPosition, openDuration).SetEase(Ease.InOutQuad);
        }
        else
        {
            doorTransform.DOMoveX(openPosition, openDuration).SetEase(Ease.InOutQuad);
        }
        
    }

    void CloseDoor()
    {
        _isOpen = false;
 
        if (audioSource != null && doorCloseSound != null)
            audioSource.PlayOneShot(doorCloseSound);
        
       
        doorTransform.DOMove(_closedPosition, openDuration).SetEase(Ease.InOutQuad);
    }
}