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
    
    [SerializeField]
    private Transform _firstPlayerCannon;
    [SerializeField]
    private Transform _secondPlayerCannon;
    
    [SerializeField]
    private Transform _firstPlayerCannonTarget;
    [SerializeField]
    private Transform _secondPlayerCannonTarget;

    [SerializeField] private GameEntities _gameEntities;

    [SerializeField] private PhysicsMover _shootingObject;
    private ObjectPool<PhysicsMover> _cannonBalls;

    [SerializeField] private float _shootingForce= 100f;
    
    private void Awake()
    {
        Instance = this;
        _cannonBalls = new(GenerateCannotBall);
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
        var instance = Instantiate(
            original: entitySO.Prefab,
            position: position,
            rotation,
            parent: teamID == 1 ? _teamOneParent : _teamTwoParent);

        _gameEntities.AddEntity(instance);
        
        instance.Init(teamID, position, rotation, entitySO)
            .Forget();
    }
    
    private PhysicsMover GenerateCannotBall()
    {
        return Instantiate(_shootingObject);
    }

    private void Reset()
    {
        _gameEntities = FindObjectOfType<GameEntities>();
        
    }
}