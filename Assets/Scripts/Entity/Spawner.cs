using System;
using Cysharp.Threading.Tasks;
using KinematicCharacterController;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Pool;

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
        var endSpawnPoint = teamID == 1 ? _firstPlayerCannonTarget : _secondPlayerCannonTarget;
        
        var cannonBall = _cannonBalls.Get();
        cannonBall.transform.position = currentSpawnPosition;
        cannonBall.Rigidbody.AddForce((endSpawnPoint.position - currentSpawnPosition).normalized * _shootingForce, ForceMode.Impulse);
        IDisposable disposable = null;
        
        disposable = cannonBall.GetComponent<Collider>().OnCollisionEnterAsObservable()
            .Subscribe(x =>
            {
                disposable?.Dispose();
                _cannonBalls.Release(cannonBall);
                Spawn(entitySO, teamID, currentSpawnPosition, Quaternion.identity)
                    .Forget();
            });
    }

    public async UniTask Spawn(EntitySO entitySO, int teamID, Vector3 position, Quaternion rotation)
    {
        var currentSpawnPosition = teamID == 1 ? _firstPlayerCannonTarget.position : _secondPlayerCannonTarget.position;
        
        var instance = Instantiate(
            original: entitySO.Prefab,
            currentSpawnPosition,
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