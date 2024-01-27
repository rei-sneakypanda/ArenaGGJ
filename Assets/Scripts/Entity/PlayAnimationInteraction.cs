using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlayAnimationInteraction : IInteractionWithOther
{
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private PlayAnimationOnSelf _playAnimationOnSelf;
    [SerializeField] private PlayAnimationOnSelf _playAnimationOnOther;

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
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private ChangeTeamOnSelf _changeTeamOnSelf;
    [SerializeField] private ChangeTeamOnSelf _changeTeamOnOther;

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