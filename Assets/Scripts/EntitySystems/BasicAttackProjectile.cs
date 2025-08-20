using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BasicAttackProjectile : EntitySystem
{
  [Header("Projectile Attack Settings")]
  public float attackRange;
  public float attackDelay;
  public float attackCooldown;
  public float projectileLifetime;
  public GameObject projectilePrefab;

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
      if (dist < closestDist && dist <= attackRange)
      {
        closestDist = dist;
        closestEntity = entity;
      }
    }
    return closestEntity;
  }

  void BasicAttackInputCallback()
  {
    if (isAttacking) return;

    Entity target = FindClosestValidTarget();

    if (target != null)
    {
      Vector2 origin = transform.position;
      Vector2 targetPos = target.transform.position;
      Vector2 direction = (targetPos - origin).normalized;

      mainEntity.OnFocusAnimationTrigger?.Invoke("BasicAttack", direction);

      StartCoroutine(AttackCoroutine());
    }
  }

  IEnumerator AttackCoroutine()
  {
    isAttacking = true;

    Debug.Log("Attack");

    yield return new WaitForSeconds(attackDelay);

    Entity target = FindClosestValidTarget();

    // Instantiate and configure the projectile
    if (target != null)
    {
      GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
      Destroy(projectileObject, projectileLifetime);
      Entity projectileEntity = projectileObject.GetComponent<Entity>();
      
      Vector2 targetPos = target.transform.position;
      
      // Calculate the direction and angle
      Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      projectileEntity.team = mainEntity.team; // Set projectile to the same team as the archer
      projectileEntity.OnMovementInput?.Invoke(direction);

      // Rotate the projectile to face the target
      projectileObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    yield return new WaitForSeconds(attackCooldown);

    isAttacking = false;
  }

  void OnEnable()
  {
    mainEntity.OnBasicAttackInput += BasicAttackInputCallback;
  }

  void OnDisable()
  {
    StopAllCoroutines();
    mainEntity.OnBasicAttackInput -= BasicAttackInputCallback;
  }
}