using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Midbaryom.Pool
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;
        private List<GameEntity> _activeEntities;
        private List<GameEntity> _notActiveEntities;
        [SerializeField] private List<PoolStaterPack> _poolStaterPacks;

        public static PoolManager Instance
        {
            get { return _instance; }
        }

        public IReadOnlyList<GameEntity> ActiveEntities => _activeEntities;

        private void Awake()
        {
            _instance = this;
        
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _activeEntities = new List<GameEntity>();
            _notActiveEntities = new List<GameEntity>();

            if (_poolStaterPacks != null)
            {
                for (int i = 0; i < _poolStaterPacks.Count; i++)
                {
                    PoolStaterPack poolStaterPack = _poolStaterPacks[i];
                    int amount = poolStaterPack.Amount;

                    for (int j = 0; j < amount; j++)
                    {
                        var entity = InstantiateEntity(poolStaterPack.EntityTagSO,Vector3.zero , quaternion.identity);
                        entity.DestroyHandler.ReturnToPool();
                    }
                }

                ReturnAllBack();
            }
        }

        public GameEntity Pull(EntitySO tagSO, Vector3 position, Quaternion rotation)
        {
            GameEntity entity = null;
            for (int i = 0; i < _notActiveEntities.Count; i++)
            {
                if (_notActiveEntities[i].EntitySO == tagSO)
                {
                    entity = _notActiveEntities[i];
                    break;
                }
            }

            if (entity == null)
                entity = InstantiateEntity(tagSO, position, rotation);
            else
                _notActiveEntities.Remove(entity);

            _activeEntities.Add(entity);
            return entity;
        }

        private GameEntity InstantiateEntity(EntitySO tagSO, Vector3 position, Quaternion rotation)
        {
            var cache = Instantiate(tagSO.Prefab, position, rotation, transform);
            cache.SetEntitySO(tagSO);
            cache.DestroyHandler.OnDisposed += Return;
            return cache;
        }

        public  void Return(GameEntity obj)
        {
            _activeEntities.Remove(obj);
            _notActiveEntities.Add(obj);

            if (obj.gameObject == null)
            {
                return;
            }
            obj.transform.position = Vector3.zero;
            obj.transform.gameObject.SetActive(false);
        }

        public void ReturnAllBack()
        {
            for (int i = 0; i < ActiveEntities.Count; i++)
            {
                Return(ActiveEntities[i]);
            }
        }

        private void OnDestroy()
        {

            foreach (var entity in DestroyHandlers())
                entity.OnDisposed -= Return;

            IEnumerable<DestroyHandler> DestroyHandlers()
            {
                if (ActiveEntities != null && ActiveEntities.Count > 0)
                    for (int i = 0; i < ActiveEntities.Count; i++)
                        yield return ActiveEntities[i].DestroyHandler;

                if (_notActiveEntities != null && _notActiveEntities.Count > 0)
                    for (int i = 0; i < _notActiveEntities.Count; i++)
                        yield return _notActiveEntities[i].DestroyHandler;

            }
        }
    }
}