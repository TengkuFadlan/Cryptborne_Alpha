using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UltimateHealDamageBoost : EntitySystem
{
  [Header("Ultimate Skill Settings")]
  public float chargeRequired;
  public float ultDuration;
  public float healAmountPerSecond;
  public float damageBoost;
  public string ultimateBarTagName;

  private float currentCharge = 0f;
  private bool isUltimateActive = false;
  private Guid damageModifierId;
  private Coroutine ultimateCoroutine;

  void Update()
  {
    mainEntity.OnUltimateProgress?.Invoke(currentCharge / chargeRequired);
  }

  void AddChargeCallback(float damageDealt)
  {
    if (isUltimateActive) return;

    currentCharge += damageDealt;
    if (currentCharge > chargeRequired)
    {
      currentCharge = chargeRequired;
    }
  }

  void UltimateInputCallback()
  {
    if (isUltimateActive)
    {
      DeactivateUltimate();
    }
    else if (currentCharge >= chargeRequired)
    {
      ultimateCoroutine = StartCoroutine(UltimateSequenceCoroutine());
      mainEntity.OnUltimateCast?.Invoke();
    }
  }

  IEnumerator UltimateSequenceCoroutine()
  {
    isUltimateActive = true;
    damageModifierId = Guid.NewGuid();
    mainEntity.OnDamagePercentModifierAdded?.Invoke(damageModifierId, damageBoost);

    float chargeDrainRate = chargeRequired / ultDuration;

    while (currentCharge > 0)
    {
      // Drain charge
      currentCharge -= chargeDrainRate * Time.deltaTime;

      // Heal player
      mainEntity.OnRecieveHeal?.Invoke(healAmountPerSecond * Time.deltaTime);

      yield return null;
    }

    DeactivateUltimate();
  }

  void DeactivateUltimate()
  {
    if (ultimateCoroutine != null)
    {
      StopCoroutine(ultimateCoroutine);
    }
    isUltimateActive = false;

    // Clear damage modifier
    mainEntity.OnDamagePercentModifierRemoved?.Invoke(damageModifierId);
  }

  void OnEnable()
  {
    mainEntity.OnDealtDamage += AddChargeCallback;
    mainEntity.OnUltimateInput += UltimateInputCallback;
  }

  void OnDisable()
  {
    StopAllCoroutines();
    isUltimateActive = false;
    mainEntity.OnDealtDamage -= AddChargeCallback;
    mainEntity.OnUltimateInput -= UltimateInputCallback;

    // Clear damage modifier if it exists
    if (damageModifierId != Guid.Empty)
    {
      mainEntity.OnDamagePercentModifierRemoved?.Invoke(damageModifierId);
    }
  }
}