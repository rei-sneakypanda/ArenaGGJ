using System;
using CardMaga.Tools.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextObject : MonoBehaviour, IPoolableMB<TextObject>
{
    public event Action<TextObject> OnDisposed;
    [SerializeField] private TextMeshPro _text;
    [FormerlySerializedAs("_team2Color"),SerializeField] private Color _teamBlueColor;
    [FormerlySerializedAs("_team1Color"),SerializeField] private Color _teamRedColor;
    public void SetText(TeamType teamID, int value)
    {
        if (value == 0)
        {
            return;
        }
        
        _text.color = teamID == TeamType.TeamRed ? _teamRedColor : _teamBlueColor;
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