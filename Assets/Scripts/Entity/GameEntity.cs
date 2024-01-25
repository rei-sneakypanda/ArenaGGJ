using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameEntity : SerializedBehaviour
{
    public EntitySO EntitySO;
    public StatHandler StatHandler;
    
}

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private GameEntity _gameEntity;
    
    
}