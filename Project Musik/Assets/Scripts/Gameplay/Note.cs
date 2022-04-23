using UnityEngine;
using System.Collections.Generic;
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
     * 8 - SideTap
     ==============*/

    public bool isLongNote = false;

    public bool isLongNoteEnd;

    public bool HoldFailed = false;

    public bool SideNote;

    public bool Flick;

    public bool FlickGreat = true;

    public bool Big;

    private bool F_LineKeep = false;

    public Note RelatedStartNote;

    public Note RelatedEndNote;

    LaneController laneController;

    RythmGameManager gameController;

    protected ParticleSystem LongHoldEffect;

    public GameObject ObjHoldLineFront;

    public LineRenderer HoldLineFront;

    public GameObject ObjHoldLineBack;

    public LineRenderer HoldLineBack;

    public int hitOffset;

    public AudioClip HitSound;

    public AudioSource SfxManager;

    public int targetOffset;

    public Text DebugText;

    [Header("Effects")]
    //[SerializeField] private ParticleSystem PS_Pure;
    [SerializeField] private List<ParticleSystem> PS_Pure = new List<ParticleSystem>();

    //[SerializeField] private ParticleSystem PS_Far;
    [SerializeField] private List<ParticleSystem> PS_Far = new List<ParticleSystem>();

    [SerializeField] private ParticleSystem HoldEffect;

    [SerializeField] private Animator IndiLost;

    void Start()
    {
        //HoldLineFront = ObjHoldLineFront.GetComponent<LineRenderer>();
        //HoldLineBack = ObjHoldLineBack.GetComponent<LineRenderer>();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameController.GamePause)
        {
            return;
        }
        if (gameController != null || laneController != null)
        {
            if (!F_LineKeep)
            {
                UpdatePosition();
            }
            GetHitOffset();
            UpdateLinePosition();

            //AutoPlay Options↓
            if (gameController.AutoPlay && hitOffset >= targetOffset)
            {
                laneController.CheckNoteHit();
                //ResetNote();
                return;
            }
            //AutoPlay Options ↑

            if (Flick && hitOffset >= targetOffset)
            {
                if (laneController.Flicking)
                {
                    Debug.Log("Flick ok");
                    FlickGreat = true;
                    laneController.CheckNoteHit();
                    return;
                }
                else
                {
                    Debug.Log("Flick Failed.");
                    FlickGreat = false;
                }
            }


            if (laneController.GoodHolding && isLongNoteEnd && hitOffset >= targetOffset)
            {
                laneController.CheckNoteHit();
                //ResetNote();
                return;

            }



            if (transform.position.z <= laneController.targetBottomTrans.position.z)
            {
                //gameController.ReturnNoteObjectToPool(this);
                gameController.Combo = 0;
                ReturnToPool();
                SpawnEffects(2);
                ResetNote();
                return;

            }
        }
        else
        {
            Debug.LogError("Error, Game or Lane Controller cannot be null.");
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
        else if (SideNote)
        {
            matNum = 1;
            Type = 8;
        }
        //Don't forget to change ↑↑↑↑↑
        visuals_Note.material = visuals[matNum];
    }

    //reset note object 
    private void ResetNote()
    {
        if (isLongNote)
        {
            return;
        }
        trackedEvent = null;
        laneController = null;
        gameController = null;
        if (RelatedStartNote != null && isLongNoteEnd)
        {
            Destroy(RelatedStartNote.gameObject);
        }
        Destroy(gameObject);

    }

    void ReturnToPool()
    {
        if (isLongNote)
        {
            F_LineKeep = true;
        }
        //gameController.ReturnNoteObjectToPool(this);
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
        if (RelatedEndNote != null && isLongNote)
        {
            //Deactivate the render after the EndNote is spawned(StartNote)
            ObjHoldLineFront.SetActive(false);
            ObjHoldLineBack.SetActive(false);
            return;
        }
        if (RelatedStartNote == null && RelatedEndNote == null && isLongNote)
        {
            //Pre-Render the Line Before the EndNote is spawned(StartNote)
            ObjHoldLineFront.SetActive(true);
            ObjHoldLineBack.SetActive(true);
            Vector3 WaitPos = new Vector3(transform.position.x, transform.position.y, 130);
            HoldLineFront.SetPosition(0, transform.position);
            HoldLineFront.SetPosition(1, WaitPos);
            HoldLineBack.SetPosition(0, transform.position);
            HoldLineBack.SetPosition(1, WaitPos);
        }
        if (isLongNote && LongHoldEffect != null && HoldFailed) //Stop the hold effect if hold failed
        {
            LongHoldEffect.Stop();
        }
        if (RelatedStartNote != null && isLongNoteEnd)
        {
            //Render the line after found the StartNote(EndNote)
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
                //Render forward if lost
                Vector3 Losepos = new Vector3(transform.position.x, transform.position.y, -20);
                HoldLineFront.SetPosition(0, transform.position);
                HoldLineFront.SetPosition(1, Losepos);
                HoldLineBack.SetPosition(0, transform.position);
                HoldLineBack.SetPosition(1, Losepos);
                if (isLongNoteEnd && RelatedStartNote.LongHoldEffect != null)
                {
                    RelatedStartNote.LongHoldEffect.Stop();
                }
            }

        }
        else
        {
          
        }
        
    }

        

    //Check if the note is missed (returns a bool)
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
        if (gameController.AutoPlay)
        {
            hitLevel = 2;
            gameController.Pure++;
            SfxManager.PlayOneShot(HitSound);
            Debug.Log("结果为 Auto-Pure, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
            if (isLongNote && HoldEffect != null)
            {
                
                LongHoldEffect = GameObject.Instantiate(HoldEffect, gameObject.transform.position, Quaternion.identity);
                LongHoldEffect.Play();
            }
            else if (isLongNoteEnd && RelatedStartNote.LongHoldEffect != null)
            {
                //GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
                SpawnEffects(0);
                RelatedStartNote.LongHoldEffect.Stop();
            }
            else
            {
                //GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
                SpawnEffects(0);
            }
            return hitLevel;
        }
            //Branch When Flick
            if (Flick )
            {
                if ((targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate)) <= hitOffset &&
                    hitOffset <= (targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate)))

                {
                    hitLevel = 2;
                    Debug.Log("结果为 Flick-Pure, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
                    SfxManager.PlayOneShot(HitSound);
                    gameController.Pure++;
                    //GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
                    SpawnEffects(1);
                    return hitLevel;
                }
                else
                {
                    gameController.Lost++;
                    Debug.Log("结果为 Lost, " + "误差为" + Mathf.RoundToInt(targetOffset - hitOffset) / 44.4 + "ms.");
                    SpawnEffects(2);
                    this.enabled = false;
                    return hitLevel;
                }
            }
            // Detect offset when input
            if
                (targetOffset - (gameController.lostFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset
                <= targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate))
            {
                gameController.Combo = 0;
                hitLevel = 0;
                gameController.Lost++;
                Debug.Log("结果为 Lost, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
                SpawnEffects(2);
                this.enabled = false;
            }
            if
                (targetOffset - (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset
                <= targetOffset - (gameController.pureFloat * 0.001f * gameController.SampleRate))
            {
                hitLevel = 1;
                Debug.Log("结果为 FarEarly, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
                SfxManager.PlayOneShot(HitSound);
                gameController.Far++;
                //GameObject.Instantiate(PS_Far, gameObject.transform.position, Quaternion.identity);
                SpawnEffects(1);
            }
            if
                (targetOffset - (gameController.pureFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset
                <= targetOffset + (gameController.pureFloat * 0.001f * gameController.SampleRate))
            {
                hitLevel = 2;
                Debug.Log("结果为 Pure, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
                SfxManager.PlayOneShot(HitSound);
                gameController.Pure++;
                if (isLongNote && HoldEffect != null)
                {
                    LongHoldEffect = GameObject.Instantiate(HoldEffect, gameObject.transform.position, Quaternion.identity);
                    LongHoldEffect.Play();
                }
                else if (isLongNoteEnd && RelatedStartNote.LongHoldEffect != null)
                {
                    //GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
                    SpawnEffects(0);
                    RelatedStartNote.LongHoldEffect.Stop();
                }
                else
                {
                    //GameObject.Instantiate(PS_Pure, gameObject.transform.position, Quaternion.identity);
                    SpawnEffects(0);
            }
            }
            if
                (targetOffset + (gameController.pureFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset
                <= targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate))
            {
                hitLevel = 3;
                Debug.Log("结果为 FarLate, " + "误差为" + (int)((targetOffset - hitOffset) / 44.4) + "ms.");
                SfxManager.PlayOneShot(HitSound);
                gameController.Far++;
                //GameObject.Instantiate(PS_Far, gameObject.transform.position, Quaternion.identity);
                SpawnEffects(1);
            }
            if
                (targetOffset + (gameController.farFloat * 0.001f * gameController.SampleRate) <= hitOffset && hitOffset
                <= targetOffset + (gameController.lostFloat * 0.001f * gameController.SampleRate))
            {
                hitLevel = 0;
                gameController.Combo = 0;
                gameController.Lost++; 
                SpawnEffects(2);
                Debug.Log("结果为 Lost, " + "误差为" + Mathf.RoundToInt(targetOffset - hitOffset) / 44.4 + "ms.");
                this.enabled = false;
            }
        return hitLevel;
    }

    private void SpawnEffects(int hitlevel) // 0 - pure, 1 - far
    {
        switch (hitlevel)
        {
            case 0:
                if (SideNote)
                {
                    GameObject.Instantiate(PS_Pure[0], gameObject.transform.position, Quaternion.identity); // Ground 
                    GameObject.Instantiate(PS_Pure[1], gameObject.transform.position, Quaternion.Euler(90, 0, 0)); // Brust Ps
                }
                else
                {
                    GameObject.Instantiate(PS_Pure[0], gameObject.transform.position, Quaternion.Euler(85, 0, 0));
                    GameObject.Instantiate(PS_Pure[1], gameObject.transform.position, Quaternion.Euler(90, 0, 0));
                }
                break;
            case 1:
                foreach (ParticleSystem item in PS_Far)
                {
                    GameObject.Instantiate(item, gameObject.transform.position, Quaternion.identity);
                }
                break;
            case 2:
                IndiLost.SetTrigger("ShowNote");
                break;
            default:
                break;
        }
    }
}
