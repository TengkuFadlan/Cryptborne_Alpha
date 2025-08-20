using UnityEngine;

public class DeathDestroy : EntitySystem
{
  public float DestroyDelay = 3f;

  void OnEnable()
  {
    mainEntity.OnDeath += HandleDeath;
  }

  void OnDisable()
  {
    mainEntity.OnDeath -= HandleDeath;
  }

  void HandleDeath()
  {
    Destroy(mainEntity.gameObject, DestroyDelay);
  }
}