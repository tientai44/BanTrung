using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G4_FPSCounter : MonoBehaviour
{
    private float deltaTime;
    private float fps;
    [SerializeField] Text txt; 
    private void Update()
    {
        // Tính thời gian mất để vẽ frame trước đó
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        // Tính FPS
        fps = 1.0f / deltaTime;
        txt.text = fps.ToString();
    }

   
}
