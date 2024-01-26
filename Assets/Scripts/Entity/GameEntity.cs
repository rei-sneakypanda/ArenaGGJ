using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameEntity : SerializedMonoBehaviour
{
    public int TeamId { get; set; }
    [SerializeField] private EntityAnimator entityAnimator;
    
    private EntitySO _entitySO;
    private StatHandler _statHandler = new();
    [SerializeField] private MovementHandler _movementHandler;
    [SerializeField] private DestroyHandler _destroyHandler;
    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private Transform _forwardTransform;
    [SerializeField] private List<IInteractionOnSelf> _spawnInteractionSelf;
    public Transform ForwardTransform => _forwardTransform ? _forwardTransform : transform;
    public Transform SpawnLocation => _spawnLocation ? _spawnLocation : transform;
    public MovementHandler MovementHandler  => _movementHandler;
    public DestroyHandler DestroyHandler => _destroyHandler;
    public InteractingContainer InteractingObjects;
    public StatHandler StatHandler => _statHandler;
    public EntitySO EntitySO => _entitySO;
    
    
    public async UniTask Init(int teamID, Vector3 position, Quaternion rotation, EntitySO entitySO)
    {
        TeamId = teamID;
        _entitySO = entitySO;
        _statHandler = new StatHandler(entitySO.Stats.ToArray());
        
        transform.position = position;
        transform.rotation = rotation;
        
        _movementHandler.Init();
        InteractingObjects.BlockForDuration(entitySO.TimeTillCanInteract)
            .Forget();
        await entityAnimator.PlaySpawnAnimation();
        
        try
        {
            foreach (var interaction in _spawnInteractionSelf)
            {
                await interaction.Interact(this);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}