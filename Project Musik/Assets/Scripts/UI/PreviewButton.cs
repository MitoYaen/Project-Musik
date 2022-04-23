using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewButton : MonoBehaviour
{
    private Button button;
    private LevelInfoSender LIS;

    private void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate { LIS.ChooseSong(); });
        button.interactable = false;
        
    }
    
    public void SentLevel(LevelInfoSender sender)
    {
        button.interactable = true;
        LIS = sender;
    }
}
