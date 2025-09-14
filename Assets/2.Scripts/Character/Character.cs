using UnityEngine;
using UniRx;
using System;

public class Character : MonoBehaviour
{
    [Serializable]
    public struct StatData
    {
        public string name;
        public float hp;
        public float atk;
        public float def;
        public float speed;

        public StatData(string name, float hp, float atk, float def, float speed)
        {
            this.name = name;
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.speed = speed;
        }
    }

    [SerializeField] private StatData _statData;
    public float CurrentHp => _statData.hp;

    private bool _isAlive = true;
    public bool IsAlive => _isAlive;

    public StatData GetStatData()
    {
        return _statData;
    }

    public void TakeDamage(float damage)
    {
        if (!_isAlive) return;

        _statData.hp = Mathf.Max(0, _statData.hp - damage);
        if (_statData.hp <= 0f && _isAlive)
        {
            _isAlive = false;
        }
    }
    
}
