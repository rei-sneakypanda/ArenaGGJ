using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }
    public static event Action OnPlayerOneReroll;
    public static event Action OnPlayerTwoReroll;
    public static event Action OnPlayerOneSpawn;
    public static event Action OnPlayerTwoSpawn;

    [SerializeField] KeyCode _playerOneRerollKey = KeyCode.W;
    [SerializeField] KeyCode _playerOneSpawnKey = KeyCode.E;

    [SerializeField] KeyCode _playerTwoRerollKey = KeyCode.U;
    [SerializeField] KeyCode _playerTwoSpawnKey = KeyCode.I;

    private void Update()
    {
        if (Input.GetKeyDown(_playerOneRerollKey))
        {
            OnPlayerOneReroll?.Invoke();
        }

        if (Input.GetKeyDown(_playerOneSpawnKey))
        {
            OnPlayerOneSpawn?.Invoke();
        }

        if (Input.GetKeyDown(_playerTwoRerollKey))
        {
            OnPlayerTwoReroll?.Invoke();
        }

        if (Input.GetKeyDown(_playerTwoSpawnKey))
        {
            OnPlayerTwoSpawn?.Invoke();
        }
    }
}