using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarrelDatabase", menuName = "Scriptable Objects/BarrelDatabase")]
public class BarrelDatabase : ScriptableObject
{
    public List<BarrelData> barrels = new List<BarrelData>();
}
