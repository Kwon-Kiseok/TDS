using UnityEngine;
using UniRx;
using System;

// 전투 담당 컴포넌트
// 피격 
public class CombatComponent : MonoBehaviour
{
    readonly Subject<Unit> _onHitSubject = new Subject<Unit>();
    readonly Subject<Unit> _onAttackSubject = new Subject<Unit>();
    readonly Subject<Unit> _onDeadSubject = new Subject<Unit>();

    public IObservable<Unit> OnHit => _onHitSubject;
    public IObservable<Unit> OnAttack => _onAttackSubject;
    public IObservable<Unit> OnDead => _onDeadSubject;

    public void ApplyDamage(Character target, float damage)
    {
        if (target == null) return;

        _onHitSubject.OnNext(Unit.Default);
        target.TakeDamage(damage);

        if (!target.IsAlive)
        {
            _onDeadSubject.OnNext(Unit.Default);
        }
    }

    public void Attack()
    { 
        _onAttackSubject.OnNext(Unit.Default);
    }

    void OnDestroy()
    {
        _onHitSubject.OnCompleted();
        _onAttackSubject.OnCompleted();
        _onDeadSubject.OnCompleted();
        _onHitSubject.Dispose();
        _onAttackSubject.Dispose();
        _onDeadSubject.Dispose();
    }
}
