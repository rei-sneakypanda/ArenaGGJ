using System;
using Cysharp.Threading.Tasks;
using Midbaryom.Pool;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class DestroyHandler : SerializedMonoBehaviour, IDisposable
{
    public event Action<GameEntity> OnDisposed; 
    [SerializeField] private GameEntity _gameEntity;
    private bool _flag;

    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _particleSystemDuration;
    [SerializeField] private EntityAnimator entityAnimator;

    private IDisposable _disposable;

    public void Init()
    {
        _disposable = _gameEntity.StatHandler[StatType.HP].StatValue.Where(x => x <= 0)
            .Take(1)
            .Subscribe(_ => DestroySelf().Forget());
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }

    public async UniTask DestroySelf()
    {
        if (_flag)
        {
            return;
        }

        _flag = true;

        try
        {
            if (_gameEntity.EntitySO.DestroyInteractionSelf != null)
            {
                foreach (var interaction in _gameEntity.EntitySO.DestroyInteractionSelf)
                {
                    await interaction.Interact(_gameEntity);
                }
            }

            GameEntities.Instance.RemoveEntity(_gameEntity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        _gameEntity.transform.SetParent(PoolManager.Instance.transform);
        ReturnToPool();
    }

    private void Reset()
    {
        entityAnimator = GetComponent<EntityAnimator>();
        _gameEntity = GetComponent<GameEntity>();
    }
    public void ReturnToPool()
    {
        OnDisposed?.Invoke(_gameEntity);
    }
    
    private void OnDestroy()
    {
        OnDisposed?.Invoke(_gameEntity);
    }
}