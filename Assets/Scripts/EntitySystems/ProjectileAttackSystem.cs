using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ProjectileAttackSystem : DamageSystem // Inherit from DamageSystem
{
  [Header("Projectile Attack Settings")]
  public float attackRange;
  public float attackDelay;
  public float attackCooldown;
  public float projectileLifetime;
  public GameObject projectilePrefab;
  protected string animationTriggerName;

  // Use a float to store the timestamp of the last attack
  private float lastAttackTime = 0f;

  void Update()
  {
    float totalCooldown = attackDelay + attackCooldown;
    float progress = Mathf.Clamp((Time.time - lastAttackTime) / totalCooldown, 0, 1);
    if (animationTriggerName == "BasicAttack")
      mainEntity.OnBasicAttackProgress?.Invoke(progress);
    if (animationTriggerName == "PrimarySkill")
      mainEntity.OnPrimarySkillProgress?.Invoke(progress);
  }

  // This is the common method that handles the attack logic
  protected virtual void PerformAttack()
  {
    // Check if enough time has passed since the last attack
    if (Time.time < lastAttackTime + attackCooldown + attackDelay)
    {
      return;
    }

    Entity target = FindClosestValidTarget();

    if (target != null)
    {
      Vector2 origin = transform.position;
      Vector2 targetPos = target.transform.position;
      Vector2 direction = (targetPos - origin).normalized;

      mainEntity.OnFocusAnimationTrigger?.Invoke(animationTriggerName, direction);

      // Update the timestamp of the last attack
      lastAttackTime = Time.time;

      StartCoroutine(AttackCoroutine());

      if (animationTriggerName == "BasicAttack")
        mainEntity.OnBasicAttackCast?.Invoke();
      if (animationTriggerName == "PrimarySkill")
        mainEntity.OnPrimarySkillCast?.Invoke();
    }
  }

  protected Entity FindClosestValidTarget()
  {
    GameObject[] entityGameObjects = GameObject.FindGameObjectsWithTag("Entity");
    Entity closestEntity = null;
    float closestDist = float.MaxValue;

    foreach (GameObject entityGameObject in entityGameObjects)
    {
      if (entityGameObject == mainEntity.gameObject) continue;

      if (!entityGameObject.TryGetComponent<Entity>(out var entity)) continue;
      if (entity.Dead || entity.Projectile) continue;
      if (!TeamManager.IsOpponent(mainEntity, entity)) continue;

      float dist = Vector2.Distance(transform.position, entityGameObject.transform.position);
      if (dist < closestDist && dist <= attackRange)
      {
        closestDist = dist;
        closestEntity = entity;
      }
    }
    return closestEntity;
  }

  protected IEnumerator AttackCoroutine()
  {
    yield return new WaitForSeconds(attackDelay);

    Entity target = FindClosestValidTarget();
    if (target != null)
    {
      // Instantiate and configure the projectile
      GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
      Destroy(projectileObject, projectileLifetime);
      Entity projectileEntity = projectileObject.GetComponent<Entity>();

      // Pass the damage modifiers to the projectile's Entity individually
      foreach (var modifier in damageModifiers)
      {
        projectileEntity.OnDamagePercentModifierAdded?.Invoke(modifier.Key, modifier.Value);
      }

      Vector2 targetPos = target.transform.position;

      // Calculate the direction and angle
      Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      projectileEntity.team = mainEntity.team;
      projectileEntity.OnMovementInput?.Invoke(direction);

      // Rotate the projectile to face the target
      projectileObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
  }

  protected virtual void HurtCallback(float _)
  {
    StopAllCoroutines();
  }

  protected abstract void SubscribeInputEvents();
  protected abstract void UnsubscribeInputEvents();

  protected override void OnEnable()
  {
    base.OnEnable(); // Call the base class OnEnable
    SubscribeInputEvents();
    mainEntity.OnHealthDamaged += HurtCallback;
  }

  protected override void OnDisable()
  {
    base.OnDisable(); // Call the base class OnDisable
    StopAllCoroutines();
    UnsubscribeInputEvents();
    mainEntity.OnHealthDamaged -= HurtCallback;
  }
}