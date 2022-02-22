using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using Bolt;

public class LaneController : MonoBehaviour
{
    RythmGameManager GameController;

    [Tooltip("音轨所对应的键位")]
    public KeyCode keyboardButton;


    [Tooltip("音轨所对应事件编号")]
    public int laneID;

    public bool OnLane;

    public string TouchTag;

    public int curLane;

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

    }

    // Update is called once per frame
    void Update()
    {
        //Clear unused notes
        while (trackedNotes.Count>0&&trackedNotes.Peek().IsNoteMissed())
        {
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
            if (noteObject.hitOffset>-7000)
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

            }

        }
    }


    //Check Lanes when touch
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TouchTag))
        {
            OnLane = true;
            curLane = laneID;
        }
        else
        {
            curLane = 0;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        curLane = 0;
    }


}
