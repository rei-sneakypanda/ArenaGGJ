using System;
using System.Collections.Generic;
using System.Linq;
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

    private bool _forceBlock = true;
    
    
    
    public async UniTask Add(GameEntity gameEntity, float delay)
    {
        using var d = Disposable.Create(() => _interactingEntities.Remove(gameEntity));
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
    }

    public async UniTask BlockForDuration(float delay)
    {
        _forceBlock = true;
        using var d = Disposable.Create(() => _forceBlock = false);
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
    }
    
    private void Update()
    {
        if (_forceBlock)
        {
            return;
        }
        
        Scan();
    }

    [SerializeField] private bool _toDrawGizmos;

    private void OnDrawGizmos()
    {
        if (!_toDrawGizmos  || GameEntities.Instance == null)
        {
            return;
        }
            Gizmos.DrawSphere(transform.position, _interactionRadius);

        var allEntities = GameEntities.Instance.GameEntitiesCollection;
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
        
        var allEntities = GameEntities.Instance.GameEntitiesCollection;
        var entityInteractionPackage = _gameEntity.EntitySO.InteractionPackage;
        var transformPosition = transform.position;
        
        foreach (var entity in allEntities)
        {
            if (entity|| _interactingEntities.Contains(entity) ||
                entity.gameObject == null ||
                Vector3.Distance(entity.transform.position, transformPosition) <= _interactionRadius)
            {
                continue;
            }

            var interaction = entityInteractionPackage.FirstOrDefault(x => x.CanInteract(_gameEntity, entity));
            if (interaction == null)
            {
                continue;
            }
            
            Add(entity, interaction.BlockDuration)
                .Forget();

            interaction.Interact(_gameEntity, entity)
                .Forget();
        }
    }
    
}