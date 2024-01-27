using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ApplyStatEffectInteractionWithOther : IInteractionWithOther
{
    [SerializeField] [OnValueChanged("Reset")] private EffectTarget _effectTarget;
    [SerializeField, ShowIf("ShowSelf")]  private ApplyStatEffectInteraction _effectIntercationSelf;
    [SerializeField, ShowIf("ShowOther")] private ApplyStatEffectInteraction _effectIntercationOther;
    private void Reset()
    {
        switch (_effectTarget)
        {
            case EffectTarget.Self:
                _effectIntercationOther = null;
                break;
            case EffectTarget.Other:
                _effectIntercationSelf = null;
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
        if (_effectIntercationSelf != null)
        {
            if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
                _effectIntercationSelf.Interact(entity);

        }

        if (_effectIntercationOther != null)
        {
            if ((_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other))
                _effectIntercationOther.Interact(otherEntity);
        }

        return UniTask.CompletedTask;
    }
}

[Serializable]
    public class ApplyStatEffectInteraction : IInteractionOnSelf
    {
        [SerializeField] private StatType _statType;

        [SerializeField] private bool _useExistingValueAsValue;
        [SerializeField,ShowIf("_useExistingValueAsValue")] private StatType _statTypeValue;
        
        [SerializeField, HideIf("_useExistingValueAsValue")] private float _value;

        [SerializeField, Range(0, 1f)] private float _probablity = 1f;
        
        public UniTask Interact(GameEntity entity)
        {
            if (!_probablity.GenerateRNDOption())
            {
                return UniTask.CompletedTask;
            }
     
            if (_useExistingValueAsValue)
            {
                entity.StatHandler[_statType].SetValue(entity.StatHandler[_statType].StatValue.Value +
                                                       entity.StatHandler[_statTypeValue].StatValue.Value);
            }
            else
            {
                entity.StatHandler[_statType].SetValue(entity.StatHandler[_statType].StatValue.Value + _value);
            }

            return UniTask.CompletedTask;
        }
    }