using UnityEngine;
using UnityEngine.UI;
using SonicBloom.Koreo;

public class LevelInfoSender : MonoBehaviour
{
    public Koreography Song;
    public int Difficulty;
    internal Button button;
    LevelLoader lvlLoader;

    private void Start()
    {
        lvlLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate{ lvlLoader.SetSong(Song); });
        button.onClick.AddListener(delegate { lvlLoader.SetDiff(Difficulty); });
        button.onClick.AddListener(delegate { lvlLoader.LoadLevel(); });
    }
}
