using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class GameEntity : SerializedMonoBehaviour
{
    public int TeamId { get; set; }
    [FormerlySerializedAs("_animatorController"),SerializeField] private EntityAnimator entityAnimator;
    
    private EntitySO _entitySO;
    private StatHandler _statHandler = new();
    public InteractingContainer InteractingObjects { get; } = new ();
    public StatHandler StatHandler => _statHandler;
    public EntitySO EntitySO => _entitySO;
    
    public async UniTask Init(int teamID, Vector3 position, Quaternion rotation, EntitySO entitySO)
    {
        TeamId = teamID;
        _entitySO = entitySO;
        _statHandler = new StatHandler(entitySO.Stats.ToArray());
        transform.position = position;
        transform.rotation = rotation;
        
        await entityAnimator.PlaySpawnAnimation();
    }
    
}

public class DestroyHandler : MonoBehaviour
{
    [SerializeField] private GameEntity _gameEntity;
    private bool _flag;
    private CancellationTokenSource _cancellationTokenSource = new();
    
    [SerializeField] private EntitySO entity;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _particleSystemDuration;
    [FormerlySerializedAs("_animatorController"),SerializeField] private EntityAnimator entityAnimator;
    public CancellationTokenSource CancellationTokenSource => _cancellationTokenSource;

    public async UniTask DestroySelf()
    {
        if (_flag)
        {
            return;
        }

        _flag = true;
        await entityAnimator.PlayDestroyAnimation();
        GameEntities.Instance.RemoveEntity(_gameEntity);
        
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    private void Reset()
    {
        entityAnimator = GetComponent<EntityAnimator>();
        _gameEntity = GetComponent<GameEntity>(); 
    }
}

public class InteractingContainer
{
    private HashSet<GameEntity> _interactingEntities = new();
    public IReadOnlyCollection<GameEntity> InteractingEntities => _interactingEntities;

    public async UniTask Add(GameEntity gameEntity, float delay)
    {
        if (!InteractingEntities.Contains(gameEntity))
        {
            return;
        }
        
        using var d = Disposable.Create(() => _interactingEntities.Remove(gameEntity));
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
    }

}
public class MovementHandler : MonoBehaviour
{
    [SerializeField] private GameEntity _gameEntity;

    private void FixedUpdate()
    {
        throw new NotImplementedException();
    }
}