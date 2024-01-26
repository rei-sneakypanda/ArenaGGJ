using Cysharp.Threading.Tasks;
using UnityEngine;

public class DestroyHandler : MonoBehaviour
{
    [SerializeField] private GameEntity _gameEntity;
    private bool _flag;

    [SerializeField] private EntityAnimator _animator;
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