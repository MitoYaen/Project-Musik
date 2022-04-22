using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class FpsDisplay : MonoBehaviour
{
    public int avgFrameRate;
    public Text display_Text;

    public void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        display_Text = GetComponent<Text>();
        Application.targetFrameRate = 60;
    }
    public void Update()
    {
        if (Time.frameCount % 30 == 0)
        {
            float current = 0;
            current = (int)(1f / Time.unscaledDeltaTime);
            avgFrameRate = Mathf.FloorToInt(current);
            display_Text.text = "FPS : " + avgFrameRate.ToString();
        }
    }

}
