using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;


public class Transition : MonoBehaviour
{
    public float Duration = 0.3f;
    public float WaitDuration = 2f;
    public static Transition Instance { get; private set; }
    public RectTransform Mask;
    public GameObject SongInfo;
    public Image songimage;
    public TextMeshProUGUI songname;
    public TextMeshProUGUI authur;
    public TextMeshProUGUI Illustrator;
    public TextMeshProUGUI LD;


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
            SongInfo.SetActive(false);
            MidToLeft();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RightToMid()
    {
        Application.targetFrameRate = 60;
        Mask.anchoredPosition = new Vector2(2800, 0);
        Mask.DOAnchorPosX(0, Duration).SetEase(Ease.OutCubic);
        SongInfo.SetActive(true);
        CurrentState = 1;
        NextState = 2;
    }
    public void MidToLeft()
    {
        Mask.anchoredPosition = new Vector2(0, 0);
        Mask.DOAnchorPosX(-2800, Duration).SetEase(Ease.InCubic);
        CurrentState = 0;
    }
    public void LeftToMid()
    {
        Application.targetFrameRate = 60;
        SongInfo.SetActive(false);
        Mask.anchoredPosition = new Vector2(-2800, 0);
        Mask.DOAnchorPosX(0, Duration).SetEase(Ease.OutCubic);
        CurrentState = 2;
        NextState = 0;
    }
    public void MidToRight()
    {
        Mask.anchoredPosition = new Vector2(0, 0);
        Mask.DOAnchorPosX(2800, Duration).SetEase(Ease.InCubic);
        CurrentState = 2;
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
            yield return null;
    }

    public void SetSongInfo(Sprite SongImage,string SongName, string Authur, string illustrator, string LevelDesigner)
    {
        songimage.sprite = SongImage;
        songname.text = SongName;
        authur.text = Authur;
        Illustrator.text = illustrator;
        LD.text = LevelDesigner;
     }
}
