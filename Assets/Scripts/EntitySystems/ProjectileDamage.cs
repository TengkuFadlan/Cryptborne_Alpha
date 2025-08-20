using UnityEngine;

public class ProjectileDamage : EntitySystem
{
  [Header("Projectile Settings")]
  public float damageValue = 50f;

  void OnEntityTouchedCallback(Collider2D other)
  {
    if (other.gameObject.TryGetComponent<Entity>(out var hitEntity))
    {
      if (TeamManager.IsOpponent(mainEntity, hitEntity))
      {
        // Apply damage to the collided entity
        hitEntity.OnRecieveDamage?.Invoke(damageValue);

        // Kill self after the damage is dealt
        mainEntity.IsInvulnerable = false;
        mainEntity.OnRecieveDamage?.Invoke(mainEntity.CurrentHealth * 3);
      }
    }
  }

  void OnEnable()
  {
    mainEntity.OnEntityTouched += OnEntityTouchedCallback;
  }
  void OnDisable()
  {
    mainEntity.OnEntityTouched -= OnEntityTouchedCallback;
  }
}