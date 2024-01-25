using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PushObject : IBaseInteraction
{
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private float _force;

    public UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
        {
            
        }
            
        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other)
        {
            
        }

        return UniTask.CompletedTask;
    }
}