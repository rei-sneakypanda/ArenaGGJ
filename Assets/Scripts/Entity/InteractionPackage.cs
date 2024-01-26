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
    public EntitySO[] OtherEntities;

     public bool IsInteractingOther;
    
    public float BlockDuration;
    
    [SerializeField] private List<IInteractionWithOther> StartInteraction;
    [SerializeField] private List<IInteractionWithOther> MainInteraction;
    [SerializeField] private List<IInteractionWithOther> EndInteraction;

    public bool CanInteract(GameEntity entity, GameEntity otherEntity)
    {
        var canInteractWithSameTeam = IsInteractingOther ? entity.TeamId != otherEntity.TeamId : entity.TeamId == otherEntity.TeamId;
        return canInteractWithSameTeam && OtherEntities.Contains(otherEntity.EntitySO);
    }

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (otherEntity == null)
        {
            return;
        }
        
        try
        {
            if (StartInteraction != null && StartInteraction.Any())
            {
                await Interact(StartInteraction, entity, otherEntity);
            }

            if (MainInteraction != null && MainInteraction.Any())
            {
                await Interact(MainInteraction, entity, otherEntity);
            }

            if (EndInteraction != null && EndInteraction.Any())
            {
                await Interact(EndInteraction, entity, otherEntity);
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
            // ignored
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