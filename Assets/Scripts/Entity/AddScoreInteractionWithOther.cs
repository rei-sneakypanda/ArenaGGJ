using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AddScoreInteractionWithOther : IInteractionWithOther
{
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private AddScoreInteraction _addScoreInteractionSelf;
    [SerializeField] private AddScoreInteraction _addScoreInteractionOther;

    public UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_addScoreInteractionSelf != null)
        {
            if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
                _addScoreInteractionSelf.Interact(entity);
        }

        if (_addScoreInteractionOther != null)
        {
            if ((_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other))
                _addScoreInteractionOther.Interact(otherEntity);
        }

        return UniTask.CompletedTask;
    }
}


[Serializable]
public class AddScoreInteraction : IInteractionOnSelf
{
    
    [SerializeField] private bool _useExistingStatAsValue;
    
    [SerializeField, HideIf( "_useExistingStatAsValue")] private int _addAmount;
    [SerializeField, ShowIf("_useExistingStatAsValue")] private StatType _statTypeValue;
    
    
    [SerializeField, Range(0, 1f)] private float _probablity = 1f;
        
    public UniTask Interact(GameEntity entity)
    {
        if (!_probablity.GenerateRNDOption())
            return UniTask.CompletedTask;
        
        var teamId = entity.TeamId;
        var amount = _useExistingStatAsValue ? (int)entity.StatHandler[_statTypeValue].StatValue.Value : _addAmount;
        
            PlayersManager.Instance.AddScore(teamId, amount);
        // Add Score;
        return UniTask.CompletedTask;
    }
}
