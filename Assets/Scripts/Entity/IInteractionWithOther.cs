using System;
using System.Collections.Generic;
using System.Linq;
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
public class TimeLoopInteraction
{
    public StatType StatTimeType;
    [SerializeField] private List<IInteractionOnSelf> _interactionOnSelves;
    private float _counter = 0;

    public async UniTask Tick(float tickTime, GameEntity entity)
    {
        if (_counter < entity.StatHandler[StatTimeType].StatValue.Value)
        {
            _counter += tickTime;
            return;
        }

        await Interaction(entity);
        _counter = 0;
    }

    private async UniTask Interaction(GameEntity gameEntity)
    {
        if (_interactionOnSelves != null)
        {
            try
            {
                foreach (var interaction in _interactionOnSelves)
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