using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using Bolt;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RythmGameManager : MonoBehaviour
{
    public bool GameStart = false;
    public bool GamePause = false;

    public Koreography Song;
    [EventID]
    public string EventID;

    [Range(0.5f,8f)]
    [SerializeField]  public float InGameNoteSpeed;
    public float NoteSpeed;

    [Range(8f,200f)]
    public float HitWindowSize_ms;
    public int FullScore;
    public float PerScore;
    public float CurScore;
    public int Combo;
    public int TotalNotes;
    public int pureFloat;
    public int farFloat;
    public int lostFloat;
    public bool AutoPlay = false;
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

    //Quote
    Koreography PlayingKoreo;
    public List<LaneController> noteLanes = new List<LaneController>();
    public AudioSource AudioCom;

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
            simpleMusicPlayer = SimpleMusicPlayerTransRef.GetComponent<SimpleMusicPlayer>();
            simpleMusicPlayer.LoadSong(GameSong, 0, false);
        }
        else
        {
            simpleMusicPlayer = SimpleMusicPlayerTransRef.GetComponent<SimpleMusicPlayer>();
            simpleMusicPlayer.LoadSong(Song, 0, false);
        }
        //EventID = 
        NoteSpeedSlider.value = InGameNoteSpeed;  //Inital Sliders
        NoteOffsetSlider.value = NoteOffset_ms;

        InitializeLeadIn();
        for (int i = 0; i < noteLanes.Count; ++i)
        {
            noteLanes[i].Initialize(this);
        }
        PlayingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        KoreographyTrackBase rythmTrack = PlayingKoreo.GetTrackByID(EventID);
        //Get Event

        List<KoreographyEvent> rawEvents = rythmTrack.GetAllEvents();

        TotalNotes = rawEvents.Count;
        PerScore = FullScore / TotalNotes ;
        Variables.ActiveScene.Set("PerScore",PerScore);
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

        NoteSpeed = InGameNoteSpeed * 10;

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
        //
        if (LeadInTimeLeft>0)
        {
            LeadInTimeLeft = Mathf.Max(LeadInTimeLeft - Time.unscaledDeltaTime, 0);
        }
        
        //Update the scores to Bolt
        Variables.Object(BoltLinkObject).Set("Score_Ori", CurScore);
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
            if (!simpleMusicPlayer.IsPlaying)
            {
                //Game Ends
            }
        }

    }
    /// <summary>
    /// InitializeLeadTime  
    /// </summary>
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
        //Questionable Code
        
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
        //Questionable Code 
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
            else
            {
                //Debug.Log("Big NOTE SPAWNED AS" + NoteIDref + ", in Lane" + LaneID);
                retObj = Instantiate(noteObjectBig);
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
            noteObjectPool.Push(obj);
            /*if (obj.Type > 1 && obj.Type < 5 )
            {
                SidenoteObjectPool.Push(obj);
            }
            if (obj.Type >= 5)
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
    }

    //Score Update (Far)
    public void FarScoreUpdate()
    {
        CurScore += PerScore/2;
    }

    //Reload Scene
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Auto Plays
    public void EAP()
    {
        AutoPlay = true;
        Debug.Log("AP Enabled.");
    }

    public void DAP()
    {
        AutoPlay = false;
        Debug.Log("AP Disabled.");
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
