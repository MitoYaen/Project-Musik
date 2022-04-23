using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
//using Bolt;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RythmGameManager : MonoBehaviour
{
    public bool GameStart = false;
    public bool GamePause = false;
    public bool GameEnd = false;
    internal bool EndOnce = true;

    public Koreography Song;
    [EventID]
    public string EventID;

    [Range(0.5f,8f)]
    [SerializeField]  public float InGameNoteSpeed;
    public float NoteSpeed;
    internal float NSpeedScale;

    [Range(8f,200f)]
    public float HitWindowSize_ms;
    public int Pure;
    public int Far;
    public int Lost;
    public double FullScore;
    public double PerScore;
    public double CurScore;
    public int Combo;
    internal int MaxCombo;
    public int TotalNotes;
    public int pureFloat;
    public int farFloat;
    public int lostFloat;
    public bool AutoPlay = false;
    public ResultDisplay Result;
    public GameObject BoltLinkObject;
    public Slider NoteOffsetSlider;
    public Slider NoteSpeedSlider;

    [Tooltip("如果感觉点击过晚，请增大；如果感觉点击过早，请减少。")]
    [Range(-50,50)]
    [SerializeField] public int NoteOffset_ms;

    public float HitWindowSize_unit
    { 
        get
        {
            return 1/NoteSpeed * (HitWindowSize_ms * 0.01f);
        }
    }
    int HitWindowSize_sample;
    public int HitWindowSampleWidth
    {
        get 
        {
            return HitWindowSize_sample;
        }
    }
    public int SampleRate
    {
        get
        {
            return PlayingKoreo.SampleRate;
        }
    }
    Stack<Note> noteObjectPool = new Stack<Note>();
    Stack<Note> SidenoteObjectPool = new Stack<Note>();
    Stack<Note> BignoteObjectPool = new Stack<Note>();
    public Transform SimpleMusicPlayerTransRef;
    SimpleMusicPlayer simpleMusicPlayer;

    //Invoke
    Koreography PlayingKoreo;
    public List<LaneController> noteLanes = new List<LaneController>();
    public AudioSource AudioCom;
    public ScoreUpdater ScoreUpdate;
    public Image Background;
    public Image EnchancerImg;
    public Animator IndiPerfect;
    public Animator IndiGreat;

    //Prefab
    public Note noteObject;
    public Note noteObjectSide;
    public Note noteObjectBig;

    //Time Related
    [Tooltip("开始播放音频之前提供的时间量，以秒为单位")]
    public float LeadInTime;
    float LeadInTimeLeft;
    float TimeLeftToPlay; //Countdown Before the music actually plays(Which means the events will be called first)

    public int DelayedSampleTime    //Previous SampleTime, including any necessary delay (To koreo)
    {
        get
        {
            return PlayingKoreo.GetLatestSampleTime() - (int)(SampleRate * LeadInTimeLeft);
        }
    }

    void Start()
    {
        GameObject LvlLoader = GameObject.Find("LevelLoader");
        if (LvlLoader != null)
        {
            //Get Param from last scene
            EventID = LvlLoader.GetComponent<LevelLoader>().Diff.ToString();
            Debug.Log("Difficulty " + EventID + "Loaded.");
            Koreography GameSong = LvlLoader.GetComponent<LevelLoader>().song;
            Debug.Log("Song Loaded.");
            NSpeedScale = LvlLoader.GetComponent<LevelLoader>().noteSpeedScale;
            Background.sprite = LvlLoader.GetComponent<LevelLoader>().BackGround;
            EnchancerImg.color = LvlLoader.GetComponent<LevelLoader>().EnchanceCol;
            EnchancerImg.color = new Color(EnchancerImg.color.r, EnchancerImg.color.g, EnchancerImg.color.b, 0.6f);

            simpleMusicPlayer = SimpleMusicPlayerTransRef.GetComponent<SimpleMusicPlayer>();
            simpleMusicPlayer.LoadSong(GameSong, 0, false);
        }
        else
        {
            simpleMusicPlayer = SimpleMusicPlayerTransRef.GetComponent<SimpleMusicPlayer>();
            simpleMusicPlayer.LoadSong(Song, 0, false);
        }
        if (PlayerPrefs.GetFloat("NoteSpeed") != 0)
        {
            NoteSpeedSlider.value = PlayerPrefs.GetFloat("NoteSpeed");
            InGameNoteSpeed = PlayerPrefs.GetFloat("NoteSpeed");
            //Debug.Log(PlayerPrefs.GetFloat("NoteSpeed"));
        }
        else
        {
            NoteSpeedSlider.value = InGameNoteSpeed;  //Inital Sliders
        }
        NoteOffsetSlider.value = NoteOffset_ms;

        InitializeLeadIn();
        for (int i = 0; i < noteLanes.Count; ++i)
        {
            noteLanes[i].Initialize(this);
        }
        PlayingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        KoreographyTrackBase rythmTrack = PlayingKoreo.GetTrackByID(EventID);
        List<KoreographyEvent> rawEvents = rythmTrack.GetAllEvents();        //Get Event

        //Init Score
        TotalNotes = rawEvents.Count;
        ScoreUpdate.Value = (int)CurScore;
        PerScore = FullScore / TotalNotes ;
        //Variables.ActiveScene.Set("PerScore",PerScore);
        for (int i = 0; i < rawEvents.Count; ++i)
        {
            KoreographyEvent evt = rawEvents[i];
            int noteID = evt.GetIntValue();
            bool skip = false;
            for (int j = 0; j < noteLanes.Count; ++j)
            {
                LaneController lane = noteLanes[j];
                if (noteID > 16)
                {
                    switch (noteID)
                    {
                        case 17:
                            noteID = 9; //Big Left Hold Start
                            skip = true;
                            break;
                        case 18:
                            noteID = 10; //Big Right Hold Start
                            skip = true;
                            break;
                        case 19:
                            noteID = 9; //Big Left Hold End
                            skip = true;
                            break;
                        case 20:
                            noteID = 10; //Big Right Hold End
                            skip = true;
                            break;
                        case 21:
                            noteID = 9; //Big Left Flick Start
                            skip = true;
                            break;
                        case 22:
                            noteID = 10; //Big Right Flick Start
                            skip = true;
                            break;
                        case 23:
                            noteID = 5; //SideNote(Tap) - Upper Left
                            skip = true;
                            break;
                        case 24:
                            noteID = 6; //SideNote(Tap) - Downer Left
                            skip = true;
                            break;
                        case 25:
                            noteID = 7; //SideNote(Tap) - Upper Right
                            skip = true;
                            break;
                        case 26:
                            noteID = 8; //SideNote(Tap) - Downer Right
                            skip = true;
                            break;

                        default:
                            break;
                    }
                }
                else if (noteID > 8 && noteID < 17 && !skip)
                {
                    noteID = noteID - 4;
                    if (noteID > 8)
                    {
                        noteID = noteID - 4;
                    }
                }
                if (lane.DoesMatch(noteID))
                {
                    lane.AddEventToLane(evt);
                    break;
                }
            }
        }
        HitWindowSize_sample = (int)(0.001f * HitWindowSize_ms * SampleRate );

        
        
    }

    void Update() {

        if (GamePause)
        {
            return;
        }

        //get sliders
        InGameNoteSpeed = NoteSpeedSlider.value;
        NoteOffset_ms = (int)NoteOffsetSlider.value;

        //Multiply the note speed
        NoteSpeed = InGameNoteSpeed * 15 * NSpeedScale;
        //Update Combos
        if (Combo > MaxCombo)
        {
            MaxCombo = Combo;
        }

        //CountDown
        if (TimeLeftToPlay > 0)
        {
            TimeLeftToPlay -= Time.unscaledDeltaTime;

            if (TimeLeftToPlay <= 0)
            {
                AudioCom.Play();
                TimeLeftToPlay = 0;
                GameStart = true;
            }
        }

        if (LeadInTimeLeft>0)
        {
            LeadInTimeLeft = Mathf.Max(LeadInTimeLeft - Time.unscaledDeltaTime, 0);
        }
        
#if UNITY_EDITOR

        //Cheats to debug
        if (Input.GetKeyDown(KeyCode.K))
        {
            CurScore += 3000;
            Debug.Log("The score adds 3000, now is " + CurScore);
        }
#endif
        if (GameStart)
        {
            if (Pure == TotalNotes)
            {
                CurScore = FullScore;
            }
            if (!simpleMusicPlayer.IsPlaying || GameEnd)
            {
                //Game Ends
                GameEnd = true;
                if (EndOnce)
                {
                    EndOnce = false;
                    simpleMusicPlayer.Pause();
                    Debug.Log("Game End");
                    Result.ShowResult(CurScore, MaxCombo, Pure, Far, Lost);
                }
            }
        }

    }
    // <summary>
    // InitializeLeadTime  
    // </summary>
    void InitializeLeadIn ()
    {
        if (LeadInTime>0)
        {
            LeadInTimeLeft = LeadInTime;
            TimeLeftToPlay = LeadInTime;
        }
        else
        {
            AudioCom.Play();
        }
    }

    //pick up the object from the pool
    public Note GetFreshNoteObject(int NoteIDref, int LaneID)
    {
        Note retObj;

        if (noteObjectPool.Count>0) 
        {
            //Debug.Log("note spawned as" + NoteIDref + ", in Lane" + LaneID);
            retObj = noteObjectPool.Pop();
        }
        
        if (SidenoteObjectPool.Count > 0)
        {
            //Debug.Log("Side note spawned as" + NoteIDref + ", in Lane" + LaneID);
            retObj = SidenoteObjectPool.Pop();
        }
        if (BignoteObjectPool.Count > 0)
        {
            //Debug.Log("Big note spawned as" + NoteIDref + ", in Lane" + LaneID);
            retObj = BignoteObjectPool.Pop();
        }
        else
        {
            //Origin of the resource
            if (NoteIDref <= 4)
            {
                //Debug.Log("side note spawned as" + NoteIDref);
                retObj = Instantiate(noteObject);
            }
            else if (NoteIDref > 4 && NoteIDref <= 16)
            {
                //Debug.Log("side note spawned as" + NoteIDref + ", in Lane" + LaneID);
                retObj = Instantiate(noteObjectSide);
            }
            else if (NoteIDref > 16 && NoteIDref <= 22)
            {
                //Debug.Log("Big NOTE SPAWNED AS" + NoteIDref + ", in Lane" + LaneID);
                retObj = Instantiate(noteObjectBig);
            }
            else
            {
                //Debug.Log("side note spawned as" + NoteIDref + ", in Lane" + LaneID);
                retObj = Instantiate(noteObjectSide);
            }

        }

        retObj.gameObject.SetActive(true);
        retObj.enabled = true;

        return retObj;
    }


    //Put back the object to the pool

    public void ReturnNoteObjectToPool(Note obj)
    {
        if (obj!=null)
        {
            obj.enabled = false;
            obj.gameObject.SetActive(false);
            //noteObjectPool.Push(obj);
            /*if (obj.Type > 1 && obj.Type < 5 || obj.Type == 8)
            {
                SidenoteObjectPool.Push(obj);
            }
            if (obj.Type >= 5 && obj.Type <= 8)
            {
                BignoteObjectPool.Push(obj);
            }
            else
            {
                noteObjectPool.Push(obj);
            }*/
        }
    }
    //Score Update (Pure)
    public void PureScoreUpdate()
    {
        CurScore += PerScore;
        ScoreUpdate.Value = (int)CurScore;
        //Add Perfect Shown
        IndiPerfect.SetTrigger("ShowNote");
    }

    //Score Update (Far)
    public void FarScoreUpdate()
    {
        CurScore += PerScore/2;
        ScoreUpdate.Value = (int)CurScore;
        //Add Far Shown
        IndiGreat.SetTrigger("ShowNote");
    }

    //Reload Scene
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Auto Plays
    public void SwitchAP()
    {
        AutoPlay = !AutoPlay;
        if (!AutoPlay)
        {
            Debug.Log("AP Disable !");
            foreach (LaneController item in noteLanes)
            {
                item.Release();
            }
        }
        else
        {
            Debug.Log("AP Enable!");
        }
    }

    public void PlayMusic()
    {
        if (!GameStart)
        {
            return;
        }
        simpleMusicPlayer.Play();
    }

    public void PauseMusic()
    {
        if (!GameStart)
        {
            return;
        }
        simpleMusicPlayer.Pause();
    }
}
