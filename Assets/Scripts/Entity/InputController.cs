using System;
using UnityEngine;
using UnityEngine.Serialization;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }
    public static event Action OnPlayerRedReroll;
    public static event Action OnPlayerBlueReroll;
    public static event Action OnPlayerRedSpawn;
    public static event Action OnPlayerBlueSpawn;

    [FormerlySerializedAs("_playerOneRerollKey"),SerializeField] KeyCode _playerRedRerollKey = KeyCode.W;
    [FormerlySerializedAs("_playerOneSpawnKey"),SerializeField] KeyCode _playerRedSpawnKey = KeyCode.E;

    [FormerlySerializedAs("_playerTwoRerollKey"),SerializeField] KeyCode _playerBlueRerollKey = KeyCode.U;
    [FormerlySerializedAs("_playerTwoSpawnKey"),SerializeField] KeyCode _playerBlueSpawnKey = KeyCode.I;

    private void Update()
    {
        if (Input.GetKeyDown(_playerRedRerollKey))
        {
            OnPlayerRedReroll?.Invoke();
        }

        if (Input.GetKeyDown(_playerRedSpawnKey))
        {
            OnPlayerRedSpawn?.Invoke();
        }

        if (Input.GetKeyDown(_playerBlueRerollKey))
        {
            OnPlayerBlueReroll?.Invoke();
        }

        if (Input.GetKeyDown(_playerBlueSpawnKey))
        {
            OnPlayerBlueSpawn?.Invoke();
        }
    }
}