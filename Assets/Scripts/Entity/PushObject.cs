using System;
using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PushObjects : IInteractionWithOther
{
    [SerializeField] [OnValueChanged("Reset")] private EffectTarget _effectTarget;
     [SerializeField, ShowIf("ShowSelf")]  PushObjectSelf _pushObjectSelf;
     [SerializeField, ShowIf("ShowOther")] PushObjectSelf _pushObjectOther;
     
     private void Reset()
     {
         switch (_effectTarget)
         {
             case EffectTarget.Self:
                 _pushObjectOther = null;
                 break;
             case EffectTarget.Other:
                 _pushObjectSelf = null;
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
        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
        {
            _pushObjectSelf?.Interact(entity);
        }

        if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other)
        {
            _pushObjectOther?.Interact(otherEntity);
        }

        return UniTask.CompletedTask;
    }
}

[Serializable]
    public class PushObjectSelf : IInteractionOnSelf
    {
        [SerializeField] private float _force;
        [SerializeField] private TransformDirectionType _transformDirectionType;
        
        public UniTask Interact(GameEntity entity)
        {
            entity.MovementHandler.AddVelocity(_transformDirectionType.GetDirection(entity.ForwardTransform) * _force);
            return UniTask.CompletedTask; 
        }
    }

    public enum TransformDirectionType
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
    }

public static class TransformExtensions
{
    public static Vector3 GetDirection(this TransformDirectionType directionType, Transform transform)
    {
        switch (directionType)
        {
            case TransformDirectionType.Forward:
                return transform.forward;
            case TransformDirectionType.Backward:
                return -transform.forward;
            case TransformDirectionType.Left:
                return -transform.right;
            case TransformDirectionType.Right:
                return transform.right;
            case TransformDirectionType.Up:
                return transform.up;
            default:
                throw new ArgumentOutOfRangeException(nameof(directionType), directionType, null);
        }
    }

}
