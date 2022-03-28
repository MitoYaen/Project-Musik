using TMPro;
using System.Collections;
using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int CountFPS = 30;
    public float Duration = 1f;
    int _value;

    private void Awake()
    {
        Text = gameObject.GetComponent<TextMeshProUGUI>();
    }
    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            updateText(value);
            _value = value;
        }
    }
    Coroutine CountingCoroutine;
    void updateText(int newValue)
    {
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }
        CountingCoroutine = StartCoroutine(CountText(newValue));
    }

    IEnumerator CountText(int newValue)
    {
        WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
        int previousValue = _value;
        int stepAmount;
        if (newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration));
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration));
        }

        if(previousValue < newValue)
        {
            while (previousValue < newValue)
            {
                previousValue += stepAmount;
                if (previousValue > newValue)
                {
                    previousValue = newValue;
                }

                Text.text = previousValue.ToString();
                yield return Wait;
            }
        }
        else
        {
            while (previousValue > newValue)
            {
                previousValue += stepAmount;
                if (previousValue < newValue)
                {
                    previousValue = newValue;
                }

                Text.text = previousValue.ToString();
                yield return Wait;
            }
        }

    }
}
