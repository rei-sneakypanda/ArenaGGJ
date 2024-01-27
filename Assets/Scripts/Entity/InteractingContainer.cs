using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;

public class InteractingContainer : MonoBehaviour
{
    [SerializeField] private float _interactionRadius;
    [SerializeField] private GameEntity _gameEntity;
    
    private HashSet<GameEntity> _interactingEntities = new();
    public IReadOnlyCollection<GameEntity> InteractingEntities => _interactingEntities;

    public bool IsInteractiveable = false;
    
    public IDisposable Add(GameEntity gameEntity)
    {
        _interactingEntities.Add(gameEntity);
        return Disposable.Create(() => _interactingEntities.Remove(gameEntity));
    }

    public async UniTask BlockForDuration(float delay)
    {
        IsInteractiveable = false;
        using var d = Disposable.Create(() => IsInteractiveable = true);
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
    }
    
    private void Update()
    {
        if (!IsInteractiveable || !_gameEntity.IsAlive)
        {
            return;
        }
        
        Scan();
    }

    [SerializeField] private bool _toDrawGizmos;

    private void OnDrawGizmosSelected()
    {
        if (!_toDrawGizmos || GameEntities.Instance == null)
        {
            return;
        }
            Gizmos.DrawSphere(transform.position, _interactionRadius);

        var allEntities = GameEntities.Instance.GetGameEntitiesCollection((_gameEntity));
        var transformPosition = transform.position;
        bool isHit = false;

        foreach (var entity in allEntities.Where(entitiy => entitiy != _gameEntity))
        {
            if(entity == null || entity.gameObject == null || this == null || gameObject == null)
                continue;
            
            
            isHit |= Vector3.Distance(entity.transform.position, transformPosition) < _interactionRadius;

            if (isHit)
            {
                break;
            }
        }

        Gizmos.color = isHit ? Color.green : Color.red;

        Gizmos.DrawSphere(transform.position, _interactionRadius);
    }

    private void Scan()
    {
        var entityInteractionPackage = _gameEntity.EntitySO.InteractionPackage;

        if (entityInteractionPackage == null || !entityInteractionPackage.Any())
        {
            return;
        }
        
        var allEntities = GameEntities.Instance.GetGameEntitiesCollection(_gameEntity);
        
        var transformPosition = transform.position;
        
        foreach (var entity in allEntities)
        {
            
            if(entity ==null || entity.gameObject == null)
                continue;

            var distance = Vector3.Distance(entity.transform.position, transformPosition);
            
            if (_interactingEntities.Contains(entity) ||
                !entity.InteractingObjects.IsInteractiveable ||
                distance >= _interactionRadius)
            {
                continue;
            }

            var interaction = entityInteractionPackage.FirstOrDefault(x => x.CanInteract(_gameEntity, entity));
            if (interaction == null)
            {
                continue;
            }
            
            StartInteraction(interaction, entity)
                .Forget();
        }
    }

    private async UniTask StartInteraction(InteractionPackage interaction, GameEntity entity)
    {
        using CompositeDisposable d = new();
        
        var cts = new CancellationTokenSource();
        d.Add(Add(entity));
        d.Add(entity.InteractingObjects.Add(_gameEntity));
        d.Add(entity.destroyCancellationToken.Register(Cancel));
        d.Add(destroyCancellationToken.Register(Cancel));
        d.Add(cts);

        try
        {
            await interaction.Interact(_gameEntity, entity);
            
            if (entity != null && entity.gameObject != null)
            {
                entity.InteractingObjects.BlockForDuration(interaction.BlockDuration)
                    .Forget();
            }

            if (_gameEntity != null && _gameEntity.gameObject != null)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(interaction.BlockDuration),
                    cancellationToken: cts.Token);
            }
            
       
        }
        catch (Exception e)
        {
            //ignored
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