using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class SpawnInteractionWithOther : IInteractionWithOther
{
    [SerializeField] private EffectTarget _effectTarget;
    [SerializeField] private SpawnInteraction _spawnInteractionSelf;
    [SerializeField] private SpawnInteraction _spawnInteractionOther;

    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_effectTarget == EffectTarget.Self || _effectTarget == EffectTarget.Both)
            await _spawnInteractionSelf.Interact(entity);

        if (_effectTarget == EffectTarget.Other || _effectTarget == EffectTarget.Both)
            await _spawnInteractionOther.Interact(otherEntity);
    }
}

[Serializable]
public class SpawnInteraction : IInteractionOnSelf
{
    [SerializeField] private int _spawnAmount;
    [SerializeField, Range(0, 1f)] private float _probablity = 1f;
    [SerializeField] private EntitySO _entityToSpawn;
    
    public async UniTask Interact(GameEntity entity)
    {
        for (var i = 0; i < _spawnAmount; i++)
        {
            if (entity == null || entity.gameObject == null)
                return;
            
            if(!_probablity.GenerateRNDOption()) continue;

                var position = entity.SpawnLocation.position;
            Spawner.Instance.Spawn(_entityToSpawn, entity.TeamId, position, Quaternion.identity)
                .Forget();
            
            await UniTask.Delay(TimeSpan.FromSeconds(entity.StatHandler[StatType.SpawnTime].StatValue.Value));
        }
    }
}