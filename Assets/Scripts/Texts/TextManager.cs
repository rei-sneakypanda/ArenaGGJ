using System;
using CardMaga.Tools.Pools;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; }
    private MBPool<TextObject> _interactionsPool;
    private MBPool<TextObject> _intervalInteractionsPool;
    
    
    [SerializeField] private TextObject _intervalInteractionPrefab;
    [SerializeField] private TextObject _interactionPrefab;
    

    [SerializeField] private float _delayTillIntervalObjectReturn = 1f;
    [SerializeField] private float _delayTillInteractionReturn = 1f;

    private void Awake()
    {
        Instance = this;
        _interactionsPool = new MBPool<TextObject>(_interactionPrefab, 10 , transform);
        _intervalInteractionsPool = new MBPool<TextObject>(_intervalInteractionPrefab, 10 , transform);
    }

    public async UniTask PlayIntervalScoreInteraction(GameEntity entity, int value, Vector3 position)
    {
        var instance = _intervalInteractionsPool.Pull();
        instance.transform.position = position;
        instance.SetText(entity.TeamId, value);
        instance.gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayTillIntervalObjectReturn), cancellationToken: destroyCancellationToken);
        if (instance != null && instance.gameObject != null)
        {
            instance.Dispose();
        }
    }
    
    public async UniTask PlayScoreInteraction(GameEntity entity, int value, Vector3 position)
    {
        var instance = _interactionsPool.Pull();
        instance.transform.position = position;
        instance.SetText(entity.TeamId, value);
        instance.gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayTillInteractionReturn), cancellationToken: destroyCancellationToken);
        if (instance != null && instance.gameObject != null)
        {
            instance.Dispose();
        }
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
}