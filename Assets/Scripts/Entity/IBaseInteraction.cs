using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IBaseInteraction
{
    public UniTask Interact(GameEntity entity, GameEntity otherEntity);
}
[Serializable]
public class SpawnInteraction : IBaseInteraction
{
    [SerializeField] private int _spawnAmount;
    
    [SerializeField] private EntitySO _entityToSpawn;

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        for (var i = 0; i < _spawnAmount; i++)
        {
            if (entity == null || entity.gameObject == null)
                return;

            var position = entity.transform.position;
            GameEntities.Instance.SpawnObject(entity.TeamId, _entityToSpawn, position, Quaternion.identity);
            await UniTask.Delay(TimeSpan.FromSeconds(entity.StatHandler[StatType.SpawnTime].StatValue.Value));
        }
    }
}