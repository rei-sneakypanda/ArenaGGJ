using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    public static event Action GameEnded;
    public static GameTime Instance { get; private set; }
    [SerializeField] public List<float> _timeToDrop;
    [SerializeField] public List<Animator> _groundObjects;
    [SerializeField] private float _timeTillGameEnd;
    [SerializeField] private GameHandler _gameHandler;
    [ShowInInspector] [Sirenix.OdinInspector.ReadOnly]
    private float _remainingTime = new();
    
    private void Awake()
    {
        Instance = this;

        _gameHandler.OnGameStarted += GameStartedHandler;
    }

    private void Start()
    {
        _remainingTime = _timeTillGameEnd;
        _timerText.SetText(TimeSpan.FromSeconds((int)_remainingTime).GetTimeSpanString(useTwoDigit: true, useLetter: false));
    }

    private void GameStartedHandler()
    {
        StartCountDown()
            .Forget();
    }

    private async UniTask StartCountDown()
    {
        while (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;

            if (_timeToDrop.Count > 0)
            {
                if (_remainingTime <= _timeToDrop.First())
                {
                    _timeToDrop.RemoveAt(0);

                    var groundObjectIndex = Random.Range(0, _groundObjects.Count - 1);
                    var ground = _groundObjects.ElementAt(groundObjectIndex);
                    _groundObjects.RemoveAt(groundObjectIndex);
                    ground.enabled = true;
                    DestroyFallingObject(ground)
                        .Forget();
                }
            }
            _timerText.SetText(TimeSpan.FromSeconds((int)_remainingTime).GetTimeSpanString(useTwoDigit: true, useLetter: false));
            await UniTask.DelayFrame(1, cancellationToken: destroyCancellationToken);
        }

        _remainingTime = 0;
        _timerText.SetText(TimeSpan.FromSeconds((int)_remainingTime).GetTimeSpanString(useTwoDigit: true, useLetter: false));
        GameEnded?.Invoke();
    }

    private async UniTask DestroyFallingObject(Animator animator)
    {
        while (animator != null
               && animator.gameObject != null)
        {
            var distance = Vector3.Distance(animator.transform.position, Vector3.zero);

            if (distance > 50f)
            {
                Destroy(animator.gameObject);
                break;
            }

            await UniTask.Yield();
        }
    }

    private void OnDestroy()
    {
        _gameHandler.OnGameStarted -= GameStartedHandler;
        Instance = null;
    }
}

public static class TimeSpanExtensions
{
    private static readonly List<(Func<TimeSpan, int> Func, string Letter)> TimespanToTemplate = new()
    {
        (time => time.Seconds, "s"),
        (time => time.Minutes, "m"),
        (time => time.Hours, "h"),
        (time => time.Days, "d")
    };

    public static string GetTimeSpanString(
        this TimeSpan timespan,
        int minimumVisualItemCount = 2,
        int maxVisualItemCount = 3,
        bool useLetter = true,
        bool useTwoDigit = false,
        bool hideZeroValues = false)
    {
        var sign = "";
        if (timespan < TimeSpan.Zero)
        {
            timespan = timespan.Duration();
            sign = "-";
        }
            
        var answer = new StringBuilder(sign);
        int addedAmount = 0;

        for (int i = TimespanToTemplate.Count - 1; i >= 0 && addedAmount < maxVisualItemCount; i--)
        {
            bool showItem = i < minimumVisualItemCount || addedAmount > 0;

            int timeAmount = TimespanToTemplate[i].Func(timespan);
            var letter = TimespanToTemplate[i].Letter;

            if (showItem || timeAmount > 0)
            {
                addedAmount++;
                var isLast = i == 0 || addedAmount == maxVisualItemCount;
                var suffix = useLetter ? letter : (!isLast ? ":" : "");
                if (hideZeroValues && timeAmount == 0)
                {
                    continue;
                }
                    
                answer.Append(string.Format((useTwoDigit ? "{0:D2}" : "{0}") + suffix, timeAmount));
            }
        }

        return answer.ToString();
    }
}