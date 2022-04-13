using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class FlickDetect : MonoBehaviour, IDragHandler, IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] bool Flicking;
    [Tooltip("Calculated in ms.")]
    [SerializeField] float ClearTime;
    public UnityEvent WhileFlicking;
    public UnityEvent WhenDone;
    //public List<Material> Mats = new List<Material>();
    private bool update;
    private bool InArea;

    public void Awake()
    {
        ClearTime = ClearTime * 0.001f;
    }
    public void Update()
    {
        if (InArea)
        {
            if (update)
            {
                Flicking = true;
            }
        }

        if (WhileFlicking != null && Flicking)
        {
            WhileFlicking.Invoke();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        update = true;
        UpdateStop();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Out");
        InArea = false;
        UpdateStop();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("In");
        InArea = true;
    }

    Coroutine CountingCoroutine;
    private void UpdateStop()
    {
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }
        CountingCoroutine = StartCoroutine(DeltaClear());
    }
    IEnumerator DeltaClear()
    {
        yield return new WaitForSeconds(ClearTime);
        Debug.Log("StopFlicking");
        Flicking = false;
        update = false;
        if (WhenDone != null)
        {
            WhenDone.Invoke();
        }
    }


}
