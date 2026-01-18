using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ReelIconValuesSO", menuName = "Scriptable Objects/ReelIconValuesSO")]
public class ReelIconValuesSO : SerializedScriptableObject
{
    [SerializeField] private Dictionary<ReelIcons, IconValues> reelValues;

    public Dictionary<ReelIcons, IconValues> ReelValues => reelValues;
}
