using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppVariables : MonoBehaviour
{
    public void SaveNoteSpeed(float val)
    {
        PlayerPrefs.SetFloat("NoteSpeed", val);
        PlayerPrefs.Save();
    }
}
