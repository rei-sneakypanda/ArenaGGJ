using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class GameEntity : SerializedMonoBehaviour
{
    public int TeamId { get; set; }
    [FormerlySerializedAs("_animatorController"),SerializeField] private EntityAnimator entityAnimator;
    
    private EntitySO _entitySO;
    private StatHandler _statHandler = new();
    [SerializeField] private MovementHandler _movementHandler;
    
    
    public MovementHandler MovementHandler  => _movementHandler;

    public InteractingContainer InteractingObjects;
    public StatHandler StatHandler => _statHandler;
    public EntitySO EntitySO => _entitySO;
    
    public async UniTask Init(int teamID, Vector3 position, Quaternion rotation, EntitySO entitySO)
    {
        TeamId = teamID;
        _entitySO = entitySO;
        _statHandler = new StatHandler(entitySO.Stats.ToArray());
        
        transform.position = position;
        transform.rotation = rotation;
        
        _movementHandler.Init();
        InteractingObjects.BlockForDuration(entitySO.TimeTillCanInteract)
            .Forget();
        await entityAnimator.PlaySpawnAnimation();
    }
}

public class DestroyHandler : MonoBehaviour
{
    [SerializeField] private GameEntity _gameEntity;
    private bool _flag;
    
    [SerializeField] private EntitySO entity;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _particleSystemDuration;
    [SerializeField] private EntityAnimator entityAnimator;
    
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

    private void Reset()
    {
        entityAnimator = GetComponent<EntityAnimator>();
        _gameEntity = GetComponent<GameEntity>(); 
    }
}