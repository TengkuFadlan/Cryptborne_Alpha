using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageSystem : EntitySystem
{
  // Dictionary to store active damage modifiers
  protected readonly Dictionary<Guid, float> damageModifiers = new();

  // New callback for adding a damage modifier
  protected void AddDamageModifierCallback(Guid id, float modifier)
  {
    damageModifiers[id] = modifier;
  }

  // New callback for removing a damage modifier
  protected void RemoveDamageModifierCallback(Guid id)
  {
    damageModifiers.Remove(id);
  }

  // Method to calculate damage based on active modifiers
  protected float CalculateModifiedDamage(float baseDamage)
  {
    float totalModifier = 1.0f;
    foreach (float modifier in damageModifiers.Values)
    {
      totalModifier *= modifier;
    }
    return baseDamage * totalModifier;
  }

  protected void ApplyDamage(Entity target, float baseDamage)
  {
    float modifiedDamage = CalculateModifiedDamage(baseDamage);
    target.OnRecieveDamage?.Invoke(modifiedDamage);
    mainEntity.OnDealtDamage?.Invoke(modifiedDamage); // Invoke the new action
  }

  protected virtual void OnEnable()
  {
    mainEntity.OnDamagePercentModifierAdded += AddDamageModifierCallback;
    mainEntity.OnDamagePercentModifierRemoved += RemoveDamageModifierCallback;
  }

  protected virtual void OnDisable()
  {
    mainEntity.OnDamagePercentModifierAdded -= AddDamageModifierCallback;
    mainEntity.OnDamagePercentModifierRemoved -= RemoveDamageModifierCallback;
  }
}