using System;
using CardMaga.Tools.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextObject : MonoBehaviour, IPoolableMB<TextObject>
{
    public event Action<TextObject> OnDisposed;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private Color _team2Color;
    [SerializeField] private Color _team1Color;
    public  void SetText(int teamID, int value)
    {
        _text.color = teamID == 1 ? _team1Color : _team2Color;
        var s = value >= 0 ? "+" : "";
        s = string.Concat(s, value);
        _text.SetText(s);
    }

    public void Dispose()
    {
        OnDisposed?.Invoke(this);
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
    }
}