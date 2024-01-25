using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "ArenaGGJ/New Entity Config")]
public class EntitySO : SerializedScriptableObject
{
    [PreviewField(75f)] public Sprite Image;
    [PreviewField(75f)] public GameObject Prefab;

    public string Name;

    public List<StatEditor> Stats;
    
    private void Reset()
    {
        Stats ??= new();
        foreach (var statType in (StatType[])Enum.GetValues(typeof(StatType)))
        {
            if (Stats.Any(x=> x.StatType == statType))
            {
               continue;
            }
            Stats.Add(new StatEditor(){ StatType = statType, StartingValue = 0});
        }
    }
}