using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDetect : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private SpriteRenderer PressVisual;

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Mouse Click");
        if (PressVisual != null)
        {
            PressVisual.enabled = true;
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Mouse Up");
        if (PressVisual != null)
        {
            PressVisual.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse Enter");
        if (PressVisual != null)
        {
            PressVisual.enabled = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse Exit");
        if (PressVisual != null)
        {
            PressVisual.enabled = false;
        }
    }
}
