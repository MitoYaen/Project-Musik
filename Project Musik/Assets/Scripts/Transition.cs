using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;


public class Transition : MonoBehaviour
{
    public float Duration = 0.3f;
    public float WaitDuration = 5f;
    public static Transition Instance { get; private set; }
    public RectTransform Mask;
    internal int CurrentState;
    internal int NextState;
    /*
     * 0 - Right
     * 1 - Mid 
     * 2 - Left
     */

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RightToMid()
    {
        Mask.anchoredPosition = new Vector2(2800, 0);
        Mask.DOAnchorPosX(0, Duration).SetEase(Ease.OutCubic);
        CurrentState = 1;
        StartCoroutine(NextStep());
        NextState = 2;
    }
    public void MidToLeft()
    {
        Mask.anchoredPosition = new Vector2(0, 0);
        Mask.DOAnchorPosX(-2800, Duration).SetEase(Ease.InCubic);
        CurrentState = 2;
    }
    public void LeftToMid()
    {
        Mask.anchoredPosition = new Vector2(-2800, 0);
        Mask.DOAnchorPosX(0, Duration).SetEase(Ease.OutCubic);
        CurrentState = 2;
        StartCoroutine(NextStep());
        NextState = 0;
    }
    public void MidToRight()
    {
        Mask.anchoredPosition = new Vector2(0, 0);
        Mask.DOAnchorPosX(2800, Duration).SetEase(Ease.InCubic);
        CurrentState = 0;
    }

    public IEnumerator NextStep()
    {
        yield return new WaitForSeconds(WaitDuration);
        switch (NextState)
        {
            case 0:
                MidToRight();
                break;
            case 2:
                MidToLeft();
                break;
            default:
                break;
        }
    }
}
