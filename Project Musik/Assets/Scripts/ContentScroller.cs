using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ContentScroller : MonoBehaviour
{
    RectTransform SelfRectTrans;
    RectTransform Levels;
    public VerticalLayoutGroup group;
    public List<RectTransform> LevelList = new List<RectTransform>();
    void Awake()
    {
        //int LastElement;
        SelfRectTrans = gameObject.GetComponent<RectTransform>();
        float itemHeight;
        Levels = GameObject.Find("Levels").GetComponent<RectTransform>();
        foreach (RectTransform levels in Levels)
        {
            LevelList.Add(levels);
        }
        itemHeight = LevelList.First().rect.height;
        SelfRectTrans.sizeDelta = new Vector2(SelfRectTrans.sizeDelta.x,
            group.padding.top + group.padding.bottom + (LevelList.Count + 1 ) * itemHeight + (LevelList.Count - 1) * group.spacing);
    }
}
