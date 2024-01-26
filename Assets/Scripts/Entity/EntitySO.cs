using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "ArenaGGJ/New Entity Config")]
public class EntitySO : TagSO
{
    [TabGroup("General")]
    public TagGroupSO Tags;
    [TabGroup("General")]
    [PreviewField(75f)] public Sprite Image;
    [TabGroup("General")]
    public string Name;
    [TextArea]    [TabGroup("General")]
    public string Description;
    
    [TabGroup("General")]
    [PreviewField(75f)]
    [ShowInInspector]
    public GameObject PrefabView => Prefab.gameObject;
    [TabGroup("General")]
   public GameEntity Prefab;

    
    
    [TabGroup("Spawn")]
    public float TimeTillCanInteract = 1.25f;

[TabGroup("Stats")]
    public List<StatTemplate> Stats;
    
    [TabGroup("Spawn")]
    [SerializeField] List<IInteractionOnSelf> _spawnInteractionSelf;
    [TabGroup("Interval")]
    [OdinSerialize] List<TimeLoopInteractionTemplate> _intervalInteractionSelf;
    [TabGroup("Destroy")]
    [SerializeField] List<IInteractionOnSelf> _destroyInteractionSelf;
    
    [OdinSerialize][TabGroup("Interactions")]
    private List<InteractionPackage> _interactionPackage;
    public IReadOnlyList<InteractionPackage> InteractionPackage => _interactionPackage;
    public IReadOnlyList<IInteractionOnSelf> SpawnInteractionSelf => _spawnInteractionSelf;
    public IReadOnlyList<IInteractionOnSelf> DestroyInteractionSelf => _destroyInteractionSelf;
    public IReadOnlyList<TimeLoopInteractionTemplate> IntervalInteractionSelf => _intervalInteractionSelf;

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



    public static bool operator ==(TagSO tag, EntitySO entity)
    {
        if (ReferenceEquals(tag, entity))
            return true;
        if (ReferenceEquals(tag, null))
            return false;
        if (ReferenceEquals(entity, null))
            return false;


        bool containTag = false;
        bool isSame = tag.GetInstanceID() == entity.GetInstanceID();
        
        if ((entity.Tags?.Tags?.Any() ?? false) && !isSame)
        {
            foreach (var t in entity.Tags.Tags)
            {
                containTag |= tag == t;
                if (containTag)
                {
                    break;
                }
            }
        }

        return isSame || containTag;
    }

    public static bool operator !=(TagSO tag, EntitySO entity)
    { 
        if (ReferenceEquals(tag, entity)) 
            return false;
        if (ReferenceEquals(tag, null)) 
            return true;
        if (ReferenceEquals(entity, null))
            return true;

        var isNotSame =tag.GetInstanceID() != entity.GetInstanceID();
        var doesNotContainTag = true;
        if (isNotSame && (entity.Tags?.Tags?.Any() ?? false))
        {
            foreach (var t in entity.Tags.Tags)
            {
                doesNotContainTag &= tag != t;
                if (!doesNotContainTag)
                {
                    break;
                }
            }
        }
        
        return isNotSame && doesNotContainTag;
    }
}

public enum EffectTarget
{
    Both,
    Self,
    Other,
}