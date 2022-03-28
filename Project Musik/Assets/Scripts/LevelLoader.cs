using UnityEngine;
using UnityEngine.SceneManagement;
using SonicBloom.Koreo;
using System.Collections;

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

    public void LoadMenu()
    {
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
        float MinLoadTime = 0.5f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);
        operation.allowSceneActivation = false;

        while (timer < MinLoadTime) 
        {
            timer += Time.unscaledDeltaTime;

            if (timer > MinLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        yield return null;

    }

}
