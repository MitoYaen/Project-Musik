using UnityEngine;
using UnityEngine.UI;
using SonicBloom.Koreo.Players; 

public class ProgressBar : MonoBehaviour
{
    public RythmGameManager GameManager;
    public Slider Bar;
    SimpleMusicPlayer Player;
    public int CurrentSample;
    public int FullSample;
    public float Progress;

    private void Start()
    {
        Bar = gameObject.GetComponent<Slider>();
        Player = GameManager.SimpleMusicPlayerTransRef.GetComponent<SimpleMusicPlayer>();
    }
    private void Update()
    {
        if (GameManager.GameStart)
        {
            CurrentSample = GameManager.DelayedSampleTime / 10000;
            string SongName = Player.GetCurrentClipName();
            FullSample = Player.GetTotalSampleTimeForClip(SongName) / 10000;
            Bar.value = Progress = (float)(GameManager.DelayedSampleTime / FullSample) / 100;
            //Bar.value = 0.002f;
        }
    }
}