using UnityEngine;

public class DeathDisableSystem : EntitySystem
{
  [Header("Systems to Disable On Death")]
  public MonoBehaviour[] systemsToDisable;

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
    foreach (var system in systemsToDisable)
    {
      system.enabled = false;
    }
  }
}