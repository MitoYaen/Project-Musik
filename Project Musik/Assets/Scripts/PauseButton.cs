using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    Button button;
    public Sprite[] ButtonSprite;
    Image image;
    public bool Clickable;
    public float ClickCD;
    public RythmGameManager rythmGameController;
    public GameObject PauseMenu;

    void Start()
    {
        button = GetComponent<Button>();
        Clickable = true;
        //button.onClick.AddListener(ChangePauseState);
        image = GetComponent<Image>();
    }
    public void ChangePauseState()
    {
        if (Clickable)
        {
            rythmGameController.GamePause = !rythmGameController.GamePause;
            PauseMenu.SetActive(rythmGameController.GamePause);
            if (rythmGameController.GamePause)
            {
                //Pause
                Debug.Log("Game Pause.");
                rythmGameController.PauseMusic();
                image.sprite = ButtonSprite[1];
                StartCoroutine(DelayClickable());
                return;
            }
            else
            {
                //start
                Debug.Log("Game Start.");
                rythmGameController.PlayMusic();
                image.sprite = ButtonSprite[0];

            }
        }
    }

    IEnumerator DelayClickable()
    {
        Clickable = false;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.2f);
        Debug.Log("UnClickable.");
        yield return new WaitForSeconds(ClickCD);
        Debug.Log("Clickable.");
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.7f);
        Clickable = true;
    }

    public void Replay()
    {
        SceneManager.LoadScene("InGame");
    }

    public void ExitSong()
    {
        SceneManager.LoadScene("Menu");
    }
}
