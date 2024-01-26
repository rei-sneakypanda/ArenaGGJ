using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private Image _entityImage;
    [SerializeField] private Slider _slider;
    [SerializeField] private float _allowedRerollTime;
    [SerializeField] private Animator _animator;
    
    private float _currentRerollTime;
    private Player _player;
    
    public void Init(Player player)
    {
        _player = player;

        if (_player.TeamID == 1)
        {
            InputController.OnPlayerOneReroll += OnRerollRequested;
            InputController.OnPlayerOneSpawn += OnSpawnRequested;
        }
        else
        {
            InputController.OnPlayerTwoReroll += OnRerollRequested;
            InputController.OnPlayerTwoSpawn += OnSpawnRequested;
        }
        
        _player.CurrentEntity.Subscribe(OnCurrentEntityChange);
    }
    
    private void Update()
    {
        if (_player == null)
        {
            return;
        }
        
        _currentRerollTime += Time.deltaTime;
        _slider.value = Mathf.Clamp01(_currentRerollTime / _allowedRerollTime);
        
        _animator.SetBool("IsLocked", _currentRerollTime > _allowedRerollTime);
    }

    private void OnRerollRequested()
    {
        if (_currentRerollTime > _allowedRerollTime)
        {
            return;
        }
        
        _player.SetRandomEntity();
        _animator.SetTrigger("Reroll");
    }
    
    private void OnSpawnRequested()
    {
        Spawner.Instance.Spawn(_player.CurrentEntity.Value, _player.TeamID).Forget();
        _player.SetRandomEntity();
        _currentRerollTime = 0;
        _animator.SetTrigger("Reroll");
    }
    
    private void OnCurrentEntityChange(EntitySO entitySo)
    {
        _entityImage.sprite = entitySo.Image;
    }
}
