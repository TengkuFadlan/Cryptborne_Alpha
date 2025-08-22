using UnityEngine;

public class ProjectileDamage : DamageSystem // Inherit from DamageSystem
{
  [Header("Projectile Settings")]
  public float baseDamage = 50f;

  void OnEntityTouchedCallback(Collider2D other)
  {
    if (other.gameObject.TryGetComponent<Entity>(out var hitEntity))
    {
      if (TeamManager.IsOpponent(mainEntity, hitEntity))
      {
        // Apply modified damage
        ApplyDamage(hitEntity, baseDamage);

        // Kill self after the damage is dealt
        mainEntity.IsInvulnerable = false;
        mainEntity.OnRecieveDamage?.Invoke(mainEntity.CurrentHealth * 3);
      }
    }
  }

  protected override void OnEnable()
  {
    base.OnEnable(); // Call the base class OnEnable to handle modifier subscriptions
    mainEntity.OnEntityTouched += OnEntityTouchedCallback;
  }

  protected override void OnDisable()
  {
    base.OnDisable(); // Call the base class OnDisable to handle modifier unsubscriptions
    mainEntity.OnEntityTouched -= OnEntityTouchedCallback;
  }
}