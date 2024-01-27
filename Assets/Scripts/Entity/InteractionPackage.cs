using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class InteractionPackage
{
    public TagSO Tag;

    public EffectTarget _targetEffect;
    
    public float BlockDuration;
    
    [SerializeField] private List<IInteractionWithOther> MainInteraction;

    public bool CanInteract(GameEntity entity, GameEntity otherEntity)
    {
        var canInteractWithSameTeam = false;

        switch (_targetEffect)
        {
            case EffectTarget.Both:
                canInteractWithSameTeam = true;
                break;
            case EffectTarget.Other:
                canInteractWithSameTeam = entity.TeamId != otherEntity.TeamId;
                break;
            
            case EffectTarget.Self:
                canInteractWithSameTeam = entity.TeamId == otherEntity.TeamId;
                break;
        }
        
        return canInteractWithSameTeam && Tag == otherEntity.EntitySO;
    }

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (otherEntity == null)
        {
            return;
        }
        
        try
        {
            if (MainInteraction != null && MainInteraction.Any())
            {
                await Interact(MainInteraction, entity, otherEntity);
            }
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    private async UniTask Interact(List<IInteractionWithOther> interactions, GameEntity entity, GameEntity otherEntity)
    {
        var cts = new CancellationTokenSource();
        using CompositeDisposable disposable = new();
        disposable.Add(entity.destroyCancellationToken.Register(Cancel));
        disposable.Add(otherEntity.destroyCancellationToken.Register(Cancel));
        disposable.Add(cts);
        
        var tasks = new List<UniTask>();
        try
        {
            foreach (var interaction in interactions)
            {
                tasks.Add(interaction.Interact(entity, otherEntity));
            }

            await UniTask.WhenAll(tasks).AttachExternalCancellation(cts.Token);
        }
        catch (Exception e)
        {
          Debug.Log(e);
        }

        void Cancel()
        {
            if (cts == null)
            {
                return;
            }

            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
    }
}