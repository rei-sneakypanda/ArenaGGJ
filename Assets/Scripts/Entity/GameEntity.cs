using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
    [OdinSerialize] private List<TimeLoopInteraction> _intervalInteractionSelf;
    public Transform ForwardTransform => _forwardTransform ? _forwardTransform : transform;
    public Transform SpawnLocation => _spawnLocation ? _spawnLocation : transform;
    public MovementHandler MovementHandler  => _movementHandler;
    public EntityAnimator EntityAnimator => entityAnimator;
    public DestroyHandler DestroyHandler => _destroyHandler;
    public InteractingContainer InteractingObjects;
    public StatHandler StatHandler => _statHandler;
    public EntitySO EntitySO => _entitySO;

    private void Update()
    {
        if (_intervalInteractionSelf == null)
        {
            return;
        }

        try
        {
            var delatTime = Time.deltaTime;
            foreach (var interaction in _intervalInteractionSelf)
            {
                interaction.Tick(delatTime, this)
                    .Forget();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async UniTask Init(int teamID, Vector3 position, Quaternion rotation, EntitySO entitySO)
    {
        TeamId = teamID;
        _entitySO = entitySO;
        _statHandler = new StatHandler(entitySO.Stats.ToArray());
        
        transform.position = position;
        transform.rotation = rotation;
        _destroyHandler.Init();
        _movementHandler.Init();
        InteractingObjects.BlockForDuration(entitySO.TimeTillCanInteract)
            .Forget();
        
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