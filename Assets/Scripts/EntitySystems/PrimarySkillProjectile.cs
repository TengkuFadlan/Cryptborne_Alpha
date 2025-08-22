using UnityEngine;

public class PrimarySkillProjectile : ProjectileAttackSystem
{
  protected override void SubscribeInputEvents()
  {
    mainEntity.OnPrimarySkillInput += PerformAttack;
  }

  protected override void UnsubscribeInputEvents()
  {
    mainEntity.OnPrimarySkillInput -= PerformAttack;
  }

  protected override void Awake()
  {
    base.Awake();
    animationTriggerName = "PrimarySkill";
  }
}