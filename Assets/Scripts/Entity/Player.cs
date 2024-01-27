using System.Linq;
using UniRx;
using UnityEngine;

public class Player
{
    private ReactiveProperty<int> _playerScore = new(0);
    private TeamType _teamID;
    private ReactiveProperty<EntitySO> _currentEntity = new();
    public IReadOnlyReactiveProperty<EntitySO> CurrentEntity => _currentEntity;
    public IReadOnlyReactiveProperty<int> PlayerScore => _playerScore;
    
    public TeamType TeamID => _teamID;
    
    public Player(TeamType id)
    {
        _teamID = id;
        SetRandomEntity();
    }

    public void AddScore(int amount)
    {
        _playerScore.Value += amount;
    }
    
    public void SetRandomEntity()
    {
        _currentEntity.Value = GameEntities.Instance.AllPossibleEntities
            .Except(_currentEntity.Value == null ? Enumerable.Empty<EntitySO>() : new []{ _currentEntity.Value})
            .OrderBy(_ => Random.value)
            .First();
    }
}