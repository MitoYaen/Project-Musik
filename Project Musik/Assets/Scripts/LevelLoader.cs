using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using SonicBloom.Koreo;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }
    public int Diff { get; set; } = 0;
    public Koreography song;
    public Sprite BackGround;
    public Color EnchanceCol;

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
    public void SetBackGround(Sprite BG,Color color)
    {
        LevelLoader.Instance.BackGround = BG;
        LevelLoader.Instance.EnchanceCol = color;
    }

    public void LoadMenu()
    {
        Debug.Log("menu");
        Transition.Instance.LeftToMid();
        StartCoroutine(LoadAsync("Menu"));
    }
    public void LoadLevel()
    {
        Transition.Instance.RightToMid();
        StartCoroutine(LoadAsync("InGame"));
    }
    IEnumerator LoadAsync(string SceneName)
    {
        float timer = 0f;
        float MinLoadTime = Transition.Instance.Duration + Transition.Instance.Duration/3;

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);
        operation.allowSceneActivation = false;

        while (timer < MinLoadTime) 
        {
            timer += Time.unscaledDeltaTime;

            if (timer > MinLoadTime)
            {
                ClearMemory();
                operation.allowSceneActivation = true;
                Transition.Instance.StartCoroutine(Transition.Instance.NextStep());
                yield break;
            }

            yield return null;
        }

        yield return null;

    }

    public static void ClearMemory()
    {
        GarbageCollector.CollectIncremental();
        Resources.UnloadUnusedAssets();
    }

}
