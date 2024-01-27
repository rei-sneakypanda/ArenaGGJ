using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class DestroyInteractionWithOther : IInteractionWithOther
{
    [SerializeField] [OnValueChanged("Reset")] EffectTarget _effectTarget;

     [SerializeField, ShowIf("ShowSelf")] private DestroyInteractionSelf _destroyInteractionSelf;
     [SerializeField, ShowIf("ShowOther")] private DestroyInteractionSelf _destroyInteractionOther;
     
     private void Reset()
     {
         switch (_effectTarget)
         {
             case EffectTarget.Self:
                 _destroyInteractionOther = null;
                 break;
             case EffectTarget.Other:
                 _destroyInteractionSelf = null;
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
    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_destroyInteractionSelf != null)
        {
            
        if (_effectTarget == EffectTarget.Self || _effectTarget == EffectTarget.Both)
            await _destroyInteractionSelf.Interact(entity);
        }

        if (_destroyInteractionOther != null)
        {
            
        if (_effectTarget == EffectTarget.Other || _effectTarget == EffectTarget.Both)
            await _destroyInteractionOther.Interact(otherEntity);
        }
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