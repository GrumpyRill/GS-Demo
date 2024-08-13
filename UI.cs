using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : PanelUI
{
    public string displayPop;
    public Sprite displayFlag;
    public int displayFactories;
    public int displayGdp;
    public string displayName;

    public Text tPop;
    public Image tFlag;
    public Text tFactories;
    public Text tGdp;
    public Text tName;

    public void Display(string n, int n2, int n3, Sprite n4, string n5)
    {
        var go = gameObject.transform.GetChild(0);

        tFlag = go.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        tPop = go.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        tFactories = go.transform.GetChild(2).GetChild(0).GetComponent<Text>();
        tGdp = go.transform.GetChild(3).GetChild(0).GetComponent<Text>();
        tName = go.transform.GetChild(4).GetChild(0).GetComponent<Text>();

        displayPop = n;
        displayFactories = n2;
        displayGdp = n3;
        displayFlag = n4;
        displayName = n5;

    }

    public void Update()
    {
        if (isActiveAndEnabled)
        {
            tPop.text = displayPop;
            tFactories.text = displayFactories.ToString();
            tGdp.text = displayGdp.ToString();
            tFlag.sprite = displayFlag;
            tName.text = displayName;
        }
    }

}