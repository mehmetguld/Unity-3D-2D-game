using UnityEngine;

public class CanvasTriggerController : MonoBehaviour
{
    public GameObject objectToShow;

    void Start()
    {
        if (objectToShow != null)
            objectToShow.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && objectToShow != null)
        {
            objectToShow.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && objectToShow != null)
        {
            objectToShow.SetActive(false);
        }
    }
}