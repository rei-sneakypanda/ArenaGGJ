using System;
using Cysharp.Threading.Tasks;
using KinematicCharacterController;
using Midbaryom.Pool;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }
    
    [FormerlySerializedAs("_teamOneParent"),SerializeField] private Transform _teamRedParent;
    [FormerlySerializedAs("_teamTwoParent"),SerializeField] private Transform _teamBlueParent;
    
        [SerializeField] private float _spawnInterval = .3f;
    
    [FormerlySerializedAs("_firstPlayerCannonTarget"),SerializeField]
    private Transform _redPlayerCannonTarget;
    [FormerlySerializedAs("_secondPlayerCannonTarget"),SerializeField]
    private Transform _bluePlayerCannonTarget;

    [SerializeField] private GameEntities _gameEntities;

    
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    
    public async UniTask Spawn(EntitySO entitySO, TeamType teamID)
    {
        var currentSpawnPosition = teamID == TeamType.TeamRed ? _redPlayerCannonTarget.position : _bluePlayerCannonTarget.position;
        await Spawn(entitySO, teamID, currentSpawnPosition, Quaternion.Euler(0, Random.value * 360, 0));
    }

    public async UniTask Spawn(EntitySO entitySO, TeamType teamID, Vector3 position, Quaternion rotation)
    {
        var t = TimeSpan.FromSeconds(_spawnInterval);
        
        for (var i = 0; i < entitySO.SpawnAmount; i++)
        {
            if (destroyCancellationToken.IsCancellationRequested)
            {
                break;
            }

            var transform = teamID == TeamType.TeamRed ? _teamRedParent : _teamBlueParent;
            Spawn(entitySO, transform, teamID, position, rotation);
            
            await UniTask.Delay(t);
        }
    }

    private void Spawn(EntitySO entitySO, Transform parent, TeamType teamID, Vector3 position,
        Quaternion rotation)
    {
        var instance = PoolManager.Instance.Pull(entitySO, position: position, rotation: rotation);
        instance.transform.SetParent(parent);
        instance.gameObject.SetActive(true);
        _gameEntities.AddEntity(instance);

        instance.Init(teamID, position, rotation, entitySO)
            .Forget();
    }

    private void Reset()
    {
        _gameEntities = FindObjectOfType<GameEntities>();
        
    }
}