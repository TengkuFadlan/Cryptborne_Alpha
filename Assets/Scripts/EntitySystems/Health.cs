public class Health : EntitySystem
{

  void DamageCallback(float DamageValue)
  {
    if (mainEntity.Dead || mainEntity.IsInvulnerable) return;

    mainEntity.CurrentHealth -= DamageValue;
    mainEntity.OnHealthDamaged?.Invoke(DamageValue);

    if (mainEntity.CurrentHealth <= 0)
    {
      mainEntity.Dead = true;
      mainEntity.CurrentHealth = 0;
      mainEntity.OnDeath?.Invoke();
    }
  }

  void HealCallback(float HealValue)
  {
    if (mainEntity.Dead) return;

    mainEntity.CurrentHealth = System.Math.Min(mainEntity.CurrentHealth + HealValue, mainEntity.MaxHealth);
    mainEntity.OnHealthHealed?.Invoke(HealValue);
  }

  void OnEnable()
  {
    mainEntity.OnRecieveDamage += DamageCallback;
    mainEntity.OnRecieveHeal += HealCallback;
  }

  void OnDisable()
  {
    mainEntity.OnRecieveDamage -= DamageCallback;
    mainEntity.OnRecieveHeal -= HealCallback;
  }
}
