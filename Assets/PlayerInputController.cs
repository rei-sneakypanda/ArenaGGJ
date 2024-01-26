using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private Image _entityImage;

    private Player _player;
    
    public void Init(Player player)
    {
        _player = player;

        if (_player.TeamID == 1)
        {
            InputController.OnPlayerOneReroll += () => { _player.SetRandomEntity(); };
            InputController.OnPlayerOneSpawn += () =>
            {
                Spawner.Instance.Spawn(_player.CurrentEntity.Value, player.TeamID);
                _player.SetRandomEntity();
            };
        }
else
        {
            InputController.OnPlayerTwoReroll += () => { _player.SetRandomEntity(); };
            InputController.OnPlayerTwoSpawn += () =>
            {
                Spawner.Instance.Spawn(_player.CurrentEntity.Value, player.TeamID);
                _player.SetRandomEntity();
            };
        }
        
        _player.CurrentEntity.Subscribe(OnCurrentEntityChange);
    }

    private void OnCurrentEntityChange(EntitySO entitySo)
    {
        _entityImage.sprite = entitySo.Image;
    }
}
