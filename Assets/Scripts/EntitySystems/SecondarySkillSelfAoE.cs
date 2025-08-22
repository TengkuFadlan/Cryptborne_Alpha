using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SecondarySkillSelfAoE : EntitySystem
{
  [Header("Secondary Skill Settings")]
  public float attackDamage;
  public float attackRange;
  public float attackDelay;
  public float attackCooldown;
  public float knockbackForce;
  public float knockbackDuration;

  private bool isAttacking = false;

  // This method is called when the secondary skill input is received
  void SecondarySkillInputCallback()
  {
    if (isAttacking) return;

    // Trigger the animation for the skill
    mainEntity.OnAnimationTrigger?.Invoke("SecondarySkill");

    // Start the attack coroutine to handle delay, damage, and cooldown
    StartCoroutine(AttackSequenceCoroutine());
  }

  // Coroutine to handle the timed sequence of the attack
  IEnumerator AttackSequenceCoroutine()
  {
    isAttacking = true;

    // Wait for the specified delay before the AoE strike
    yield return new WaitForSeconds(attackDelay);

    // Find and damage all opponents within the attack range
    ApplyAoEDamageAndKnockback();

    // Wait for the cooldown to finish
    yield return new WaitForSeconds(attackCooldown);

    isAttacking = false;
  }

  // Applies damage and knockback to all valid targets in the area
  void ApplyAoEDamageAndKnockback()
  {
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

    foreach (Collider2D hitCollider in hitColliders)
    {
      if (hitCollider.gameObject == mainEntity.gameObject) continue;

      if (hitCollider.TryGetComponent<Entity>(out var entity) && TeamManager.IsOpponent(mainEntity, entity))
      {
        // Apply knockback
        Vector2 direction = (entity.transform.position - mainEntity.transform.position).normalized;
        entity.OnKnockback?.Invoke(direction * knockbackForce, knockbackDuration);

        // Damage the opponent
        entity.OnRecieveDamage?.Invoke(attackDamage);
      }
    }
  }

  // This method is called when the main entity gets hurt
  void HurtCallback(float _)
  {
    StopAllCoroutines();
    isAttacking = false;
  }

  // Subscribe to events when the script is enabled
  void OnEnable()
  {
    mainEntity.OnSecondarySkillInput += SecondarySkillInputCallback;
    mainEntity.OnHealthDamaged += HurtCallback;
  }

  // Unsubscribe from events when the script is disabled
  void OnDisable()
  {
    StopAllCoroutines();
    mainEntity.OnSecondarySkillInput -= SecondarySkillInputCallback;
    mainEntity.OnHealthDamaged -= HurtCallback;
  }
}