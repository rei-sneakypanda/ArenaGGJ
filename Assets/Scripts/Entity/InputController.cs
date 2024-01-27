using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }
    public static event Action OnPlayerRedReroll;
    public static event Action OnPlayerBlueReroll;
    public static event Action OnPlayerRedSpawn;
    public static event Action OnPlayerBlueSpawn;

    [SerializeField] KeyCode _playerOneRerollKey = KeyCode.W;
    [SerializeField] KeyCode _playerOneSpawnKey = KeyCode.E;

    [SerializeField] KeyCode _playerTwoRerollKey = KeyCode.U;
    [SerializeField] KeyCode _playerTwoSpawnKey = KeyCode.I;

    private void Update()
    {
        if (Input.GetKeyDown(_playerOneRerollKey))
        {
            OnPlayerRedReroll?.Invoke();
        }

        if (Input.GetKeyDown(_playerOneSpawnKey))
        {
            OnPlayerRedSpawn?.Invoke();
        }

        if (Input.GetKeyDown(_playerTwoRerollKey))
        {
            OnPlayerBlueReroll?.Invoke();
        }

        if (Input.GetKeyDown(_playerTwoSpawnKey))
        {
            OnPlayerBlueSpawn?.Invoke();
        }
    }
}