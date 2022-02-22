using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using Bolt;

public class Note : MonoBehaviour
{

    public Material[] visuals;

    public MeshRenderer visuals_Note;

    KoreographyEvent trackedEvent;

    public bool isLongNote;

    public bool isLongNoteEnd;

    LaneController laneController;

    RythmGameManager gameController;

    public int hitOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        GetHitOffset();
    }

    //Initialize
    public void Initialize(KoreographyEvent evt, int noteNum, LaneController laneCont,
        RythmGameManager gameCont, bool isLongStart,bool isLongEnd)
    {
        trackedEvent = evt;
        laneController = laneCont;
        gameController = gameCont;
        isLongNote = isLongStart;
        isLongNoteEnd = isLongEnd;
        int matNum = 0;
        if (isLongNote)
        {
            matNum = 1;
        }
        else if (isLongNoteEnd)
        {
            matNum = 1;
        }
        visuals_Note.material = visuals[matNum];
    }

    //reset note object 
    private void ResetNote()
    {
        trackedEvent = null;
        laneController = null;
        gameController = null;
    }

    void ReturnToPool()
    {
        gameController.ReturnNoteObjectToPool(this);
        ResetNote();
    }
    public void OnHit()
    {
        CustomEvent.Trigger(gameObject, "Hit");
        ReturnToPool();
        
    }

    //
    void UpdatePosition()
    {
        Vector3 pos = laneController.TargetPosition;

        pos.z -= (gameController.DelayedSampleTime - trackedEvent.StartSample) / (float)gameController.SampleRate * gameController.NoteSpeed;

        transform.position = pos;

    }

    void GetHitOffset()
    {
        int curTime = gameController.DelayedSampleTime;
        int noteTime = trackedEvent.StartSample;
        int hitWindow = gameController.HitWindowSampleWidth;
        hitOffset = hitWindow - Mathf.Abs(noteTime - curTime);
    }
    
    public int IsNoteHittable()
    {
        int hitLevel = 0;
        visuals_Note.material = visuals[3];
        if (-7000<=hitOffset&&hitOffset<=-4411)
        {
            hitLevel = 0;
            this.enabled = false;
            CustomEvent.Trigger(gameObject, "Lost");
            Debug.Log("Lost");
        }
        if (-4410 <= hitOffset && hitOffset <= -2206)
        {
            hitLevel = 1;
            CustomEvent.Trigger(gameObject, "FarEarly");
            Debug.Log("FarEarly");
        }
        if (-2205<= hitOffset && hitOffset <= 2205)
        {
            hitLevel = 2;
            CustomEvent.Trigger(gameObject, "Pure");
            Debug.Log("Pure");
        }
        if (2205<=hitOffset && hitOffset <= 4410)
        {
            hitLevel = 3;
            CustomEvent.Trigger(gameObject, "FarLate");
            Debug.Log("FarLate");
        }
        if (4411<=hitOffset && hitOffset <= 7000)
        {
            hitLevel = 0;
            CustomEvent.Trigger(gameObject, "Lost");
            Debug.Log("Lost");
            this.enabled = false;
        }

        return hitLevel;
    }
}
