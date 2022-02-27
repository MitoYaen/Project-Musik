using UnityEngine;
using SonicBloom.Koreo;

public class Note : MonoBehaviour
{

    public Material[] visuals;

    public MeshRenderer visuals_Note;

    KoreographyEvent trackedEvent;

    public bool isLongNote;

    public bool isLongNoteEnd;

    LaneController laneController;

    RythmGameManager gameController;

    public ParticleSystem PS_Pure;

    public ParticleSystem PS_Far;

    public int hitOffset;

    public AudioClip HitSound;

    public AudioSource SfxManager;

    public int targetOffset;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        GetHitOffset();
        if (transform.position.z <=laneController.targetBottomTrans.position.z)
        {

            gameController.ReturnNoteObjectToPool(this);
            ResetNote();
        }

        //Needed to ask about null reference
        if (gameController.AutoPlay && hitOffset > targetOffset)
        {
            laneController.CheckNoteHit();
            ResetNote();
        }
        else
        {
            
        }

        
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
        visuals_Note.material = visuals[6];
        ReturnToPool();

    }

    //
    void UpdatePosition()
    {
        Vector3 pos = laneController.TargetPosition;

        pos.z -= (gameController.DelayedSampleTime - (trackedEvent.StartSample + gameController.NoteOffset_ms * 0.01f * gameController.SampleRate)) / (float)gameController.SampleRate * gameController.NoteSpeed;

        transform.position = pos;

    }

    void GetHitOffset()
    {
        int curTime = gameController.DelayedSampleTime;
        int noteTime = trackedEvent.StartSample + (int)(gameController.NoteOffset_ms * 0.001f * gameController.SampleRate);
        int hitWindow = gameController.HitWindowSampleWidth;
        hitOffset = hitWindow - Mathf.Abs(noteTime - curTime);
    }

    //Check if the note is missed
    public bool IsNoteMissed()
    {
        bool bMissed = true;
        if (enabled)
        {
            int curTime = gameController.DelayedSampleTime;
            int noteTime = trackedEvent.StartSample;
            int hitWindow = gameController.HitWindowSampleWidth;

            bMissed = curTime - noteTime > hitWindow;

        }
        return bMissed;
    }

    public int IsNoteHittable()
    {
        int hitLevel = 0;
        visuals_Note.material = visuals[3];
        if (targetOffset - (gameController.lostFloat * 0.001f * gameController.SampleRate) <= hitOffset&&hitOffset<= targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 0;
            Debug.Log("结果为 Lost, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            this.enabled = false;
        }
        if (targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset <= targetOffset - (gameController.pureFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 1;
            Debug.Log("结果为 FarEarly, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Far,gameObject.transform.position,Quaternion.identity);
        }
        if (targetOffset - (gameController.pureFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset <= targetOffset + (gameController.pureFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 2;
            Debug.Log("结果为 Pure, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
        }
        if (targetOffset + (gameController.pureFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset <= targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 3;
            Debug.Log("结果为 FarLate, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Far, gameObject.transform.position, Quaternion.identity);
        }
        if (targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset <= targetOffset + (gameController.lostFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 0;
            Debug.Log("结果为 Lost, " + "误差为" + Mathf.RoundToInt(targetOffset - hitOffset) / 44.4 + "ms.");
            this.enabled = false;
        }

        return hitLevel;
    }
}
