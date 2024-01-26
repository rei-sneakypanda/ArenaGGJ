using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class WaitDelayBetweenInteractionsWithOther : IInteractionWithOther
{
    [SerializeField, Range(0, 10f)] private float _duration;

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
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
[Serializable]
public class WaitDelayBetweenInteractions : IInteractionOnSelf
{
    
    [SerializeField, Range(0, 10f)] private float _duration;
    public async UniTask Interact(GameEntity entity)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: entity.destroyCancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}