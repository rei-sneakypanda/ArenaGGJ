using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class DestroyInteraction : IBaseInteraction
{
    [SerializeField] EffectTarget _effectTarget; 
    [SerializeField, Range(0, 1f)] private float _probablity = 1f;
    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if ((_effectTarget == EffectTarget.Self || _effectTarget == EffectTarget.Both)  && _probablity.GenerateRNDOption())
            await entity.DestroyHandler.DestroySelf();
        if ((_effectTarget == EffectTarget.Other || _effectTarget == EffectTarget.Both) && _probablity.GenerateRNDOption())
            await otherEntity.DestroyHandler.DestroySelf();
    }
}