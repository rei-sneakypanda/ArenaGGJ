using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum AnimationType
{
    Spawn,
    Destroy,
    Interacting,
    Reacting,
}


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
    [SerializeField] private float _reactingAnimationDuration;

    private Dictionary<AnimationType, float> _animationToDelay = new Dictionary<AnimationType, float>(4);
    private Dictionary<AnimationType, int> _animationToHash = new Dictionary<AnimationType, int>(4);

    private void Awake()
    {
        _animationToDelay.Add(AnimationType.Spawn, _spawnAnimationDuration);
        _animationToDelay.Add(AnimationType.Destroy, _destroyAnimationDuration);
        _animationToDelay.Add(AnimationType.Interacting, _interactingAnimationDuration);
        _animationToDelay.Add(AnimationType.Reacting, _reactingAnimationDuration);
        
        _animationToHash.Add(AnimationType.Spawn, IsSpawning);
        _animationToHash.Add(AnimationType.Destroy, IsDestroyed);
        _animationToHash.Add(AnimationType.Interacting, IsInteracting);
        _animationToHash.Add(AnimationType.Reacting, IsReacting);
    }

    public async UniTask PlayAnimation(AnimationType animationType)
    {
        try
        {
            Animator.SetBool(_animationToHash[animationType], true);
            var cancel = await UniTask.Delay(TimeSpan.FromSeconds(_animationToDelay[animationType]),
                    cancellationToken: _gameEntity.destroyCancellationToken)
                .SuppressCancellationThrow();
            if (cancel)
            {
                return;
            }

            Animator.SetBool(_animationToHash[animationType], false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
   
    private void Reset()
    {
        Animator = GetComponent<Animator>();
        _gameEntity = GetComponent<GameEntity>(); 
    }
}