using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "ArenaGGJ/New Entity Config")]
public class EntitySO : TagSO
{
    public TagGroupSO Tags;
    [PreviewField(75f)] public Sprite Image;
    [PreviewField(75f)] public GameEntity Prefab;

    public string Name;
    [TextArea]
    public string Description;

    public List<StatTemplate> Stats;

    [OdinSerialize]
    private List<InteractionPackage> _interactionPackage;
    public IReadOnlyList<InteractionPackage> InteractionPackage => _interactionPackage;

    private void Reset()
    {
        Stats ??= new();
        foreach (var statType in (StatType[])Enum.GetValues(typeof(StatType)))
        {
            if (Stats.Any(x => x.StatType == statType))
            {
                continue;
            }

            Stats.Add(new StatTemplate() { StatType = statType, StartingValue = 0 });
        }
    }
}

public enum EffectTarget
{
    Both,
    Self,
    Other,
}