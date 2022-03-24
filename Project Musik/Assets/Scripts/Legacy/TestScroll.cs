using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    using UnityEngine.UI;

public class TestScroll : MonoBehaviour
{
    public ScrollPanelVerticalLayout scrollPanel;

    private int index = 0;
    public void Add()
    {
        GameObject go = Instantiate(Resources.Load("Item")) as GameObject;
        go.name = index.ToString();
        go.transform.Find("Text").GetComponent<Text>().text = index.ToString();
        go.transform.Find("Button").GetComponent<Button>().onClick.AddListener
        (
           () =>{Remove(go.transform);}
        );

        scrollPanel.AddItem(go.transform);
        index++;
    }

    void Remove(Transform t)
    {
        scrollPanel.RemoveItem(t);
    }
}
