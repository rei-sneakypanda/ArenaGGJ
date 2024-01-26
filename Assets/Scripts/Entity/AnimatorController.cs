using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private static readonly int IsSpawning = Animator.StringToHash("Spawn");
    private static readonly int IsDestroyed = Animator.StringToHash("Destroyed");
    private static readonly int IsInteracting = Animator.StringToHash("Interacting");
    
    [SerializeField] private GameEntity _gameEntity;
    [SerializeField] private Animator Animator;

    [SerializeField] private float _spawnAnimationDuration;
    [SerializeField] private float _destroyAnimationDuration;
    [SerializeField] private float _interactingAnimationDuration;

    public async UniTask PlaySpawnAnimation()
    {
        try
        {
            Animator.SetBool(IsSpawning, true);
            var cancel = await UniTask.Delay(TimeSpan.FromSeconds(_spawnAnimationDuration), cancellationToken: _gameEntity.destroyCancellationToken).SuppressCancellationThrow();
            if (cancel)
            {
                return;
            }

            Animator.SetBool(IsSpawning, false);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    public async UniTask PlayInteractionAnimation()
    {
        try
        {
            Animator.SetBool(IsInteracting, true);
            var cancel = await UniTask.Delay(TimeSpan.FromSeconds(_interactingAnimationDuration), cancellationToken: _gameEntity.destroyCancellationToken).SuppressCancellationThrow();
            if (cancel)
            {
                return;
            }

            Animator.SetBool(IsInteracting, false);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
    
    public async UniTask PlayDestroyAnimation()
    {
        try
        {
            Animator.SetBool(IsDestroyed, true);
            var cancel = await UniTask.Delay(TimeSpan.FromSeconds(_destroyAnimationDuration), cancellationToken: _gameEntity.destroyCancellationToken).SuppressCancellationThrow();
            if (cancel)
            {
                return;
            }

            Animator.SetBool(IsDestroyed, false);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
    
    private void Reset()
    {
        Animator = GetComponent<Animator>();
        _gameEntity = GetComponent<GameEntity>(); 
    }
}