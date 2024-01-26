using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameEntities : MonoBehaviour
{
    [SerializeField] private Transform _teamOneParent;
    [SerializeField] private Transform _teamTwoParent;
    public static GameEntities Instance { get; private set; }

    private HashSet<GameEntity> _allEntities = new();

    public IReadOnlyCollection<GameEntity> GameEntitiesCollection => new HashSet<GameEntity>(_allEntities);
    private void Awake()
    {
        Instance = this;
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

    
    [SerializeField] private EntitySO _entitySO1;
    [SerializeField] private EntitySO _entitySO2;
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnObject(1, _entitySO1, Vector3.zero, Quaternion.identity);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnObject(2, _entitySO2, Vector3.zero, Quaternion.identity);
        }
    }

    public void SpawnObject(int teamID, EntitySO entitySO, Vector3 position, Quaternion rotation)
    {
       var instance =  Instantiate(
                original: entitySO.Prefab,
                position,
                rotation,
                parent: teamID == 1 ? _teamOneParent : _teamTwoParent);

       AddEntity(instance);
            instance.Init(teamID, position, rotation, entitySO)
            .Forget();
    }

    public void AddEntity(GameEntity gameEntity)
    {
        _allEntities.Add(gameEntity);
    }

    public void RemoveEntity(GameEntity gameEntity)
    {
        _allEntities.Remove(gameEntity);
    }
}