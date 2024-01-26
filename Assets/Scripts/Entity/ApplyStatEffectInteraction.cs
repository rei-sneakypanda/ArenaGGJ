using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ApplyStatEffectInteraction : IBaseInteraction
{
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private StatType _statType;
    [SerializeField] private float _value;

    public UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
            entity.StatHandler[_statType].SetValue(entity.StatHandler[_statType].StatValue.Value + _value);

        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other)
            otherEntity.StatHandler[_statType].SetValue(entity.StatHandler[_statType].StatValue.Value + _value);

        return UniTask.CompletedTask;
    }
}