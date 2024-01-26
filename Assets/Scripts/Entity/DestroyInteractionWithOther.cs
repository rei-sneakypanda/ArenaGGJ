using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class DestroyInteractionWithOther : IInteractionWithOther
{
    [SerializeField] EffectTarget _effectTarget;

    [SerializeField] private DestroyInteractionSelf _destroyInteractionSelf;
    [SerializeField] private DestroyInteractionSelf _destroyInteractionOther;

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_effectTarget == EffectTarget.Self || _effectTarget == EffectTarget.Both)
            await _destroyInteractionSelf.Interact(entity);
        if (_effectTarget == EffectTarget.Other || _effectTarget == EffectTarget.Both)
            await _destroyInteractionOther.Interact(otherEntity);
    }
}

[Serializable]
public class DestroyInteractionSelf : IInteractionOnSelf
{
    [SerializeField, Range(0, 1f)] private float _probablity = 1f;

    public async UniTask Interact(GameEntity entity)
    {
        if (!_probablity.GenerateRNDOption())
        {
            return;
        }

        await entity.DestroyHandler.DestroySelf();
    }
}