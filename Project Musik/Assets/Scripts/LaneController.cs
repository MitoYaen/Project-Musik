using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using DG.Tweening;

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

    //Border
    public Transform targetTopTrans;
    public Transform targetBottomTrans;

    //Lane Status
    public bool Flicking;    //Check Flick Status
    public bool Holding;    //Check Hold Status
    //Lane Status

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init(true, true, LogBehaviour.Default);
    }

    // Update is called once per frame
    void Update()
    {
        //Clear unused notes
        while (trackedNotes.Count>0&&trackedNotes.Peek().IsNoteMissed())
        {
            //Debug.Log("结果为 Lost, " + "漏键。");
            trackedNotes.Dequeue();
            //Questionable Code
        }


        //Check the Spawning of the new notes
        CheckSpawnNext();
        //
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
                        noteNum = 9;
                        break;
                    case 18:
                        isBig = true;
                        isLongNoteStart = true;
                        noteNum = 10;
                        break;
                    //19-20 Big Hold End
                    case 19:
                        isBig = true;
                        isLongNoteEnd = true;
                        noteNum = 9;
                        break;
                    case 20:
                        isBig = true;
                        isLongNoteEnd = true;
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
                    if (noteNum > 4)
                    {
                        //Hold In side(on)?
                        isLongNoteStart = true;

                        if (noteNum > 8)
                        {
                            //Hold In side(off)?
                            isLongNoteEnd = true;
                            isLongNoteStart = false;
                            noteNum = noteNum - 4;
                            if (noteNum > 8)
                            {
                                isLongNoteEnd = false;
                                isFlick = true;
                                noteNum = noteNum - 4;
                            }
                        }
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
        if (trackedNotes.Count>0)
        {
            Note noteObject = trackedNotes.Peek();
            //Check if the note is further than the detect distance
            if (noteObject.hitOffset > noteObject.targetOffset - (GameController.lostFloat * 0.001f * GameController.SampleRate))
            {
                trackedNotes.Dequeue(); 
                int hitLevel = noteObject.IsNoteHittable();
                if (hitLevel == 1)
                {
                    //Far(Early)
                    GameController.FarScoreUpdate();
                    
                }
                if (hitLevel == 2)
                {
                    //Pure
                    GameController.PureScoreUpdate();
                }
                if (hitLevel == 3)
                {
                    //Far(Late)
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
                Debug.Log("目前偏移量为 " + noteObject.hitOffset + "ms.");
            }

        }
    }

    public void FlickDetect() 
    {
        Debug.Log("Flick Detected in " + laneID);
        Flicking = true;
    }

    public void FlickDone()
    {
        Debug.Log("Flick Done in" + laneID);
        Flicking = false;
    }
    public void HoldDetect()
    {
        Debug.Log("Hold Detected in " + laneID);
        Holding = true;
    }
    public void Release()
    {
        Debug.Log("Released in " + laneID);
        Holding = false;
    }


}
