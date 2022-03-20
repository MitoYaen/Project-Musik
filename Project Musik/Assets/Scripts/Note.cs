using UnityEngine;
using SonicBloom.Koreo;
using UnityEngine.UI;

public class Note : MonoBehaviour
{

    public Material[] visuals;

    public MeshRenderer visuals_Note;

    KoreographyEvent trackedEvent;

    public int Type = 1;
    /*==========
     * Type : 
     * 1 - TapNote
     * 2 - LongNote(Start)
     * 3 - LongNote(End)
     * 4 - Flick
     * 5 - BigLongNote(Start)
     * 6 - BigLongNote(End)
     * 7 - BigFlick
     ==============*/

    public bool isLongNote;

    public bool isLongNoteEnd;

    public bool SideNote;

    public bool Flick;

    public bool Big;

    LaneController laneController;

    RythmGameManager gameController;

    public ParticleSystem PS_Pure;

    public ParticleSystem PS_Far;

    public int hitOffset;

    public AudioClip HitSound;

    public AudioSource SfxManager;

    public int targetOffset;

    public Text DebugText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePosition();
        GetHitOffset();
        while (laneController.trackedNotes.Peek().isLongNoteEnd)
        {
            
        }

        if (transform.position.z <=laneController.targetBottomTrans.position.z)
        {
            gameController.ReturnNoteObjectToPool(this);
            ResetNote();
            return;
        }
        //Needed to ask about null reference
        if(gameController.AutoPlay && hitOffset >= targetOffset)
        {
            laneController.CheckNoteHit();
            ResetNote();
        }
        else
        {

        }

        
    }

     //Initialize
    public void Initialize(KoreographyEvent evt, int noteNum, int AbsNoteNum, LaneController laneCont,
        RythmGameManager gameCont, bool isLongStart,bool isLongEnd,bool isFlick, bool isBig)
    {
        trackedEvent = evt;
        laneController = laneCont;
        gameController = gameCont;
        isLongNote = isLongStart;
        isLongNoteEnd = isLongEnd;
        Flick = isFlick;
        DebugText.text = AbsNoteNum.ToString() + "," + laneController.laneID.ToString() ;
        Big = isBig;
        int matNum = 0;
        // Initialize input or parametres ↓↓↓↓↓
        if (isLongNote)
        {
            matNum = 1;
            Type = 2;
        }
        else if (isLongNoteEnd)
        {
            matNum = 1;
            Type = 3;
        }
        else if (Flick)
        {
            matNum = 1;
            Type = 4;
        }
        else if (isLongNote && Big)
        {
            Type = 5;
        }
        else if (isLongNoteEnd && Big)
        {
            Type = 6;
        }
        else if (Flick && Big)
        {
            Type = 7;
        }
        //Don't forget to change ↑↑↑↑↑
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

    void UpdatePosition()
    {
        if (laneController != null)
        {
            Vector3 pos = laneController.TargetPosition;
            pos.z -= (gameController.DelayedSampleTime - (trackedEvent.StartSample + gameController.NoteOffset_ms * 0.01f * gameController.SampleRate))
            / (float)gameController.SampleRate * gameController.NoteSpeed;
            transform.position = pos;
        }
        else
        {
           
        }



    }

    void GetHitOffset()
    {
        if (gameController != null)
        {
            int curTime = gameController.DelayedSampleTime;
            int noteTime = trackedEvent.StartSample + (int)(gameController.NoteOffset_ms * 0.001f * gameController.SampleRate);
            //int noteTime = trackedEvent.StartSample + gameController.SampleRate;
            int hitWindow = gameController.HitWindowSampleWidth;
            hitOffset = hitWindow - Mathf.Abs(noteTime - curTime);
        }
        else
        {
            Debug.Log("Bug");
        }

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
        //Branch When Flick
        if (Flick)
        {
            if (
            !(targetOffset - (gameController.lostFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset
            <= targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate)) 
            &&
            !(targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset
            <= targetOffset + (gameController.lostFloat * 0.001f * gameController.SampleRate)))
            {
                hitLevel = 2;
                Debug.Log("结果为 Flick-Pure, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
                SfxManager.PlayOneShot(HitSound);
                visuals_Note.material = visuals[2];
                GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("结果为 Lost, " + "误差为" + Mathf.RoundToInt(targetOffset - hitOffset) / 44.4 + "ms.");
                this.enabled = false;
            }
        }
        // Detect offset when input
        if 
            (targetOffset - (gameController.lostFloat * 0.001f * gameController.SampleRate) <= hitOffset&&hitOffset 
            <= targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 0;
            Debug.Log("结果为 Lost, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            this.enabled = false;
        }
        if 
            (targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset 
            <= targetOffset - (gameController.pureFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 1;
            Debug.Log("结果为 FarEarly, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Far,gameObject.transform.position,Quaternion.identity);
        }
        if 
            (targetOffset - (gameController.pureFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset 
            <= targetOffset + (gameController.pureFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 2;
            Debug.Log("结果为 Pure, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
        }
        if 
            (targetOffset + (gameController.pureFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset 
            <= targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 3;
            Debug.Log("结果为 FarLate, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Far, gameObject.transform.position, Quaternion.identity);
        }
        if 
            (targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset 
            <= targetOffset + (gameController.lostFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 0;
            Debug.Log("结果为 Lost, " + "误差为" + Mathf.RoundToInt(targetOffset - hitOffset) / 44.4 + "ms.");
            this.enabled = false;
        }

        return hitLevel;
    }
}
