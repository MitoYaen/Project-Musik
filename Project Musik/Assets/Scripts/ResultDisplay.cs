using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ResultDisplay : MonoBehaviour
{
    public Sprite DefaultSprite;
    public Image SongImage;
    public RectTransform Rtext;
    internal RectTransform Self;
    internal float Duration = 1f;
    internal AudioSource ResultAudio;
    public TextMeshProUGUI SongName;
    public TextMeshProUGUI Authur;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Combo;
    public TextMeshProUGUI Perfect;
    public TextMeshProUGUI Great;
    public TextMeshProUGUI Miss;
    // Start is called before the first frame update
    void Awake()
    {
        ResultAudio = gameObject.GetComponent<AudioSource>();
        Self = gameObject.GetComponent<RectTransform>();
        if (SongImage == null || Transition.Instance == null)
        {
            SongImage.sprite = DefaultSprite;
        }
        else
        {
            SongName.text = Transition.Instance.songname.text;
            Authur.text = Transition.Instance.authur.text;
            SongImage.sprite = Transition.Instance.songimage.sprite;
        }
    }
    public void ShowResult(double score, int combo, int perfect, int great, int miss)
    {
        score = (int)score;
        ResultAudio.Play();
        SetText(score, combo, perfect, great, miss);
        MoveWhole();

    }

    void SetText(double score, int combo, int perfect, int great, int miss)
    {
        Score.text = score.ToString();
        Combo.text = combo.ToString();
        Perfect.text = perfect.ToString();
        Great.text = great.ToString();
        Miss.text = miss.ToString();
    }
    void MoveWhole()
    {
        Self.anchoredPosition = new Vector2(2500, 0);
        Self.DOAnchorPosX(0, Duration).SetEase(Ease.OutCubic);
        StartCoroutine(MoveText());
    }

    IEnumerator MoveText()
    {
        Rtext.anchoredPosition = new Vector2(-570, 0);
        yield return new WaitForSeconds(Duration + 0.1f);
        Rtext.DOAnchorPosX(0,Duration).SetEase(Ease.OutCubic);
    }

}
