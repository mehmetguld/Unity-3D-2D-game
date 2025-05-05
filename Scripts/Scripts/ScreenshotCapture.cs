using UnityEngine;
using System.IO;
using System;

public class ScreenshotCapture : MonoBehaviour
{
    // Screenshot klasörünün yolu
    private string screenshotFolderPath;
    
    void Start()
    {
        // Assets/Screenshot klasörünün tam yolunu oluştur
        screenshotFolderPath = Path.Combine(Application.dataPath, "Screenshot");
        
        // Klasör yoksa oluştur
        if (!Directory.Exists(screenshotFolderPath))
        {
            Directory.CreateDirectory(screenshotFolderPath);
            Debug.Log("Screenshot klasörü oluşturuldu: " + screenshotFolderPath);
        }
    }
    
    void Update()
    {
        // K tuşuna basıldıysa ekran görüntüsü al
        if (Input.GetKeyDown(KeyCode.K))
        {
            CaptureScreenshot();
        }
    }
    
    void CaptureScreenshot()
    {
        // Dosya adı için tarih ve saat bilgisini kullan
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string fileName = "Screenshot_" + timestamp + ".png";
        
        // Tam dosya yolunu oluştur
        string filePath = Path.Combine(screenshotFolderPath, fileName);
        
        // Ekran görüntüsünü al ve kaydet
        ScreenCapture.CaptureScreenshot(filePath);
        
        Debug.Log("Ekran görüntüsü kaydedildi: " + filePath);
    }
}