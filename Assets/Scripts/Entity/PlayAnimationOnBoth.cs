using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlayAnimationOnBoth
{
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private PlayAnimationOnSelf _animationTypeSelf;
    [SerializeField] private PlayAnimationOnSelf _animationTypeOther;
    
    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
        {
            await _animationTypeSelf.Interact(entity: entity);
        }

        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other)
        {
            await _animationTypeOther.Interact(otherEntity);
        }
    }
}


[Serializable]
public class PlayAnimationOnSelf : IInteractionOnSelf
{
    [SerializeField] private AnimationType _animationType;
    
    public async UniTask Interact(GameEntity entity)
    {
        try
        {
            await entity.EntityAnimator.PlayAnimation(_animationType);
        }
        catch (Exception e)
        {
            #if UNITY_EDITOR
            Debug.Log(e);
            
            #endif
        }
    }
}