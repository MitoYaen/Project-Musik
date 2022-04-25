using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private bool onHard ;
    private string LastClickedLevel;
    public AudioSource MainLoop;
    [Header("For Previewing Levels")]
    [SerializeField] private RectTransform PreviewObj;
    [SerializeField] private Text PreviewText;
    [SerializeField] private Image PreviewImage;
    public AudioSource PreviewSong;

    [Header("For Starting Levels")]
    public AudioClip StartingSFX;
    [Header("For Switching Levels")]
    [SerializeField] private float DiffShowTimeCost;
    [SerializeField] private Text TextDifficulty;
    [SerializeField] private RectTransform EZList;
    [SerializeField] private RectTransform HDList;

    private void Update()
    {
        if (PreviewSong.isPlaying)
        {
            MainLoop.volume = 0;
        }
        else
        {
            MainLoop.volume = 0.5f;
        }
    }

    public void ChangeDiff()
    {
        onHard = !onHard;
        if (onHard)
        {
            ShowHD();
        }
        else
        {
            ShowEZ();
        }
    }
    public bool isSame(string level)
    {
        if (LastClickedLevel == null) // First Time Click
        {
            LastClickedLevel = level;
            return false;
        }
        if (level == LastClickedLevel)
        {
            return true;
        }
        else
        {
            LastClickedLevel = level;
            return false;
        }
    }
    public IEnumerator Preview(Sprite songImage,string songName,AudioClip songPreview)
    {
        PreviewText.text = songName;
        PreviewImage.sprite = songImage;
        PreviewSong.clip = songPreview;
        PreviewSong.Play();
        DOTween.Clear();
        PreviewObj.anchoredPosition = new Vector2(-700, -72);
        PreviewObj.DOAnchorPosX(300, 1f).SetEase(Ease.OutCubic);
        yield return null;

    }
    private void ShowEZ()
    {
        TextDifficulty.text = "EASY";
        HDList.gameObject.SetActive(false);
        EZList.gameObject.SetActive(true);
        DOTween.Clear();
        EZList.anchoredPosition = new Vector2(600, -145);
        EZList.GetComponent<RectTransform>().DOAnchorPosX(-430, DiffShowTimeCost).SetEase(Ease.OutCubic);
    }
    private void ShowHD()
    {
        TextDifficulty.text = "HARD";
        EZList.gameObject.SetActive(false);
        HDList.gameObject.SetActive(true);
        DOTween.Clear();
        HDList.anchoredPosition = new Vector2(600, -145);
        HDList.GetComponent<RectTransform>().DOAnchorPosX(-430, DiffShowTimeCost).SetEase(Ease.OutCubic);
    }


}
