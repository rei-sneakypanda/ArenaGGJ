using System;
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
    
    [SerializeField] private AudioSource _redSpawnAudioSource;
    [SerializeField] private AudioSource _blueSpawnAudioSource;
    [SerializeField] private AudioSource _rerollAudioSource;
    public void Init(Player player)
    {
        _player = player;

        if (_player.TeamID == TeamType.TeamRed)
        {
            InputController.OnPlayerRedReroll += OnRerollRequested;
            InputController.OnPlayerRedSpawn += OnSpawnRequested;
        }
        else
        {
            InputController.OnPlayerBlueReroll += OnRerollRequested;
            InputController.OnPlayerBlueSpawn += OnSpawnRequested;
        }
        
        _player.CurrentEntity.Subscribe(OnCurrentEntityChange);
    }

    private void OnDestroy()
    {
        if (_player.TeamID == TeamType.TeamRed)
        {
            InputController.OnPlayerRedReroll -= OnRerollRequested;
            InputController.OnPlayerRedSpawn  -= OnSpawnRequested;
        }
        else
        {
            InputController.OnPlayerBlueReroll-= OnRerollRequested;
            InputController.OnPlayerBlueSpawn -=OnSpawnRequested;
        }
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
        
        _rerollAudioSource.Play();
        _player.SetRandomEntity();
        _animator.SetTrigger("Reroll");
    }

    private void OnSpawnRequested()
    {
        if (_currentRerollTime < _allowedRerollTime)
        {
            return;
        }
        
        if ( _player.TeamID == TeamType.TeamRed)
        {
            _redSpawnAudioSource.Play();
        }
        else
        {
            _blueSpawnAudioSource.Play();
        }
        
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
