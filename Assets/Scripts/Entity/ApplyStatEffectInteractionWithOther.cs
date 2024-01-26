using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ApplyStatEffectInteractionWithOther : IInteractionWithOther
{
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private ApplyStatEffectInteraction _effectIntercationSelf;
    [SerializeField] private ApplyStatEffectInteraction _effectIntercationOther;

    
    public UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {

        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
            _effectIntercationSelf.Interact(entity);

        if ((_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other))
            _effectIntercationOther.Interact(otherEntity);

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