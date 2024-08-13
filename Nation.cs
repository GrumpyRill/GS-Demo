using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class Nation : Element
{
    [SerializeField]
    public string nationName;
    public string prefix;
    public string nationTag;
    public string capital;
    public int pop;
    public int manpower;
    public Color32 color;
    public Color32 color2;
    public Dictionary<string, Province> provinceList;
    public List<GameObject> unitList = new();

    public void AddProvince(string id, Province province)
    {
        provinceList.Add(id.Trim(), province);
    }

    public void InitCom(string name, string tag, Color32 color, Color32 color2, string prefix)
    {
        this.provinceList = new Dictionary<string, Province>();
        this.prefix = prefix;
        this.nationName= name;
        this.nationTag = tag;
        this.color = color;
        this.color2 = color2;
    }

    public void InitHist(string capital)
    {
        this.capital = capital.Trim();
    }

    override
    public void Tick()
    {
        CalcStrength();

        if (unitList != null || unitList.Count > 0)
        {
            foreach (var unit in unitList)
            {
                unit.GetComponent<Unit>().Tick();   
            }
        }
    }
    public void CalcStrength()
    {
        int tpop = 0;
        foreach (Province province in provinceList.Values)
        {
            tpop += province.pop;
        }
        pop = tpop;
        manpower = pop / 20;
    }


    public void InitArmy(string province)
    {
        if (province.Contains("0"))
        {
            province = province.Replace("0", "");
        }

        GameObject unit;
        unit = (GameObject)Resources.Load("Prefabs/Unit");
        unit.GetComponent<Unit>().ROOT_nation = this;
        unit.GetComponent<Unit>().enable = true;
        unit.GetComponent<Unit>().province = provinceList[province];
        GameObject unitInstance = Instantiate(unit, provinceList[province].GetComponent<PolygonCollider2D>().bounds.center, Quaternion.identity);
        unitList.Add(unitInstance);
    }
} 
