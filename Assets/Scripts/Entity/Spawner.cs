using System;
using Cysharp.Threading.Tasks;
using KinematicCharacterController;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }
    
    [SerializeField] private Transform _teamOneParent;
    [SerializeField] private Transform _teamTwoParent;
    
        [SerializeField] private float _spawnInterval = .3f;
    
    [SerializeField]
    private Transform _firstPlayerCannonTarget;
    [SerializeField]
    private Transform _secondPlayerCannonTarget;

    [SerializeField] private GameEntities _gameEntities;

    
    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    
    public async UniTask Spawn(EntitySO entitySO, int teamID)
    {
        var currentSpawnPosition = teamID == 1 ? _firstPlayerCannonTarget.position : _secondPlayerCannonTarget.position;
        await Spawn(entitySO, teamID, currentSpawnPosition, Quaternion.Euler(0, Random.value * 360, 0));
    }

    public async UniTask Spawn(EntitySO entitySO, int teamID, Vector3 position, Quaternion rotation)
    {
        var t = TimeSpan.FromSeconds(_spawnInterval);
        
        for (var i = 0; i < entitySO.SpawnAmount; i++)
        {
            if (destroyCancellationToken.IsCancellationRequested)
            {
                break;
            }

            var transform = teamID == 1 ? _teamOneParent : _teamTwoParent;
            Spawn(entitySO, transform, teamID, position, rotation)
                .Forget();
            
            await UniTask.Delay(t);
        }
    }

    private async UniTask Spawn(EntitySO entitySO, Transform parent, int teamID, Vector3 position, Quaternion rotation)
    {
        var instance = Instantiate(
            original: entitySO.Prefab,
            position: position,
            rotation,
            parent: parent);

        _gameEntities.AddEntity(instance);

        instance.Init(teamID, position, rotation, entitySO)
            .Forget();
    }
    
    private void Reset()
    {
        _gameEntities = FindObjectOfType<GameEntities>();
        
    }
}