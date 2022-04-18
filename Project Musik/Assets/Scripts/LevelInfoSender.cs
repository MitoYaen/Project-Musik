using UnityEngine;
using UnityEngine.UI;
using SonicBloom.Koreo;

public class LevelInfoSender : MonoBehaviour
{
    public string SongName;
    public Koreography Song;
    public int Difficulty;
    public float NoteSpeedScale;
    public Text SongText;
    public Sprite DefaultImage;
    public Sprite SongImage;
    internal string songName;
    public string Authur;
    public string illustrator;
    public string LevelDesigner;
    public Sprite DefaultBackGround;
    public Sprite BackGround;
    public bool ChangeColor;
    public Color EnchanceColor;
    internal Button button;
    LevelLoader lvlLoader;
    Transition Transition;

    private void Start()
    {
        SongText.text = SongName;
        songName = SongName;
        lvlLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        Transition = GameObject.Find("Transition").GetComponent<Transition>();
        button = gameObject.GetComponent<Button>();
        // Null Check
        if (SongImage == null)
        {
            SongImage = DefaultImage;
        }
        if (songName == null || songName == "")
        {
            songName = "The Forgotten Song";
        }
        if (Authur == null || Authur == "")
        {
            Authur = "Slient Composer";
        }
        if (illustrator == null || illustrator == "")
        {
            illustrator = "Painting Guy";
        }
        if (LevelDesigner == null || LevelDesigner == "")
        {
            LevelDesigner = "The Big Brain";
        }
        if (BackGround == null)
        {
            BackGround = DefaultBackGround;
        }
        if (NoteSpeedScale == 0 || NoteSpeedScale == null)
        {
            NoteSpeedScale = 1;
        }

        button.onClick.AddListener(delegate { lvlLoader.SetSong(Song); });
        button.onClick.AddListener(delegate { lvlLoader.SetDiff(Difficulty,NoteSpeedScale); });
        if (ChangeColor)
        {
            button.onClick.AddListener(delegate { lvlLoader.SetBackGround(BackGround, EnchanceColor); });
        }
        else
        {
            button.onClick.AddListener(delegate { lvlLoader.SetBackGround(BackGround, Color.white); });
        }
        button.onClick.AddListener(delegate { Transition.SetSongInfo(SongImage, songName, Authur, illustrator, LevelDesigner); });
        button.onClick.AddListener(delegate { lvlLoader.LoadLevel(); });
    }
    public void SetSongInfo()
    {
        Transition.Instance.SetSongInfo(SongImage,songName, Authur, illustrator, LevelDesigner);
    }
}
