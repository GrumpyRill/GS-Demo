using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ArmyUI: PanelUI
{
    public string displayName;
    public List<Unit> units;
    public Sprite displaySprite;

    public Text tName;
    public Image tImage;

    public void Display(string n, Sprite n2)
    {
        gameObject.SetActive(true);

        tName = gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        tImage = gameObject.transform.GetChild(2).GetComponent<Image>();

        displayName = n;
        displaySprite = n2;

    }

    public void Update()
    {
        if (isActiveAndEnabled)
        {
            tName.text = displayName;
            tImage.sprite = displaySprite;
        }
    }
}
