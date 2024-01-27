using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PlayAnimationInteraction : IInteractionWithOther
{
    [SerializeField] [OnValueChanged("Reset")] private EffectTarget _effectTarget;
     [SerializeField, ShowIf("ShowSelf")] private PlayAnimationOnSelf _playAnimationOnSelf;
     [SerializeField, ShowIf("ShowOther")]private PlayAnimationOnSelf _playAnimationOnOther;
     
     private void Reset()
     {
         switch (_effectTarget)
         {
             case EffectTarget.Self:
                 _playAnimationOnOther = null;
                 break;
             case EffectTarget.Other:
                 _playAnimationOnSelf = null;
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
        try
        {
            if (_playAnimationOnSelf != null)
            {
                if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
                    await _playAnimationOnSelf.Interact(entity);
            }

            if (_playAnimationOnOther != null)
            {
                if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other)
                    await _playAnimationOnOther.Interact(otherEntity);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
[Serializable]
public class ChangeTeamInteraction : IInteractionWithOther
{
    [SerializeField] [OnValueChanged("Reset")] private EffectTarget _effectTarget;
     [SerializeField, ShowIf("ShowSelf")]  private ChangeTeamOnSelf _changeTeamOnSelf;
     [SerializeField, ShowIf("ShowOther")] private ChangeTeamOnSelf _changeTeamOnOther;
     
     private void Reset()
     {
         switch (_effectTarget)
         {
             case EffectTarget.Self:
                 _changeTeamOnOther = null;
                 break;
             case EffectTarget.Other:
                 _changeTeamOnSelf = null;
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
        if (_changeTeamOnSelf != null)
        {
            if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Self)
            {
                await _changeTeamOnSelf.Interact(entity);
            }
        }

        if (_changeTeamOnOther != null)
        {
            if (_effectTarget == EffectTarget.Both || _effectTarget == EffectTarget.Other)
            {
                await _changeTeamOnOther.Interact(otherEntity);
            }
        }
    }
}
[Serializable]
public class ChangeTeamOnSelf : IInteractionOnSelf
{
    [Range(0, 1f), SerializeField] private float _probability = 1f;

    public UniTask Interact(GameEntity entity)
    {
        entity.TeamId = GetOppositeTeam(entity.TeamId);
        return UniTask.CompletedTask;
    }

    private int GetOppositeTeam(int team)
    {
        return team == 1 ? 1 : 2;
    }
}