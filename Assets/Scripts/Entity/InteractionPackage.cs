using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[Serializable]
public class InteractionPackage
{
    public EntitySO[] OtherEntities;

    public bool _canInteractWithSameTeam;
    
    public float BlockDuration;
    
    [SerializeField] private List<IBaseInteraction> StartInteraction;
    [SerializeField] private List<IBaseInteraction> MainInteraction;
    [SerializeField] private List<IBaseInteraction> EndInteraction;

    public bool CanInteract(GameEntity entity, GameEntity otherEntity)
    {
        var canInteractWithSameTeam = _canInteractWithSameTeam ? entity.TeamId == otherEntity.TeamId : entity.TeamId != otherEntity.TeamId;
        return canInteractWithSameTeam && OtherEntities.Contains(otherEntity.EntitySO);
    }

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (otherEntity == null
            || entity.InteractingObjects.InteractingEntities.Contains(otherEntity))
            return;

        entity.InteractingObjects.Add(otherEntity, BlockDuration).Forget();
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
        }
    }

    private async UniTask Interact(List<IBaseInteraction> interactions, GameEntity entity, GameEntity otherEntity)
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
            Debug.LogError(e);
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