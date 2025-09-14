using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _zombiePrefab;
    [SerializeField] private Transform _spawnOrigin;
    public float spawnInterval = 1.5f;
    public int spawnCountPerTick = 1;   // 마리당 소환수
    public int maxZombieCount = 15;

    [SerializeField] private float _baseSpeed = 2.0f;
    [SerializeField] private Vector2 _speedJitter = new Vector2(-0.2f, 0.2f);   // 속도 노이즈 추가

    private List<GameObject> _aliveZombies = new List<GameObject>();

    CompositeDisposable _cd = new();

    void OnEnable()
    {
        Observable.Interval(TimeSpan.FromSeconds(spawnInterval))
        .Subscribe(_ => StartSpawn(spawnCountPerTick))
        .AddTo(_cd);
    }

    void OnDisable() => _cd.Clear();

    void OnDestroy() => _cd.Dispose();

    void StartSpawn(int count)
    {
        if (_aliveZombies.Count >= maxZombieCount) return;

        for (int i = 0; i < count; i++)
        {
            int lane = LaneService.GetRandomLane();
            SpawnZombie(lane);
        }
    }

    void SpawnZombie(int lane)
    {
        var zombieGO = Instantiate(_zombiePrefab, _spawnOrigin);
        var mc = zombieGO.GetComponent<MovementComponent>();
        if (mc == null)
        {
            mc = zombieGO.AddComponent<MovementComponent>();
        }

        mc.SetLane(lane);

        _aliveZombies.Add(zombieGO);

        mc.moveSpeed = _baseSpeed + UnityEngine.Random.Range(_speedJitter.x, _speedJitter.y);

        var pos = zombieGO.transform.position;
        pos.y = _spawnOrigin.transform.position.y + LaneService.laneY[lane];
        zombieGO.transform.position = pos;
    }
}
