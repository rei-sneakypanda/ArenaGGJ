using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class GameEntity : SerializedMonoBehaviour
{
    private int _teamId;

    public int TeamId
    {
        get => _teamId;
        set
        {
            if (value != _teamId)
            {
                _teamId = value;
                InitMaterials();
            }
        }
    }

    [SerializeField] private EntityAnimator entityAnimator;

    private EntitySO _entitySO;
    [ShowInInspector, ReadOnly] private StatHandler _statHandler = new();
    [SerializeField] private MovementHandler _movementHandler;
    [SerializeField] private DestroyHandler _destroyHandler;
    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private Transform _forwardTransform;
    [SerializeField] private Material _teamOneMat;
    [SerializeField] private Material _teamTwoMat;

    public SkinnedMeshRenderer[] SkinnedMeshRenderers;

    [OdinSerialize, HideInInspector] private List<TimeLoopInteraction> _intervalInteractionSelf;
    public Transform ForwardTransform => _forwardTransform ? _forwardTransform : transform;
    public Transform SpawnLocation => _spawnLocation ? _spawnLocation : transform;
    public MovementHandler MovementHandler => _movementHandler;
    public EntityAnimator EntityAnimator => entityAnimator;
    public DestroyHandler DestroyHandler => _destroyHandler;
    public InteractingContainer InteractingObjects;
    public StatHandler StatHandler => _statHandler;
    public EntitySO EntitySO => _entitySO;
    public bool IsAlive { get; private set; }

    private void Awake()
    {
        DestroyHandler.OnDisposed += Dispose;
    }


    
    private void Update()
    {
        if (_intervalInteractionSelf == null || _intervalInteractionSelf.Count == 0 || !IsAlive)
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

    [Button]
    private void GetAllSkinnedMaterials()
    {
        SkinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>().ToArray();
    }

    private void InitMaterials()
    {
        if (SkinnedMeshRenderers == null)
        {
            return;
        }

        var mat = TeamId == 1 ? _teamOneMat : _teamTwoMat;

        foreach (var sMR in SkinnedMeshRenderers)
        {

            if (sMR == null)
            {
                continue;
            }

            sMR.material = mat;
        }
    }

    public void SetEntitySO(EntitySO entitySO)
    {
        _entitySO = entitySO;
    }
    
    public async UniTask Init(int teamID, Vector3 position, Quaternion rotation, EntitySO entitySO)
    {
        TeamId = teamID;
        SetEntitySO(entitySO);
        _statHandler = new StatHandler(entitySO.Stats.ToArray());
        _intervalInteractionSelf = entitySO.IntervalInteractionSelf.Select(x => new TimeLoopInteraction(x)).ToList();
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
        _destroyHandler.Init();
        _movementHandler.Init();
        InteractingObjects.BlockForDuration(entitySO.TimeTillCanInteract)
            .Forget();

        IsAlive = true;

        try
        {
            if (_entitySO.SpawnInteractionSelf == null)
            {
                return;
            }

            foreach (var interaction in _entitySO.SpawnInteractionSelf)
            {
                await interaction.Interact(this);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    
    public void Dispose(GameEntity self = null)
    {
        _movementHandler.Dispose();
        _destroyHandler.Dispose();
    }

    private void OnDestroy()
    {
        Dispose();
        DestroyHandler.OnDisposed -= Dispose;
    }
}