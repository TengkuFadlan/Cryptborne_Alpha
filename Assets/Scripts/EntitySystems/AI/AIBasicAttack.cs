using UnityEngine;

public class AIBasicAttack : EntitySystem
{
  [Header("AI Attack Settings")]
  public float attackTriggerDistance = 1.2f;
  public float attackHoldTime = 1.0f;

  Entity currentTarget;
  float closeTimer = 0f;

  void FixedUpdate()
  {
    currentTarget = FindClosestOpponent();

    if (currentTarget != null)
    {
      float dist = Vector2.Distance(mainEntity.transform.position, currentTarget.transform.position);
      if (dist <= attackTriggerDistance)
      {
        closeTimer += Time.fixedDeltaTime;
        if (closeTimer >= attackHoldTime)
        {
          mainEntity.OnBasicAttackInput?.Invoke();
        }
      }
      else
      {
        closeTimer = 0f; // Reset timer if out of range
      }
    }
    else
    {
      closeTimer = 0f;
    }
  }

  Entity FindClosestOpponent()
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

      float dist = Vector2.Distance(mainEntity.transform.position, entity.transform.position);
      if (dist < closestDist)
      {
        closestDist = dist;
        closestEntity = entity;
      }
    }
    return closestEntity;
  }

  void OnHealthDamagedCallback(float damage)
  {
    closeTimer = 0f; // stunned
  }

  void OnEnable()
  {
    mainEntity.OnHealthDamaged += OnHealthDamagedCallback;
  }

  void OnDisable()
  {
    mainEntity.OnHealthDamaged -= OnHealthDamagedCallback;
  }
}