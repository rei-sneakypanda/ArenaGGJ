using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IInteractionWithOther
{
    public UniTask Interact(GameEntity entity, GameEntity otherEntity);
}

public interface IInteractionOnSelf
{
    public UniTask Interact(GameEntity entity);
}

[Serializable]
public class TimeLoopInteractionTemplate
{
     public StatType StatIntervalType;
    [SerializeField] private List<IInteractionOnSelf> _interactionOnSelves;
    public IReadOnlyList<IInteractionOnSelf> InteractionOnSelves => _interactionOnSelves;
}


public class TimeLoopInteraction
{
    private readonly TimeLoopInteractionTemplate _timeLoopInteractionTemplate;
    private float _counter = 0;
    private bool _flag;
    public TimeLoopInteraction(TimeLoopInteractionTemplate timeLoopInteractionTemplate)
    {
        _timeLoopInteractionTemplate = timeLoopInteractionTemplate;
    }

    public async UniTask Tick(float tickTime, GameEntity entity)
    {
        if (_counter < entity.StatHandler[_timeLoopInteractionTemplate.StatIntervalType].StatValue.Value)
        {
            _counter += tickTime;
            _flag = false;
            return;
        }

        if (_flag)
            return;

        _flag = true;
        await Interaction(entity);
        _counter = 0;
    }

    private async UniTask Interaction(GameEntity gameEntity)
    {
        if (_timeLoopInteractionTemplate.InteractionOnSelves != null)
        {
            try
            {
                foreach (var interaction in _timeLoopInteractionTemplate.InteractionOnSelves)
                {
                    await interaction.Interact(gameEntity);
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Console.WriteLine(e);
#endif
            }
        }
    }
}


public static class ProbabilityExtensions
{
    public static bool GenerateRNDOption(this float chance)
    {
        return UnityEngine.Random.Range(0,1f) <= chance;
    }
}