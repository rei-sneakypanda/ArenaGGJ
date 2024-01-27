using UnityEngine;

public enum TeamType
{
    TeamRed = 1,
    TeamBlue = 2
}

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance { get; private set; }
    [SerializeField] private PlayerInputController _playerInputController1;
    [SerializeField] private PlayerInputController _playerInputController2;
    
    public Player PlayerOne;
    public Player PlayerTwo;


    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        _playerInputController1.Init(PlayerOne = new Player(TeamType.TeamRed));
        _playerInputController2.Init(PlayerTwo = new Player(TeamType.TeamBlue));
    }

    public void AddScore(TeamType teamID, int score)
    {
        (teamID == TeamType.TeamRed ? PlayerOne : PlayerTwo).AddScore(score);
    }
}