using UnityEngine;
using UnityEngine.UI;
using SonicBloom.Koreo;

public class LevelInfoSender : MonoBehaviour
{
    public string SongName;
    public Koreography Song;
    public int Difficulty;
    public Text SongText;
    public Sprite DefaultImage;
    public Sprite SongImage;
    public string songName;
    public string Authur;
    public string illustrator;
    public string LevelDesigner;
    internal Button button;
    LevelLoader lvlLoader;
    Transition Transition;

    private void Start()
    {
        SongText.text = SongName; 
        lvlLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        Transition = GameObject.Find("Transition").GetComponent<Transition>();
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate{ lvlLoader.SetSong(Song); });
        button.onClick.AddListener(delegate { lvlLoader.SetDiff(Difficulty); });
        button.onClick.AddListener(delegate { Transition.SetSongInfo(SongImage, songName, Authur, illustrator, LevelDesigner); });
        button.onClick.AddListener(delegate { lvlLoader.LoadLevel(); });
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
    }
    public void SetSongInfo()
    {
        Transition.Instance.SetSongInfo(SongImage,songName, Authur, illustrator, LevelDesigner);
    }
}
