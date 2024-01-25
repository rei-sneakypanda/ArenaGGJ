using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class GameEntity : SerializedMonoBehaviour
{
    private EntitySO _entitySO;
    private StatHandler _statHandler;

    public StatHandler StatHandler => _statHandler;
    public EntitySO EntitySO => _entitySO;
}

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private GameEntity _gameEntity;

    private void FixedUpdate()
    {
        throw new NotImplementedException();
    }
}