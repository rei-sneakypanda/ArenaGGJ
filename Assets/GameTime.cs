using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameTime : MonoBehaviour
{
    public static event Action GameStarted;
    public static event Action GameEnded;
    public static GameTime Instance { get; private set; }
    [SerializeField] private float _explodingForce=30f;
    [SerializeField] public List<float> _timeToDrop;
    [SerializeField] public List<Rigidbody> _groundObjects;
    [SerializeField] private float _timeTillGameEnd;
    [SerializeField] private float _timeTillGameStart;
    
    [ShowInInspector] [Sirenix.OdinInspector.ReadOnly]
    private ReactiveProperty<float> _remainingTime = new();
    public IReadOnlyReactiveProperty<float> RemainingTime => _remainingTime;
    
    private void Awake()
    {
        Instance = this;
        _remainingTime.Value = _timeTillGameEnd;
    }

    private void Start()
    {
        StartCountDown()
            .Forget();
    }

    private async UniTask StartCountDown()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_timeTillGameStart), cancellationToken: destroyCancellationToken);
        GameStarted?.Invoke();

        while (_remainingTime.Value > 0)
        {
            _remainingTime.Value -= Time.deltaTime;

            if (_timeToDrop.Any())
            {
                if (_remainingTime.Value <= _timeToDrop.First())
                {
                    _timeToDrop.RemoveAt(0);

                    var ground = _groundObjects.First();
                    _groundObjects.RemoveAt(Random.Range(0, _groundObjects.Count - 1));

                    ground.useGravity = true;
                    ground.isKinematic = false;
                    ground.AddForce((ground.position - Vector3.zero) * _explodingForce, ForceMode.Impulse);
                }
            }
            
            await UniTask.DelayFrame(1, cancellationToken: destroyCancellationToken);
        }

        _remainingTime.Value = 0;
        GameEnded?.Invoke();
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
}