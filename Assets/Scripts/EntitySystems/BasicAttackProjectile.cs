using UnityEngine;

public class BasicAttackProjectile : ProjectileAttackSystem
{
    protected override void SubscribeInputEvents()
    {
        mainEntity.OnBasicAttackInput += PerformAttack;
    }

    protected override void UnsubscribeInputEvents()
    {
        mainEntity.OnBasicAttackInput -= PerformAttack;
    }

    protected override void Awake()
    {
        base.Awake();
        animationTriggerName = "BasicAttack";
    }
}