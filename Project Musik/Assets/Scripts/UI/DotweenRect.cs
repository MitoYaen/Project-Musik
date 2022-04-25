using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DotweenRect : MonoBehaviour
{
    [SerializeField] RectTransform TargetRectTransform;
    [SerializeField] float Duration;
    [SerializeField] Vector2 StartingPos;
    [SerializeField] Vector2 EndingPos;
    private bool isCome = false;
    //[Header ("Cooldown")]
    //public bool HasCoolDown;
    //[SerializeField] float Cooldown;

    public void RectMove()
    {
        StartCoroutine(AllerRetoursRect());
    }
    public IEnumerator AllerRetoursRect() 
    {
        isCome = !isCome;
        DOTween.Clear();
        if (isCome)
        {
            TargetRectTransform.anchoredPosition = StartingPos;
            TargetRectTransform.DOAnchorPos(EndingPos, Duration).SetEase(Ease.OutCubic);
        }
        else
        {
            TargetRectTransform.anchoredPosition = EndingPos;
            TargetRectTransform.DOAnchorPos(StartingPos, Duration).SetEase(Ease.OutCubic);
        }
        yield return null;
    }
}
