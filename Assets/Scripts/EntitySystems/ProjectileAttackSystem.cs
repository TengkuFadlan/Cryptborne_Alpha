using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class ProjectileAttackSystem : EntitySystem
{
  [Header("Projectile Attack Settings")]
  public float attackRange;
  public float attackDelay;
  public float attackCooldown;
  public float projectileLifetime;
  public GameObject projectilePrefab;
  protected string animationTriggerName;

  protected bool isAttacking = false;

  // This is the common method that handles the attack logic
  protected virtual void PerformAttack()
  {
    if (isAttacking) return;

    Entity target = FindClosestValidTarget();

    if (target != null)
    {
      Vector2 origin = transform.position;
      Vector2 targetPos = target.transform.position;
      Vector2 direction = (targetPos - origin).normalized;

      mainEntity.OnFocusAnimationTrigger?.Invoke(animationTriggerName, direction);

      StartCoroutine(AttackCoroutine());
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
    isAttacking = true;

    yield return new WaitForSeconds(attackDelay);

    Entity target = FindClosestValidTarget();
    if (target != null)
    {
      // Instantiate and configure the projectile
      GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
      Destroy(projectileObject, projectileLifetime);
      Entity projectileEntity = projectileObject.GetComponent<Entity>();

      Vector2 targetPos = target.transform.position;

      // Calculate the direction and angle
      Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      projectileEntity.team = mainEntity.team;
      projectileEntity.OnMovementInput?.Invoke(direction);

      // Rotate the projectile to face the target
      projectileObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    yield return new WaitForSeconds(attackCooldown);

    isAttacking = false;
  }

  protected virtual void HurtCallback(float _)
  {
    StopAllCoroutines();
    isAttacking = false;
  }

  protected abstract void SubscribeInputEvents();
  protected abstract void UnsubscribeInputEvents();

  protected virtual void OnEnable()
  {
    SubscribeInputEvents();
    mainEntity.OnHealthDamaged += HurtCallback;
  }

  protected virtual void OnDisable()
  {
    StopAllCoroutines();
    UnsubscribeInputEvents();
    mainEntity.OnHealthDamaged -= HurtCallback;
  }
}