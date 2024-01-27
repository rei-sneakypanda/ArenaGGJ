using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AddScoreInteractionWithOther : IInteractionWithOther
{
    [SerializeField] [OnValueChanged("Reset")] private EffectTarget _effectTarget;
    [SerializeField, ShowIf("ShowSelf")]  private AddScoreInteraction _addScoreInteractionSelf;
    [SerializeField, ShowIf("ShowOther")] private AddScoreInteraction _addScoreInteractionOther;


    private void Reset()
    {
        switch (_effectTarget)
        {
            case EffectTarget.Self:
                _addScoreInteractionOther = null;
                break;
            case EffectTarget.Other:
                _addScoreInteractionSelf = null;
                break;
        }
    }
    
    private bool ShowOther()
    {
        return _effectTarget == EffectTarget.Other || _effectTarget == EffectTarget.Both;
    }
    private bool ShowSelf()
    {
        return _effectTarget == EffectTarget.Self || _effectTarget == EffectTarget.Both;
    }
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

    [SerializeField] private bool _isIntervalScoreText;
    
    [SerializeField, Range(0, 1f)] private float _probablity = 1f;
        
    public UniTask Interact(GameEntity entity)
    {
        if (!_probablity.GenerateRNDOption())
            return UniTask.CompletedTask;
        
        var teamId = entity.TeamId;
        var amount = _useExistingStatAsValue ? (int)entity.StatHandler[_statTypeValue].StatValue.Value : _addAmount;
        
        PlayersManager.Instance.AddScore(teamId, amount);
        
        if (_isIntervalScoreText)
        {
            TextManager.Instance.PlayIntervalScoreInteraction(entity, amount, entity.transform.position)
                .Forget();
        }
        else
        {
            TextManager.Instance.PlayScoreInteraction(entity, amount, entity.transform.position)
                .Forget();
        }

        return UniTask.CompletedTask;
    }
}
