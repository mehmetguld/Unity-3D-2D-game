using UnityEngine;
using System.Collections;

public class EmergencyLightEffect : MonoBehaviour
{
    [Header("Light Settings")]
    public Light[] spotLights;
    public Light[] pointLights;
    
    [Header("Material Settings")]
    public Material emissionMaterial;
    private string _emissionKeyword = "_EMISSION";
    private string _emissionColorProperty = "_EmissionColor";
    
    [Header("Color Settings")]
    public Color redColor = new Color(1f, 0f, 0f, 1f);
    public Color whiteColor = new Color(1f, 1f, 1f, 1f);
    public float redEmissionIntensity = 2f;
    public float whiteEmissionIntensity = 4f;
    
    [Header("Animation Settings")]
    public float flashSpeed = 2f;
    public bool useAlternatingPattern = true; 
    
    private bool _isRunning = false;
    private Coroutine _emergencyRoutine;

    void Start()
    {
        
        if (emissionMaterial != null)
        {
            emissionMaterial.EnableKeyword(_emissionKeyword);
        }
        
        StartEmergencyLights();
    }

    private void StartEmergencyLights()
    {
        if (!_isRunning)
        {
            _isRunning = true;
            _emergencyRoutine = StartCoroutine(EmergencyLightRoutine());
        }
    }

    public void StopEmergencyLights()
    {
        if (_isRunning && _emergencyRoutine != null)
        {
            StopCoroutine(_emergencyRoutine);
            _isRunning = false;
            ResetLights();
        }
    }

    void ResetLights()
    {
        
        foreach (var light in spotLights)
        {
            if (light != null)
                light.color = whiteColor;
        }
        
        foreach (var light in pointLights)
        {
            if (light != null)
                light.color = whiteColor;
        }
        
       
        if (emissionMaterial != null)
        {
            emissionMaterial.SetColor(_emissionColorProperty, whiteColor);
        }
    }

    IEnumerator EmergencyLightRoutine()
    {
        float time = 0;
        bool isRed = true;
        
        while (_isRunning)
        {
            time += Time.deltaTime * flashSpeed;
            
            if (useAlternatingPattern)
            {
              
                for (int i = 0; i < spotLights.Length; i++)
                {
                    if (spotLights[i] != null)
                    {
                        bool lightIsRed = (i % 2 == 0) ? isRed : !isRed;
                        spotLights[i].color = lightIsRed ? redColor : whiteColor;
                    }
                }
                
                for (int i = 0; i < pointLights.Length; i++)
                {
                    if (pointLights[i] != null)
                    {
                        bool lightIsRed = (i % 2 == 0) ? isRed : !isRed;
                        pointLights[i].color = lightIsRed ? redColor : whiteColor;
                    }
                }
            }
            else
            {
               
                Color currentColor = isRed ? redColor : whiteColor;
                
                foreach (var light in spotLights)
                {
                    if (light != null)
                        light.color = currentColor;
                }
                
                foreach (var light in pointLights)
                {
                    if (light != null)
                        light.color = currentColor;
                }
            }
            
          
            if (emissionMaterial != null)
            {
                Color emissionColor = isRed ? redColor : whiteColor;
                float intensity = isRed ? redEmissionIntensity : whiteEmissionIntensity;
                
                
                Color finalEmissionColor = emissionColor * Mathf.Pow(2, intensity);
                emissionMaterial.SetColor(_emissionColorProperty, finalEmissionColor);
            }
            
           
            if (time >= 1)
            {
                time = 0;
                isRed = !isRed;
            }
            
            yield return null;
        }
    }

    void OnDestroy()
    {
       
        if (emissionMaterial != null)
        {
            emissionMaterial.SetColor(_emissionColorProperty, whiteColor);
        }
    }
}