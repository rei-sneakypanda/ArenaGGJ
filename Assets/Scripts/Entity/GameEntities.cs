using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Midbaryom.Pool;
using Sirenix.OdinInspector;
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
    
    #if UNITY_EDITOR
    [SerializeField] private EntitySO _entitySO1;
    [SerializeField] private EntitySO _entitySO2;
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Spawner.Instance.Spawn( _entitySO1,TeamType.TeamRed, Vector3.zero, Quaternion.identity).Forget();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Spawner.Instance.Spawn( _entitySO2,TeamType.TeamBlue, Vector3.zero, Quaternion.identity).Forget();
        }
    }
    
    #endif
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
                gameEntity.transform.SetParent(PoolManager.Instance.transform);
               PoolManager.Instance.Return(gameEntity);
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