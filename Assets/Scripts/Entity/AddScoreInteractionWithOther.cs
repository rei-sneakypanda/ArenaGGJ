using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class AddScoreInteractionWithOther : IInteractionWithOther
{
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private AddScoreInteraction _addScoreInteractionSelf;
    [SerializeField] private AddScoreInteraction _addScoreInteractionOther;
    public UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
                
        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
            _addScoreInteractionSelf.Interact(entity);
            
        if ((_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other))
            _addScoreInteractionOther.Interact(otherEntity);

        return UniTask.CompletedTask;
    }
}

        
[Serializable]
public class AddScoreInteraction : IInteractionOnSelf
{
    [SerializeField] private int _addAmount;
    [SerializeField, Range(0, 1f)] private float _probablity = 1f;
        
    public UniTask Interact(GameEntity entity)
    {
        if (!_probablity.GenerateRNDOption())
            return UniTask.CompletedTask;
        
        var teamId = entity.TeamId;
            PlayersManager.Instance.AddScore(teamId, _addAmount);
        // Add Score;
        return UniTask.CompletedTask;
    }
}
