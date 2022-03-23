using UnityEngine;
using UnityEngine.SceneManagement;
using SonicBloom.Koreo;
public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }
    public int Diff { get; set; } = 0;
    public Koreography song;

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

    public void SetSong(Koreography Song)
    {
        LevelLoader.Instance.song = Song;
    }
    public void SetDiff(int Difficulty)
    {
        LevelLoader.Instance.Diff = Difficulty;
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene("InGame");
    }
}
