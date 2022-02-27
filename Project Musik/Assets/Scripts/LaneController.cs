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

    public Transform VisualLane;

    public GameObject CamCatcher;

    //the list of all the events in the lane
    List<KoreographyEvent> laneEvents = new List<KoreographyEvent>();

    //the queue of all active note in current lane
    Queue<Note> trackedNotes = new Queue<Note>();

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

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init(true, true, LogBehaviour.Default);
        CamCatcher = GameObject.FindGameObjectWithTag("CamCatcher");
    }

    // Update is called once per frame
    void Update()
    {
        //Clear unused notes
        while (trackedNotes.Count>0&&trackedNotes.Peek().IsNoteMissed())
        {
            Debug.Log("结果为 Lost, " + "漏键。");
            trackedNotes.Dequeue();

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

        //Time of arrival
        float spawnPosToTargetTime = spawnDistToTarget / GameController.NoteSpeed;

        return (int)spawnPosToTargetTime * GameController.SampleRate;
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
            Note newObj = GameController.GetFreshNoteObject();
            bool isLongNoteStart = false;
            bool isLongNoteEnd = false;
            if (noteNum > 4)
            {
                isLongNoteStart = true;
                noteNum = noteNum - 4;
                if (noteNum > 4)
                {
                    isLongNoteEnd = true;
                    isLongNoteStart = false;
                    noteNum = noteNum - 4;
                }
            }
            newObj.Initialize(evt, noteNum, this, GameController, isLongNoteStart, isLongNoteEnd);
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
                //Debug.Log("目前偏移量为 " + noteObject.hitOffset + "ms.");
            }

        }
    }

    //Visual FeedBack When Pressed
    public void VisualPressLane()
    {
        VisualLane.DOMoveY(-10.65f, 0.2f, false);
        CamCatcher.transform.DOMoveY(-5.6f, 0.2f, false);
    }

    //Visual FeedBack When Released
    public void VisualReleaseLane()
    {
        VisualLane.DOMoveY(-10.5f, 0.2f, false);
        CamCatcher.transform.DOMoveY(-5.2f, 0.2f, false);
    }


}
