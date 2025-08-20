using UnityEngine;

public class AIChase : EntitySystem
{
  [Header("Chase Settings")]
  public float minChaseDistance = 1.0f; // Minimum distance to stop chasing
  public float moveBackDistance = 0.5f; // Distance to move back

  [Header("Chase Cooldown")]
  public float chaseHurtStunDuration = 0.7f; // Seconds to pause chasing after hurt

  float chaseDisabledTimer = 0f;
  Entity currentTarget;

  void FixedUpdate()
  {
    if (chaseDisabledTimer > 0f)
    {
      chaseDisabledTimer -= Time.fixedDeltaTime;
      mainEntity.OnMovementInput?.Invoke(Vector2.zero);
      return;
    }

    currentTarget = FindClosestOpponent();

    if (currentTarget != null)
    {
      float dist = Vector2.Distance(mainEntity.transform.position, currentTarget.transform.position);
      Vector2 direction = ((Vector2)currentTarget.transform.position - (Vector2)mainEntity.transform.position).normalized;

      if (dist > minChaseDistance)
      {
        // Move towards the target
        mainEntity.OnMovementInput?.Invoke(direction);
      }
      else if (dist < moveBackDistance)
      {
        // Move away from the target by moveBackDistance
        Vector2 moveBackTarget = (Vector2)mainEntity.transform.position - direction * (moveBackDistance-dist);
        Vector2 moveBackDir = (moveBackTarget - (Vector2)mainEntity.transform.position).normalized;
        mainEntity.OnMovementInput?.Invoke(moveBackDir);
      }
      else
      {
        // Exactly at minChaseDistance, stop moving
        mainEntity.OnMovementInput?.Invoke(Vector2.zero);
      }
    }
    else
    {
      mainEntity.OnMovementInput?.Invoke(Vector2.zero);
    }
  }

  void OnEnable()
  {
    if (mainEntity != null)
      mainEntity.OnHealthDamaged += OnHurt;
  }

  void OnDisable()
  {
    if (mainEntity != null)
      mainEntity.OnHealthDamaged -= OnHurt;
  }

  void OnHurt(float damage)
  {
    chaseDisabledTimer = chaseHurtStunDuration;
  }

  Entity FindClosestOpponent()
  {
    GameObject[] entityGameObjects = GameObject.FindGameObjectsWithTag("Entity");
    Entity closestEntity = null;
    float closestDist = float.MaxValue;

    foreach (GameObject entityGameObject in entityGameObjects)
    {
      if (entityGameObject == mainEntity.gameObject) continue; // Skip self

      if (!entityGameObject.TryGetComponent<Entity>(out var entity)) continue;
      if (entity.Dead) continue;
      if (!TeamManager.IsOpponent(mainEntity, entity)) continue;

      float dist = Vector2.Distance(mainEntity.transform.position, entity.transform.position);
      if (dist < closestDist)
      {
        closestDist = dist;
        closestEntity = entity;
      }
    }
    return closestEntity;
  }
}