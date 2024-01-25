using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameEntities : SerializedMonoBehaviour
{
    public static GameEntities Instance { get; private set; }

    private HashSet<GameEntity> _allEntities = new();

    private void Awake()
    {
        Instance = this;
    }
    
    private void OnDestroy()
    {
        foreach (var gameEntity in _allEntities)
        {
            if (gameEntity.gameObject != null)
            {
                Destroy(gameEntity.gameObject);
            }
        }

        _allEntities.Clear();
        Instance = null;
    }

    public void SpawnObject(int teamID, EntitySO entitySO, Vector3 position, Quaternion rotation)
    {
        Instantiate(entitySO.Prefab, position, rotation).Init(teamID, position, rotation, entitySO);
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