using UniRx;
using UnityEngine;

public enum AttackType
{
    Melee,
    Range
}

public class Zombie : Character
{
    [SerializeField] private AttackType _attackType;
    [SerializeField] private CombatComponent _combatComponent;

    void Start()
    {
        _combatComponent.OnHit.ObserveOnMainThread().Subscribe(_ =>
        {
            // hit 리액션
        }).AddTo(this);

        _combatComponent.OnDead.Take(1).ObserveOnMainThread().Subscribe(_ =>
        {
            // dead 리액션 및 사망처리
        }).AddTo(this);
    }

    // 외부에서 공격 받을 시 호출
    public void ReceiveAttack(float damage)
    {
        _combatComponent.ApplyDamage(this, damage);
    }
}
