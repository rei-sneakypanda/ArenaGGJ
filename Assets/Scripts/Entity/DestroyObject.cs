using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class DestroyObject : IBaseInteraction
{
    [SerializeField] private EffectTarget _effectTarget;
    
    public UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if(_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
            entity.gameObject.SetActive(false);

        return UniTask.CompletedTask;
    }
}