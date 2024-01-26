using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance { get; private set; }
    [SerializeField] private PlayerInputController _playerInputController1;
    [SerializeField] private PlayerInputController _playerInputController2;
    
    public Player PlayerOne;
    public Player PlayerTwo;

    private void Start()
    {
        Instance = this;
        _playerInputController1.Init(PlayerOne = new Player(1));
        _playerInputController2.Init(PlayerTwo = new Player(2));
    }
}
