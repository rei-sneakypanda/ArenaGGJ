using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class GameEntities : MonoBehaviour
{
    [SerializeField] private float _maximumDistanceFromCenter;
    [SerializeField] private List<EntitySO> _allPossibleEntities;
    
    public IReadOnlyList<EntitySO> AllPossibleEntities => _allPossibleEntities;
    
    public static GameEntities Instance { get; private set; }

    private HashSet<GameEntity> _allEntities = new();

    public IReadOnlyCollection<GameEntity> GetGameEntitiesCollection(GameEntity exclude) => new HashSet<GameEntity>(_allEntities.Where(x => x != exclude));
    private void Awake()
    {
        Instance = this;
    }
    
    [SerializeField] private EntitySO _entitySO1;
    [SerializeField] private EntitySO _entitySO2;
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Spawner.Instance.Spawn( _entitySO1,1, Vector3.zero, Quaternion.identity).Forget();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Spawner.Instance.Spawn( _entitySO2,2, Vector3.zero, Quaternion.identity).Forget();
        }
    }
    
    private void OnDestroy()
    {
        foreach (var gameEntity in _allEntities)
        {
            if ( gameEntity != null && gameEntity.gameObject != null)
            {
                Destroy(gameEntity.gameObject);
            }
        }

        _allEntities.Clear();
        Instance = null;
    }

    private void FixedUpdate()
    {
        var centerPos = Vector3.zero;
        foreach (var gameEntity in GetGameEntitiesCollection(null))
        {
            if(gameEntity == null || gameEntity.gameObject == null)
            {
                continue;
            }

            if (Vector3.Distance(gameEntity.transform.position, centerPos) > _maximumDistanceFromCenter)
            {
                Destroy(gameEntity.gameObject);
            }
        }
    }

    public void AddEntity(GameEntity gameEntity)
    {
        _allEntities.Add(gameEntity);
    }

    public void RemoveEntity(GameEntity gameEntity)
    {
        _allEntities.Remove(gameEntity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, _maximumDistanceFromCenter);
    }
}

public class Player
{
    private ReactiveProperty<int> _playerScore = new(0);
    private int _teamID;
    private ReactiveProperty<EntitySO> _currentEntity = new();
    public IReadOnlyReactiveProperty<EntitySO> CurrentEntity => _currentEntity;
    
    public int TeamID => _teamID;
    
    public Player(int id)
    {
        _teamID = id;
        SetRandomEntity();
    }

    public void SetRandomEntity()
    {
        _currentEntity.Value = GameEntities.Instance.AllPossibleEntities
            .Except(_currentEntity.Value == null ? Enumerable.Empty<EntitySO>() : new []{ _currentEntity.Value})
            .OrderBy(_ => Random.value)
            .First();
    }
}