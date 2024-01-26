using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    private static readonly int IsSpawning = Animator.StringToHash("Spawn");
    private static readonly int IsDestroyed = Animator.StringToHash("Destroyed");
    private static readonly int IsInteracting = Animator.StringToHash("Interacting");
    private static readonly int IsReacting = Animator.StringToHash("Reacting");
    
    [SerializeField] private GameEntity _gameEntity;
    [SerializeField] private Animator Animator;

    [SerializeField] private float _spawnAnimationDuration;
    [SerializeField] private float _destroyAnimationDuration;
    [SerializeField] private float _interactingAnimationDuration;

    public async UniTask PlaySpawnAnimation()
    {
        return;
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
    
    public async UniTask PlayReactingAnimation()
    {
        try
        {
            Animator.SetBool(IsReacting, true);
            var cancel = await UniTask.Delay(TimeSpan.FromSeconds(_interactingAnimationDuration), cancellationToken: _gameEntity.destroyCancellationToken).SuppressCancellationThrow();
            if (cancel)
            {
                return;
            }

            Animator.SetBool(IsReacting, false);
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