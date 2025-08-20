using UnityEngine;

public class AIChase : EntitySystem
{
  [Header("Chase Settings")]
  public float minChaseDistance = 1.0f; // Minimum distance to stop chasing

  Entity currentTarget;

  void FixedUpdate()
  {
    currentTarget = FindClosestOpponent();

    if (currentTarget != null)
    {
      float dist = Vector2.Distance(mainEntity.transform.position, currentTarget.transform.position);
      if (dist > minChaseDistance)
      {
        Vector2 direction = ((Vector2)currentTarget.transform.position - (Vector2)mainEntity.transform.position).normalized;
        mainEntity.OnMovementInput?.Invoke(direction);
      }
      else
      {
        mainEntity.OnMovementInput?.Invoke(Vector2.zero);
      }
    }
    else
    {
      mainEntity.OnMovementInput?.Invoke(Vector2.zero);
    }
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