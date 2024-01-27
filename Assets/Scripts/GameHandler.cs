using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum GameStateType
{
    PreGame,
    Game,
    PostGame
}


public class GameHandler : MonoBehaviour
{
   public GameStateType CurrentGameState { get; private set; }
    
    public event Action OnGameStarted;
    [SerializeField] private GameObject _startScreen;
    
    
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private GameObject[] _redWinScreen;
    [SerializeField] private GameObject[] _blueWinScreen;
    
    
    private void Awake()
    {
        CurrentGameState = GameStateType.PreGame;
        
        InputController.OnGameStart += HandleGameState;
        GameTime.GameEnded += ShowGameOver;
    }

    private void HandleGameState()
    {
        if (CurrentGameState == GameStateType.Game)
        {
            return;
        }

        if (CurrentGameState == GameStateType.PreGame)
        {
            CurrentGameState = GameStateType.Game;
            _startScreen.SetActive(false);
            OnGameStarted?.Invoke();
         return;
        }

        ApplicationManager.Instance?.Restart()
            .Forget();
    }

    private void ShowGameOver()
    {
        ShowGameOverTask().Forget();
    }
    
    public async UniTask ShowGameOverTask()
    {
        var winnerScreen = PlayersManager.Instance.GetWinner() == TeamType.TeamRed ? _redWinScreen : _blueWinScreen;

        foreach (var winnerObject in winnerScreen)
        {
            winnerObject.SetActive(true);
        }
        
        _endScreen.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(.15f));
        CurrentGameState = GameStateType.PostGame;
    }

    private void OnDestroy()
    {
        GameTime.GameEnded -= ShowGameOver;
        InputController.OnGameStart -= HandleGameState;
    }
}
