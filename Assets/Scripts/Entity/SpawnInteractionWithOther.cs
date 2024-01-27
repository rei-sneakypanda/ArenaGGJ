using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class SpawnInteractionWithOther : IInteractionWithOther
{
    [SerializeField]  [OnValueChanged("Reset")]private EffectTarget _effectTarget;
    [SerializeField, ShowIf("ShowSelf")] private SpawnInteraction _spawnInteractionSelf;
    [SerializeField, ShowIf("ShowOther")] private SpawnInteraction _spawnInteractionOther;
    private void Reset()
    {
        switch (_effectTarget)
        {
            case EffectTarget.Self:
                _spawnInteractionOther = null;
                break;
            case EffectTarget.Other:
                _spawnInteractionSelf = null;
                break;
        }
    }
    private bool ShowOther()
    {
        return _effectTarget == EffectTarget.Other || _effectTarget == EffectTarget.Both;
    }
    private bool ShowSelf()
    {
        return _effectTarget == EffectTarget.Self || _effectTarget == EffectTarget.Both;
    }
    
    public async UniTask Interact(GameEntity entity, GameEntity otherEntity)
    {
        if (_spawnInteractionSelf != null)
        {
            if (ShowSelf())
                await _spawnInteractionSelf.Interact(entity);
        }

        if (_spawnInteractionOther == null)
        {
            return;
        }

        if (ShowOther())
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
            Spawner.Instance.Spawn(_entityToSpawn, entity.TeamId, position, entity.SpawnLocation.rotation)
                .Forget();
            
            await UniTask.Delay(TimeSpan.FromSeconds(entity.StatHandler[StatType.SpawnTime].StatValue.Value));
        }
    }
}