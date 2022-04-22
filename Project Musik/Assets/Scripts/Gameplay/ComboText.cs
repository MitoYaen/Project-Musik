using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboText : MonoBehaviour
{
    public RythmGameManager GameManager;
    float Alpha;
    public int ShowCondition;
    public TextMeshProUGUI Combo;
    [Tooltip("¼ÈCombo_Text.")]
    public TextMeshProUGUI Text;

    private void Awake()
    {
        Combo = gameObject.GetComponent<TextMeshProUGUI>();
        Alpha = Combo.color.a;
        Text.color = new Color(Combo.color.r, Combo.color.g, Combo.color.b, 0f);
        Combo.color = new Color(Combo.color.r, Combo.color.g, Combo.color.b, 0f);
    }
    private void Update()
    {
        Combo.text = GameManager.Combo.ToString();
        if (GameManager.Combo > ShowCondition)
        {
            Text.color = new Color(Combo.color.r, Combo.color.g, Combo.color.b, Alpha);
            Combo.color = new Color(Combo.color.r, Combo.color.g, Combo.color.b, Alpha);
        }
        else
        {
            Text.color = new Color(Combo.color.r, Combo.color.g, Combo.color.b, 0f);
            Combo.color = new Color(Combo.color.r, Combo.color.g, Combo.color.b, 0f);
        }
    }
}
