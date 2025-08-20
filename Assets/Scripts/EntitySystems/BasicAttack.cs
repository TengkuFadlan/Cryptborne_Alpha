using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BasicAttack : EntitySystem
{
  [Header("Attack Settings")]
  public float[] attackDamageIndex;
  public float[] attackTimeIndex;
  public float[] attackRangeIndex;
  public float attackEndDuration;
  int attackIndex;

  Entity currentTarget;
  bool isAttacking = false;

  Entity FindClosestValidTarget()
  {
    GameObject[] entityGameObjects = GameObject.FindGameObjectsWithTag("Entity");
    Entity closestEntity = null;
    float closestDist = float.MaxValue;

    foreach (GameObject entityGameObject in entityGameObjects)
    {
      if (entityGameObject == mainEntity.gameObject) continue; // Skip self

      if (!entityGameObject.TryGetComponent<Entity>(out var entity)) continue;
      if (!TeamManager.IsOpponent(mainEntity, entity)) continue;

      float dist = Vector2.Distance(transform.position, entityGameObject.transform.position);
      if (dist < closestDist && dist <= attackRangeIndex[0]) // Use first range for targeting
      {
        closestDist = dist;
        closestEntity = entity;
      }
    }
    return closestEntity;
  }

  void BasicAttackInputCallback()
  {
    if (isAttacking) return; // Prevent overlapping attacks

    currentTarget = FindClosestValidTarget();

    if (currentTarget != null)
    {
      Vector2 origin = transform.position;
      Vector2 target = currentTarget.transform.position;
      Vector2 direction = (target - origin).normalized;

      mainEntity.OnFocusAnimationTrigger?.Invoke("BasicAttack", direction);
    }
    else
    {
      mainEntity.OnFocusAnimationTrigger?.Invoke("BasicAttack", mainEntity.LastMovementDirection);
    }

    StartCoroutine(AttackSequenceCoroutine());
  }

  IEnumerator AttackSequenceCoroutine()
  {
    isAttacking = true;

    for (attackIndex = 0; attackIndex < attackDamageIndex.Length; attackIndex++)
    {
      float delay = attackTimeIndex[attackIndex];
      yield return new WaitForSeconds(delay);

      currentTarget = FindClosestValidTarget();

      if (currentTarget != null && !currentTarget.Dead)
      {
        float dist = Vector2.Distance(transform.position, currentTarget.transform.position);
        if (dist <= attackRangeIndex[attackIndex])
        {
          currentTarget.OnRecieveDamage?.Invoke(attackDamageIndex[attackIndex]);
        }
      }
    }

    yield return new WaitForSeconds(attackEndDuration);

    attackIndex = 0; // Reset for next attack input
    isAttacking = false;
  }

  void OnEnable()
  {
    mainEntity.OnBasicAttackInput += BasicAttackInputCallback;
  }

  void OnDisable()
  {
    mainEntity.OnBasicAttackInput -= BasicAttackInputCallback;
  }
}