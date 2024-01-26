using System;
using Cysharp.Threading.Tasks;
using PhysicsCharacterController;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private GameEntity _gameEntity;
    [SerializeField] private CharacterManager _characterController;

    [SerializeField, MinMaxSlider(0, 5f)] private Vector2 _turningTime;

    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private float _rayDistance = 20f;
    
    [ShowInInspector,ReadOnly]
    private bool _isTurning;

    private IDisposable _disposable;
    private void Awake()
    {
        _characterController.SetLockRotation(true);
    }

    public void Init()
    {
        _disposable = _gameEntity.StatHandler[StatType.MovementSpeed].StatValue.Subscribe(SetSpeed);
    }
    
    private void OnDestroy()
    {
        _disposable?.Dispose();
    }

    private void SetSpeed(float val)
    {
        _characterController.movementSpeed = val;
    }

    public void AddVelocity(Vector3 dir)
    {
        _characterController.AddVelocity(dir);
    }    
    private void Update()
    {
        if (_isTurning)
        {
            return;
        }
        
        if (!HasGroundAhead())
        {
            Turn()
                .Forget();
            return;
        }
        
        Move(transform.forward);
    }

    private void Move(Vector3 axis)
    {
        _characterController.AxisInput = new Vector2(axis.x,axis.z);
    }

    private void OnDrawGizmosSelected()
    {
        var transform1 = transform;
        Gizmos.color = HasGroundAhead() ? Color.green : Color.red;
        Gizmos.DrawRay(transform1.position + Vector3.up, (transform1.forward - transform1.up) * _rayDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform1.position, transform1.position +new Vector3(transform1.forward.x, 0, transform1.forward.z));
    }

    private async UniTask Turn()
    {
        _isTurning = true;
        float counter = 0;
        float max = Random.Range(_turningTime.x, _turningTime.y);

        float hasGroundAheadCounter = 0; 
float hasGroundAheadCheckDuration = 1f;
        
        
        var startValue = transform.forward;
      
        Quaternion rotation = Quaternion.AngleAxis( Random.Range(-30f, 30f),Vector3.up);
        Vector3  endValue = rotation * (-startValue);
        bool previousHasGroundAhead = false;

        while (!this.destroyCancellationToken.IsCancellationRequested &&
               (counter < max || (!previousHasGroundAhead && hasGroundAheadCounter >= hasGroundAheadCheckDuration)))

        {
            if (gameObject == null || this == null)
            {
                return;
            }

            var hasGroundAhead = HasGroundAhead();

            hasGroundAheadCounter =
                hasGroundAhead && previousHasGroundAhead ? hasGroundAheadCounter + Time.deltaTime : 0;

            previousHasGroundAhead = hasGroundAhead;

            counter += Time.deltaTime;

            var axis = Vector3.Slerp(startValue, endValue, counter / max);
            Move(axis);

            await UniTask.Yield();
        }

        _isTurning = false;
    }

    private bool HasGroundAhead()
    {
        var transform1 = transform;
        var direction = (transform1.forward * 2) - transform1.up;
        return Physics.Raycast(transform1.position + Vector3.up, direction, out var hit, _rayDistance,
            layerMask: _layerMask);
    }
    private void Reset()
    {
        _characterController = GetComponent<CharacterManager>();
    }
}