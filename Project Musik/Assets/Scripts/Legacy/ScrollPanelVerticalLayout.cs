using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPanelVerticalLayout : MonoBehaviour
{
    private List<Transform> items = new List<Transform>();
    private RectTransform contentRectTra;//����  
    private VerticalLayoutGroup group;//���ڼ������ݵĸ߶�
    private float itemHeight;//����ĸ߶�

    void Awake()
    {
        contentRectTra = transform.Find("Content").GetChild(0).GetComponent<RectTransform>();
        contentRectTra.sizeDelta = new Vector2(contentRectTra.sizeDelta.x, 0);
        group = contentRectTra.GetComponent<VerticalLayoutGroup>();
    }

    //����б���  
    public void AddItem(Transform t)
    {
        if (itemHeight == 0f) itemHeight = t.GetComponent<RectTransform>().rect.height;

        t.SetParent(contentRectTra.transform);
        t.localScale = Vector3.one;
        items.Add(t);

        contentRectTra.sizeDelta = new Vector2(contentRectTra.sizeDelta.x,
            group.padding.top + group.padding.bottom + items.Count * itemHeight + (items.Count - 1) * group.spacing);
    }

    //�Ƴ��б���  
    public void RemoveItem(Transform t)
    {
        //Debug.Log(t.gameObject.name);
        int index = items.IndexOf(t);
        items.Remove(t);
        Destroy(t.gameObject);

        contentRectTra.sizeDelta = new Vector2(contentRectTra.sizeDelta.x,
            group.padding.top + group.padding.bottom + items.Count * itemHeight + (items.Count - 1) * group.spacing);
    }  

}
