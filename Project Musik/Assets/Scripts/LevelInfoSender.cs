using UnityEngine;
using UnityEngine.UI;
using SonicBloom.Koreo;

public class LevelInfoSender : MonoBehaviour
{
    public string SongName;
    public Koreography Song;
    public int Difficulty;
    public Text SongText;
    internal Button button;
    LevelLoader lvlLoader;

    private void Start()
    {
        SongText.text = SongName; 
        lvlLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate{ lvlLoader.SetSong(Song); });
        button.onClick.AddListener(delegate { lvlLoader.SetDiff(Difficulty); });
        button.onClick.AddListener(delegate { lvlLoader.LoadLevel(); });
    }
}
