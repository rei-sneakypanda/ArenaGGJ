using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "ArenaGGJ/New Entity Config")]
public class EntitySO : TagSO
{
    [PreviewField(75f)] public Sprite Image;
    [PreviewField(75f)] public GameObject Prefab;

    public string Name;

    public TagGroupSO Tags;

    public List<StatTemplate> Stats;

    [OdinSerialize] private IBaseInteraction t;
    [OdinSerialize]
    private InteractionPackage InteractionPackage;
    
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


[Serializable]
public class InteractionPackage
{
    public TagSO[] Tags;
    
   [SerializeField] private IBaseInteraction StartInteraction;
   [SerializeField] private IBaseInteraction EndInteraction;
   [SerializeField] private IBaseInteraction UpdateInteraction;

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (otherEntity == null)
            return;
        
        try
        {
            if (StartInteraction != null)
                await StartInteraction.Interact(entity, otherEntity);

            if (UpdateInteraction != null)
                await UpdateInteraction.Interact(entity, otherEntity);

            if (EndInteraction != null)
                await EndInteraction.Interact(entity, otherEntity);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}

[Serializable]
public class StayInPlace : IBaseInteraction
{
    [SerializeField, Range(0, 10f)] private float _duration;
    [SerializeField] private ICondition<EntitySO> _condition;

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_condition.IsTrueFor(otherEntity.EntitySO))
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_duration));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }
}

public interface IBaseInteraction 
{
    public UniTask Interact(GameEntity entity, GameEntity otherEntity);
}