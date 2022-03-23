using DG.Tweening;
using SonicBloom.Koreo;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    RythmGameManager GameController;

    [Tooltip("音轨所对应的键位")]
    public KeyCode keyboardButton;


    [Tooltip("音轨所对应事件编号")]
    public int laneID;

    //the list of all the events in the lane
    List<KoreographyEvent> laneEvents = new List<KoreographyEvent>();

    //the queue of all active note in current lane
    public Queue<Note> trackedNotes = new Queue<Note>();

    //Detect the index of the next-spawn note in the lane
    int pendingEventIdx = 0;

    public Vector3 TargetPosition
    {
        get
        {
            return transform.position;
        }
    }

    //target visual
    public Transform TargetVisual;
    public GameObject PressedVisual;
    public GameObject PressedBigVisual;

    //Border
    public Transform targetTopTrans;
    public Transform targetBottomTrans;

    //Lane Status
    public bool Flicking;    //Check Flick Status
    public bool Holding;    //Check Hold Status (Visual)
    internal bool JudgeHolding; //Check Hold Status (Judgement)
    public bool GoodHolding; // Check if hold and reach Note(Hold)
    //Lane Status

    //Get Last Hold-Start Note
    public Note LastHoldStartNote;
    //Get Last Hold-End Note
    public Note LastHoldEndNote;
    //Get Related Lane;
    public LaneController RelatedLane1;
    public LaneController RelatedLane2;
    //Get Releated BigLane;
    public LaneController RelatedBigLane;
    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init(true, true, LogBehaviour.Default);
    }

    // Update is called once per frame
    void Update()
    {

        if (GameController.GamePause)
        {
            return;
        }
        
        // PressVisual (Null Check)
        if ((PressedVisual != null || PressedBigVisual != null))
        {
            if (PressedVisual != null)
            {
                PressedVisual.SetActive(Holding);
            }
            else
            {
                
            }
        }

        //Clear unused notes
        while (trackedNotes.Count>0&&trackedNotes.Peek().IsNoteMissed())
        {
            //Debug.Log("结果为 Lost, " + "漏键。");
            trackedNotes.Dequeue();
            //Questionable Code
        }
        //Check the Spawning of the new notes
        CheckSpawnNext();


        // KeyBoardInputDetect
        if (Input.GetKeyDown(keyboardButton))
        {
            CheckNoteHit();
            //Debug.Log(keyboardButton + "Pressed");
        }
        else if (Input.GetKey(keyboardButton))
        {
            //Check LongnotesStart
        }
        else if (Input.GetKeyUp(keyboardButton))
        {
            //Check LongNotesEnd
        }

    }

    //Initialize
    public void Initialize(RythmGameManager controller)
    {
        GameController = controller;
    }

    //check the event is match with the named lane
    public bool DoesMatch(int noteID)
    {
        return noteID == laneID;
    }

    //If match, add the event into the lane event list
    public void AddEventToLane(KoreographyEvent evt)
    {
        laneEvents.Add(evt);

    }

    //the offset position of the notes spawn in the lane
    int GetSpawnSampleOffset()
    {
        //Spawn pos and target pos
        float spawnDistToTarget = targetTopTrans.position.z - transform.position.z;

        if (GameController != null)
        {
            //Time of arrival
            float spawnPosToTargetTime = spawnDistToTarget / GameController.NoteSpeed;
            return (int)spawnPosToTargetTime * GameController.SampleRate;
        }
        else
        {
            Debug.Log("no GC.");
            return 0;
        }
    }
    //to check if needed to spawn new note when reach certain event
    void CheckSpawnNext()
    {
        int sampleToTarget = GetSpawnSampleOffset();

        int currentTime = GameController.DelayedSampleTime;
        //int currentTime = GameController.DelayedSampleTime + (int)(GameController.NoteOffset_ms * 0.001f * GameController.SampleRate);

        while (pendingEventIdx < laneEvents.Count
            && laneEvents[pendingEventIdx].StartSample < currentTime + sampleToTarget)
        {
            KoreographyEvent evt = laneEvents[pendingEventIdx];
            int noteNum = evt.GetIntValue();
            int AbsNoteNum = evt.GetIntValue();
            Note newObj = GameController.GetFreshNoteObject(AbsNoteNum, laneID);
            bool isLongNoteStart = false;
            bool isLongNoteEnd = false;
            bool isFlick = false;
            bool isBig = false;
            if (noteNum >16)
            {
                switch (noteNum){
                    //17-18 Big Hold Start
                    case 17:
                        isBig = true;
                        isLongNoteStart = true;
                        LastHoldStartNote = newObj;
                        noteNum = 9;
                        break;
                    case 18:
                        isBig = true;
                        isLongNoteStart = true;
                        LastHoldStartNote = newObj;
                        noteNum = 10;
                        break;
                    //19-20 Big Hold End
                    case 19:
                        isBig = true;
                        isLongNoteEnd = true;
                        newObj.ObjHoldLineFront.SetActive(true); //Spawn the line if the end exists
                        newObj.ObjHoldLineBack.SetActive(true);
                        LastHoldEndNote = newObj;
                        LastHoldStartNote.RelatedEndNote = LastHoldEndNote;
                        newObj.RelatedStartNote = LastHoldStartNote;
                        noteNum = 9;
                        break;
                    case 20:
                        isBig = true;
                        isLongNoteEnd = true;
                        newObj.ObjHoldLineFront.SetActive(true); //Spawn the line if the end exists
                        newObj.ObjHoldLineBack.SetActive(true);
                        LastHoldEndNote = newObj;
                        LastHoldStartNote.RelatedEndNote = LastHoldEndNote;
                        newObj.RelatedStartNote = LastHoldStartNote;
                        noteNum = 10;
                        break;
                    //21-22 Big Flick;
                    case 21:
                        isBig = true;
                        isFlick = true;
                        noteNum = 9;
                        break;
                    case 22:
                        isBig = true;
                        isFlick = true;
                        noteNum = 10;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (isBig == false)
                {
                    if (noteNum > 4 && noteNum <= 8)
                    {
                        //Hold In side(on)? (5-8)
                        Debug.Log("LongNoteStart Spawned");
                        isLongNoteStart = true;
                        LastHoldStartNote = newObj;
                    }
                    else if (noteNum > 8 && noteNum <= 12)
                    {
                        if (LastHoldStartNote != null)
                        {
                            //Hold In side(off)? (9-12)
                            isLongNoteEnd = true;
                            isLongNoteStart = false;
                            Debug.Log("LongNoteEnd Spawned");
                            newObj.ObjHoldLineFront.SetActive(true); //Spawn the line if the end exists
                            newObj.ObjHoldLineBack.SetActive(true);
                            LastHoldEndNote = newObj;
                            LastHoldStartNote.RelatedEndNote = LastHoldEndNote;
                            newObj.RelatedStartNote = LastHoldStartNote;
                            noteNum = noteNum - 4;
                        }

                    }
                    else if (noteNum > 12)
                    {

                         //Flick? (13-16)
                         isLongNoteEnd = false;
                         isFlick = true;
                         noteNum = noteNum - 8;

                    }
                    
                    
                }
            }
            
            newObj.Initialize(evt, noteNum, AbsNoteNum, this, GameController, isLongNoteStart, isLongNoteEnd, isFlick, isBig);
            trackedNotes.Enqueue(newObj);
            pendingEventIdx++;
        }
    }

    //Check If hit the notes
    //If so, it will excute "Hit" and delete
    public void CheckNoteHit()
    {
        if (!GameController.GameStart)
        {
            return;
        }
        if (trackedNotes.Count>0)
        {
            Note noteObject = trackedNotes.Peek();
            //GoodHolding = true;
            if (noteObject.isLongNoteEnd && !GoodHolding)
            {
                noteObject.HoldFailed = true;
                return;
            }
            //Check if the note is further than the detect distance
            if (noteObject.hitOffset > noteObject.targetOffset - (GameController.lostFloat * 0.001f * GameController.SampleRate))
            {
                if (noteObject.isLongNote && Holding)
                {
                    GoodHolding = true; //Successfully detected the Hold input in right time

                }
                if (noteObject.isLongNoteEnd && GoodHolding)
                {
                    GoodHolding = false;
                    JudgeHolding = false;
                }
                if (noteObject.Big)
                {
                    if (PressedBigVisual != null)
                    {
                        PressedBigVisual.SetActive(true);
                    }
                }
                // Cancel the flick after check note
                if (RelatedLane1 != null && RelatedLane2 != null)
                {
                    Flicking = false;
                    RelatedLane1.Flicking = false;
                    RelatedLane2.Flicking = false;
                }
                
                if (RelatedBigLane != null)
                {
                    Flicking = false;
                    RelatedBigLane.Flicking = false;
                }

                trackedNotes.Dequeue();

                int hitLevel = noteObject.IsNoteHittable();
                if (hitLevel == 1)
                {
                    //Far(Early)
                    GameController.Combo++;
                    GameController.FarScoreUpdate();
                    
                }
                if (hitLevel == 2)
                {
                    //Pure
                    GameController.Combo++;
                    GameController.PureScoreUpdate();
                }
                if (hitLevel == 3)
                {
                    //Far(Late)
                    GameController.Combo++;
                    GameController.FarScoreUpdate();
                }
                else
                {
                    //Not Hit
                }
                noteObject.OnHit();
            }
            else
            {
                //Debug.Log("目前偏移量为 " + noteObject.hitOffset + "ms.");
            }
        }
    }
    internal bool DebuglogFlick = true;
    internal bool DebuglogJudgeHold = true;
    public void FlickDetect() 
    {
        
        if (DebuglogFlick)
        {
             Debug.Log("Flick Detected in " + laneID);
            DebuglogFlick = false;
            Flicking = true;
        }
        
    }

    public void FlickDone()
    {
        Debug.Log("Flick Done in" + laneID);
        Flicking = false;
        DebuglogFlick = true;
    }
    public void HoldDetect()
    {
        Debug.Log("Hold Detected in " + laneID);
        Holding = true;
    }
    public void JudgeHoldStart()
    {
        if (DebuglogJudgeHold)
        {
            Debug.Log("Judge Hold Detected in " + laneID);
            JudgeHolding = true;
        }
        
    }
    public void Release()
    {
        Debug.Log("Released in " + laneID);
        FlickDone();
        DebuglogJudgeHold = true;
        JudgeHolding = false;
        Holding = false;
        if (GoodHolding)
        {
            GoodHolding = false;
            CheckNoteHit();
        }
        if (PressedBigVisual != null)
        {
            PressedBigVisual.SetActive(false);
        }
    }


}
