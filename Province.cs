using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Province : Element
{
    //GENERAL
    [SerializeField]
    public string provinceName;
    public int provinceId;
    public new string tag;
    public Nation ROOT_nation;
    public Color32 color;
    public Color32 color2;
    public double provinceForceLimit;
    public int pop;
    public string terrain;

    //PATHFINDING
    public List<GameObject> _OBJ_Neighbours;
    public List<Province> _PROV_Neighbours;
    public double gScore;
    public double fScore;
    public Vector2 pos;
    public int weight;

    //COMBAT
    public Unit Unit;
    public bool hasUnit;


    public bool isHighlighted = false;

    override
    public void Tick()
    {
    }

    public void Init(string provinceName, string pop, string tag, Nation nation)
    { 

        this.provinceName=provinceName;
        this.pop = Int32.Parse(pop);
        this.tag = tag;
        this.ROOT_nation = nation;
        this.color = ROOT_nation.color;
        this.color2 = ROOT_nation.color2;
        pos = GetComponent<PolygonCollider2D>().bounds.center;

    }

    public void InitCom(string id, string weight, string terrain)
    {
        provinceId = Int32.Parse(id);
        this.weight = Int32.Parse(weight);
        this.terrain = terrain;

    }

    public void InitNeighbours(List<GameObject> nbs)
    {
        _OBJ_Neighbours = nbs;

        foreach (GameObject obj in _OBJ_Neighbours)
        {
            _PROV_Neighbours.Add(obj.GetComponent<Province>());
        }

    }
    void Update()
    {
        this.color = ROOT_nation.color;
        this.color2 = ROOT_nation.color2;
        if (isHighlighted) {
            //needs to be host color
            gameObject.GetComponent<MeshRenderer>().material.color = this.color2;
        } else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = this.color;
        }
    }
}
