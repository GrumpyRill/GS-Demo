using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class Battle : Element
{
    [SerializeField]
    public Province Province;  
    public Unit UnitA;
    public Unit UnitD;

    public bool instantiated = false;

    public Battle(Province province, Unit unitA, Unit unitD)
    {
        this.UnitA = unitA;
        this.UnitD = unitD;
        this.Province = province;
    }

    override
    public void Tick()
    {
        if (UnitD.health > 0 && UnitA.health > 0)
        {
            UnitD.health -= (UnitA.damagePotential * 0.85) / UnitD.defencePotential;
            UnitA.health -= UnitD.defencePotential / UnitA.defencePotential;
        }

        Unit defeatedUnit;

        if (UnitD.health <= 0)
        {
            defeatedUnit = UnitD;
            defeatedUnit.ROOT_nation.unitList.Remove(defeatedUnit.gameObject);
            Destroy(defeatedUnit.gameObject);
            UnitA.inCombat = false;
            UnitA.BattleOutcome();
        }
        else 
        {
            defeatedUnit = UnitA;
            defeatedUnit.ROOT_nation.unitList.Remove(defeatedUnit.gameObject);
            Destroy(defeatedUnit.gameObject);
            UnitD.inCombat = false;
            UnitA.BattleOutcome();
        }


    }
}
