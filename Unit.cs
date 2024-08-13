using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Element
{
    [SerializeField]
    private int strength;
    private string unitName;
    public bool isSelected;
    public bool enable;

    public int daysToGoal;
    public int daysToNextNode;
    public bool isMoving;
    public int pathLength;
    public Vector2 pos;
    public Vector2 movePosDir;
    public Vector2 combatPos;
    public List<Province> path = new();
    public int tempPath;

    public Path pathObject;

    public GameObject outline;
    public GameObject flag;

    public GameObject battleObj;
    public GameObject BattleObjInstance;

    public Nation ROOT_nation;
    public Province province;

    public bool inCombat;
    public bool defender;
    public float cPosN;
    public float size;

    public double health;
    public double damagePotential;
    public double defencePotential;
     
    override
    public void Tick()
    {
        Color32 color = new Color(ROOT_nation.color.r / 1.25f, ROOT_nation.color.g / 1.25f, ROOT_nation.color.b / 1.25f, 255);
    }

    public Unit(int strength, string unitName)
    {
        this.strength = strength;
        this.unitName = unitName;
        isSelected = false;
        isMoving = false;
        inCombat = false;
        defender = false;

    }

    public void Start()
    {
        //PLACEHOLDERS
        health = 100;
        damagePotential = 10;
        defencePotential= 8;
    }

    #region Pathfinding
    /*When a unit is selected, and a destination is also selected via user input, the unit will determine if the path to the destination is valid
     * It will then create a list of a path, the path, if valid, will then be executed via UnitMove(), which will take a destination value that is
     * the same value of the next node in the list. Once that province weight has been taken in days, the unit will change its root province to
     * the destination, and then continue the cycle until there is no more provinces left to move. 
     * In the update function, it constantly checks if the unit is moving, and using the days coroutine in GameManager, will progressivley decrement
     * the days until the next node, in which the province changes and the unit moves. 
     * If 'daysToGoal' which is an accumulation of all province weights, is 0, the path is cleared, and the unit is no longer considered moving, but remains
     * selected
     */
    public void Update()
    {
        size = (float)(gameObject.GetComponent<PolygonCollider2D>().bounds.size.x * 1.35);
        cPosN = defender ? -size : size;

        province.hasUnit = true;
        this.combatPos = new Vector2(province.pos.x + cPosN, province.pos.y);
        this.outline = gameObject.transform.GetChild(0).gameObject;
        this.flag = this.outline.transform.GetChild(0).gameObject;
        this.pathObject = gameObject.transform.GetChild(1).GetComponent<Path>();
        this.flag.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Flags/" + this.ROOT_nation.nationTag);

        if (isSelected)
        {
            this.outline.GetComponent<SpriteRenderer>().material.color = Color.yellow;
        } else
        {
            this.outline.GetComponent<SpriteRenderer>().material.color = new Color(1, 50, 32);
        }

        if (enable)
        {
            gameObject.transform.position = this.pos;
        }

        if (isMoving && path.Count != 0)
        {
            if (pathLength < path.Count)
            {
                Province destination = path[pathLength];
                this.pos = province.pos;
                province.hasUnit = true;
                movePosDir = this.pos + new Vector2(((destination.pos.x - this.pos.x) / 5), ((destination.pos.y - this.pos.y) / 5));
                this.pos = movePosDir;
            }

            if (daysToNextNode <= 0 && pathLength < path.Count)
            {

                if (pathLength <= path.Count - 1)
                {
                    daysToNextNode = path[pathLength].weight;
                }
                UnitMove(path[pathLength]);
            }
            
        } else if (inCombat)
        {
            this.pos = combatPos;
        } else
        {
            this.pos = province.pos;
        }

    }

    /**
     * Find path first checks if there already is a path, if so, it clears it. 
     * It then creates a new pathfinding object and then returns the path as 'path'
     * it sets pathLength to 1. This is the pointer for the next node
     * it sets days to next node as 0 to begin with
     * It then checks if the path is null and bigger than 1, and then sets moving to true to commence it
     * It then calculates the total weight of the journey, minusing the first province in the path.
     * **/

    internal void FindPath(Province destination)
    {
        path?.Clear();
        AStar aStar = new(province, destination);
        path = aStar.FindPath(this);
        pathLength = 1;
        daysToNextNode = 0;

        if (path != null && path.Count > 1)
        {
            //UnitMove(path[pathLength]);
            isMoving = true;
            daysToNextNode = path[pathLength].weight;
            daysToGoal = 0; 
            foreach (Province p in path)
            {
                daysToGoal += p.weight;
            }
            daysToGoal -= path[0].weight;

            tempPath = path.Count;
        }

    }

    public void UnitMove(Province destination)
    {
        this.pos = destination.pos;
        tempPath--;
        pathLength++;
        province.hasUnit = false;
        province = destination;

        if (province.ROOT_nation != ROOT_nation)
        {

            if (province.hasUnit)
            {
                InitBattle(province);
            } else
            {
                Occupy(province);
            }
        }

        province.Unit = this;
        province.hasUnit = true;

    }
    #endregion

    public void InitBattle(Province p)
    {
        p.Unit.defender = true;
        p.Unit.inCombat = true;
        p.Unit.inCombat = true;
        p.Unit.isMoving = false;
        defender = false;
        isMoving = false;
        inCombat = true;

        var battleObjBattle = battleObj.GetComponent<Battle>();
        battleObjBattle.UnitA = this;
        battleObjBattle.UnitD = p.Unit;
        battleObjBattle.Province = p;
        battleObjBattle.instantiated = true;

        BattleObjInstance = Instantiate(battleObj, province.pos, Quaternion.identity);
    }

    public void BattleOutcome()
    {
        Destroy(BattleObjInstance);
    }

    public void Occupy(Province p)
    {
        p.ROOT_nation = ROOT_nation;
    }
}
