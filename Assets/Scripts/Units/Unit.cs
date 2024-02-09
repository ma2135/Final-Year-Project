using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField] GameObject gameObject;
    [SerializeField] UnitObject unit;

    public UnitObject GetUnitObject() { return unit; }
    public void SetUnitObject(UnitObject unit) { this.unit = unit; }

}
