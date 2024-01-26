using UnityEngine;

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
        _playerInputController1.Init(PlayerOne = new Player(1));
        _playerInputController2.Init(PlayerTwo = new Player(2));
    }

    public void AddScore(int teamID, int score)
    {
        (teamID == 1 ? PlayerOne : PlayerTwo).AddScore(score);
    }
}