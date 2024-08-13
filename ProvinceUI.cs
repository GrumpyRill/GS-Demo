using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProvinceUI : PanelUI
{
    public string displayName;
    public string displayTerrain;
    public int displayPopulation;
    public string displayOwner;
    public Sprite displayImage;

    public Text tName;
    public Text tTerrain;
    public Text tPopulation;
    public Text tOwner;
    public Image tImage;

    public void Display(string n, string n2, int n3, Sprite n4, string n5)
    {
        gameObject.SetActive(true);

        tName = gameObject.transform.GetChild(2).GetComponent<Text>();
        tTerrain = gameObject.transform.GetChild(3).GetComponent<Text>();
        tPopulation = gameObject.transform.GetChild(4).GetComponent<Text>();
        tImage = gameObject.transform.GetChild(5).GetComponent<Image>();
        tOwner = gameObject.transform.GetChild(6).GetComponent<Text>();

        displayName = n;
        displayTerrain = n2;
        displayPopulation = n3;
        displayImage = n4;
        displayOwner = n5;
    }
    public void Update()
    {
        if (isActiveAndEnabled)
        {
            tName.text = displayName;
            tTerrain.text = displayTerrain;
            tPopulation.text = displayPopulation.ToString() + "k";
            tImage.sprite = displayImage;
            tOwner.text = displayOwner.ToUpper();
        }
    }
}
