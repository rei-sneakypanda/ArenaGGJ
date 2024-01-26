using System;
using Cysharp.Threading.Tasks;
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
        
        [SerializeField] private float _value;

        [SerializeField, Range(0, 1f)] private float _probablity = 1f;
        
        public UniTask Interact(GameEntity entity)
        {
            entity.StatHandler[_statType].SetValue(entity.StatHandler[_statType].StatValue.Value + _value);
            return UniTask.CompletedTask;
        }
    }