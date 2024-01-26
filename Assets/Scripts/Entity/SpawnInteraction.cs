using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class SpawnInteraction : IBaseInteraction
{
    [SerializeField] private int _spawnAmount;
    [SerializeField, Range(0, 1f)] private float _probablity = 1f;
    [SerializeField] private EntitySO _entityToSpawn;

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
      
        for (var i = 0; i < _spawnAmount; i++)
        {
            if (entity == null || entity.gameObject == null ||  !_probablity.GenerateRNDOption())
                return;
            var position = entity.transform.position;
            Spawner.Instance.Spawn( _entityToSpawn,entity.TeamId, position, Quaternion.identity);
            await UniTask.Delay(TimeSpan.FromSeconds(entity.StatHandler[StatType.SpawnTime].StatValue.Value));
        }
    }
}
