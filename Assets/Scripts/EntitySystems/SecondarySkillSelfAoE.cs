using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SecondarySkillSelfAoE : DamageSystem
{
  [Header("Secondary Skill Settings")]
  public float attackDamage;
  public float attackRange;
  public float attackDelay;
  public float attackCooldown;
  public float knockbackForce;
  public float knockbackDuration;

  private float lastAttackTime = -Mathf.Infinity;

  void Update()
  {
    float totalCooldown = attackDelay + attackCooldown;
    float progress = Mathf.Clamp((Time.time - lastAttackTime) / totalCooldown, 0, 1);
    mainEntity.OnSecondarySkillProgress?.Invoke(progress);
  }

  void SecondarySkillInputCallback()
  {
    if (Time.time < lastAttackTime + attackDelay + attackCooldown) return;

    mainEntity.OnAnimationTrigger?.Invoke("SecondarySkill");
    StartCoroutine(AttackSequenceCoroutine());
    lastAttackTime = Time.time;
    mainEntity.OnSecondarySkillCast?.Invoke();
  }

  IEnumerator AttackSequenceCoroutine()
  {
    yield return new WaitForSeconds(attackDelay);
    ApplyAoEDamageAndKnockback();
  }

  void ApplyAoEDamageAndKnockback()
  {
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

    foreach (Collider2D hitCollider in hitColliders)
    {
      if (hitCollider.gameObject == mainEntity.gameObject) continue;

      if (hitCollider.TryGetComponent<Entity>(out var entity) && TeamManager.IsOpponent(mainEntity, entity))
      {
        Vector2 direction = (entity.transform.position - mainEntity.transform.position).normalized;
        entity.OnKnockback?.Invoke(direction * knockbackForce, knockbackDuration);

        ApplyDamage(entity, attackDamage);
      }
    }
  }

  void HurtCallback(float _)
  {
    StopAllCoroutines();
  }

  protected override void OnEnable()
  {
    base.OnEnable();
    mainEntity.OnSecondarySkillInput += SecondarySkillInputCallback;
    mainEntity.OnHealthDamaged += HurtCallback;
  }

  protected override void OnDisable()
  {
    base.OnDisable();
    StopAllCoroutines();
    mainEntity.OnSecondarySkillInput -= SecondarySkillInputCallback;
    mainEntity.OnHealthDamaged -= HurtCallback;
  }
}