using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BasicAttack : DamageSystem // Inherits from DamageSystem
{
  [Header("Attack Settings")]
  public float[] attackDamageIndex;
  public float[] attackTimeIndex;
  public float[] attackRangeIndex;
  public float attackEndDuration;
  public float knockbackForce;
  public float knockbackDuration;
  int attackIndex;

  Entity currentTarget;
  private float lastAttackTime = -Mathf.Infinity;
  private float totalCooldown;

  protected override void Awake()
  {
    base.Awake();
    float totalAttackDuration = 0f;
    foreach (float attackTime in attackTimeIndex)
    {
      totalAttackDuration += attackTime;
    }
    totalCooldown = totalAttackDuration + attackEndDuration;
  }

  void Update()
  {
    float progress = Mathf.Clamp((Time.time - lastAttackTime) / totalCooldown, 0, 1);
    mainEntity.OnBasicAttackProgress?.Invoke(progress);
  }

  Entity FindClosestValidTarget()
  {
    GameObject[] entityGameObjects = GameObject.FindGameObjectsWithTag("Entity");
    Entity closestEntity = null;
    float closestDist = float.MaxValue;

    foreach (GameObject entityGameObject in entityGameObjects)
    {
      if (entityGameObject == mainEntity.gameObject) continue;

      if (!entityGameObject.TryGetComponent<Entity>(out var entity)) continue;
      if (entity.Dead) continue;
      if (!TeamManager.IsOpponent(mainEntity, entity)) continue;

      float dist = Vector2.Distance(transform.position, entityGameObject.transform.position);
      if (dist < closestDist && dist <= attackRangeIndex[0])
      {
        closestDist = dist;
        closestEntity = entity;
      }
    }
    return closestEntity;
  }

  void BasicAttackInputCallback()
  {
    if (Time.time < lastAttackTime + totalCooldown) return;

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
    lastAttackTime = Time.time;
  }

  IEnumerator AttackSequenceCoroutine()
  {
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
          if (!currentTarget.IsInvulnerable)
          {
            Vector2 direction = (currentTarget.transform.position - mainEntity.transform.position).normalized;
            currentTarget.OnKnockback?.Invoke(direction * knockbackForce, knockbackDuration);
          }

          // Apply modified damage
          ApplyDamage(currentTarget, attackDamageIndex[attackIndex]);
        }
      }

      mainEntity.OnBasicAttackCast?.Invoke();
    }

    yield return new WaitForSeconds(attackEndDuration);

    attackIndex = 0;
  }

  void HurtCallback(float _)
  {
    StopAllCoroutines();
    attackIndex = 0;
  }

  protected override void OnEnable()
  {
    base.OnEnable();
    mainEntity.OnBasicAttackInput += BasicAttackInputCallback;
    mainEntity.OnHealthDamaged += HurtCallback;
    attackIndex = 0;
  }

  protected override void OnDisable()
  {
    base.OnDisable();
    StopAllCoroutines();
    mainEntity.OnBasicAttackInput -= BasicAttackInputCallback;
    mainEntity.OnHealthDamaged -= HurtCallback;
  }
}