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

    public bool HoldFailed = false;

    public bool SideNote;

    public bool Flick;

    public bool Big;

    public Note RelatedStartNote;

    public Note RelatedEndNote;

    LaneController laneController;

    RythmGameManager gameController;

    public ParticleSystem PS_Pure;

    public ParticleSystem PS_Far;

    public GameObject ObjHoldLineFront;

    public LineRenderer HoldLineFront;

    public GameObject ObjHoldLineBack;

    public LineRenderer HoldLineBack;

    public int hitOffset;

    public AudioClip HitSound;

    public AudioSource SfxManager;

    public int targetOffset;

    public Text DebugText;


    void Start()
    {
        //HoldLineFront = ObjHoldLineFront.GetComponent<LineRenderer>();
        //HoldLineBack = ObjHoldLineBack.GetComponent<LineRenderer>();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameController !=null)
        {
            UpdatePosition();
            GetHitOffset();
            UpdateLinePosition();

            //AutoPlay Options↓
            if (gameController.AutoPlay && hitOffset >= targetOffset)
            {
                laneController.CheckNoteHit();
                //ResetNote();
                return;
            }
            else
            {

            }
            //AutoPlay Options ↑

            if (laneController.Flicking && hitOffset >= targetOffset)
            {
                laneController.CheckNoteHit();
                //ResetNote();
                return;
            }
            else
            {

            }


            if (laneController.GoodHolding && isLongNoteEnd && hitOffset >= targetOffset)
            {
                laneController.CheckNoteHit();
                //ResetNote();
                return;
            }

            if (transform.position.z <= laneController.targetBottomTrans.position.z)
            {
                gameController.ReturnNoteObjectToPool(this);
                ResetNote();
                return;

            }
        }
        else
        {
            Debug.Log("Error, Game Controller cannot be null.");
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
        /*if (RelatedEndNote != null)
        {
            Debug.Log("Delayed the entrance!");
            if (RelatedEndNote.hitOffset >= targetOffset)
            //Case when hit the start note,delay it's entrance into pool until the related end note reaches the target.
            {
                trackedEvent = null;
                laneController = null;
                gameController = null;
            }
        }
        else 
        {

        }*/
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
            int curTime = gameController.DelayedSampleTime;
            int noteTime = trackedEvent.StartSample + (int)(gameController.NoteOffset_ms * 0.001f * gameController.SampleRate);
            //int noteTime = trackedEvent.StartSample + gameController.SampleRate;
            int hitWindow = gameController.HitWindowSampleWidth;
            hitOffset = hitWindow - Mathf.Abs(noteTime - curTime);

    }

    public void UpdateLinePosition()
    {
        if (RelatedStartNote != null && isLongNoteEnd)
        {
            Vector3 LastHoldStartNotePos = RelatedStartNote.transform.position;
            //Debug.Log("RelatedStartNote found.");
            if (!HoldFailed)
            {
                HoldLineFront.SetPosition(0, transform.position);
                HoldLineFront.SetPosition(1, LastHoldStartNotePos);
                HoldLineBack.SetPosition(0, transform.position);
                HoldLineBack.SetPosition(1, LastHoldStartNotePos);
            }
            else
            {
                Vector3 Losepos = new Vector3(transform.position.x, transform.position.y, -20);
                HoldLineFront.SetPosition(0, transform.position);
                HoldLineFront.SetPosition(1, Losepos);
                HoldLineBack.SetPosition(0, transform.position);
                HoldLineBack.SetPosition(1, Losepos);
            }

        }
        else
        {
          
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
                //Debug.Log("结果为 Flick-Pure, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
                SfxManager.PlayOneShot(HitSound);
                visuals_Note.material = visuals[2];
                GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
            }
            else
            {
                //Debug.Log("结果为 Lost, " + "误差为" + Mathf.RoundToInt(targetOffset - hitOffset) / 44.4 + "ms.");
                this.enabled = false;
            }
        }
        // Detect offset when input
        if 
            (targetOffset - (gameController.lostFloat * 0.001f * gameController.SampleRate) <= hitOffset&&hitOffset 
            <= targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 0;
            //Debug.Log("结果为 Lost, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            this.enabled = false;
        }
        if 
            (targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset 
            <= targetOffset - (gameController.pureFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 1;
            //Debug.Log("结果为 FarEarly, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Far,gameObject.transform.position,Quaternion.identity);
        }
        if 
            (targetOffset - (gameController.pureFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset 
            <= targetOffset + (gameController.pureFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 2;
            //Debug.Log("结果为 Pure, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
        }
        if 
            (targetOffset + (gameController.pureFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset 
            <= targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 3;
            //Debug.Log("结果为 FarLate, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            SfxManager.PlayOneShot(HitSound);
            visuals_Note.material = visuals[2];
            GameObject.Instantiate(PS_Far, gameObject.transform.position, Quaternion.identity);
        }
        if 
            (targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset 
            <= targetOffset + (gameController.lostFloat * 0.001f * gameController.SampleRate))
        {
            hitLevel = 0;
            //Debug.Log("结果为 Lost, " + "误差为" + Mathf.RoundToInt(targetOffset - hitOffset) / 44.4 + "ms.");
            this.enabled = false;
        }

        return hitLevel;
    }
}
