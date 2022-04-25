using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NoteSpeedButton : MonoBehaviour
{
    [SerializeField] private float noteSpeedMin;
    [SerializeField] private float noteSpeedMax;
    [SerializeField] private TextMeshProUGUI SpeedText;
    private Animator SpeedTextAnim;

    public void Start()
    {
        SpeedTextAnim = SpeedText.GetComponent<Animator>();
        if (PlayerPrefs.HasKey("NoteSpeed"))
        {
            //PlayerPrefs.SetFloat("NoteSpeed", 4f);
            SpeedText.text = PlayerPrefs.GetFloat("NoteSpeed").ToString("f1");
        }
        else
        {
            PlayerPrefs.SetFloat("NoteSpeed", 4f);
            SpeedText.text = PlayerPrefs.GetFloat("NoteSpeed").ToString("f1");
        }
    }

    public void Add(float value)
    {
        if (PlayerPrefs.GetFloat("NoteSpeed") > noteSpeedMax || PlayerPrefs.GetFloat("NoteSpeed") < noteSpeedMin )
        {
            return;
        }
        if (PlayerPrefs.GetFloat("NoteSpeed") + value > noteSpeedMax)
        {
            PlayerPrefs.SetFloat("NoteSpeed", noteSpeedMax);
            SpeedText.text = PlayerPrefs.GetFloat("NoteSpeed").ToString("f1");
            SpeedTextAnim.SetTrigger("Add");
            return;
        }
        else if (PlayerPrefs.GetFloat("NoteSpeed") + value < noteSpeedMin)
        {
            PlayerPrefs.SetFloat("NoteSpeed", noteSpeedMin);
            SpeedText.text = PlayerPrefs.GetFloat("NoteSpeed").ToString("f1");
            SpeedTextAnim.SetTrigger("Sub");
            return;
        }

        PlayerPrefs.SetFloat("NoteSpeed", PlayerPrefs.GetFloat("NoteSpeed") + value);
        SpeedText.text = PlayerPrefs.GetFloat("NoteSpeed").ToString("f1");
        if (value > 0) //if add
        {
            SpeedTextAnim.SetTrigger("Add");
        }
        else
        {
            SpeedTextAnim.SetTrigger("Sub");
        }
    }

}
