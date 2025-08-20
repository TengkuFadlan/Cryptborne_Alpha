using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Dodge : EntitySystem
{
  public float dodgeDuration = 0.5f; // Set as needed
  public float dodgeCooldown = 1.0f; // Cooldown after dodge

  bool isDodging = false;
  bool isOnCooldown = false;

  public void OnDodgeInputCallback()
  {
    if (isDodging || isOnCooldown) return;
    StartCoroutine(DodgeCoroutine());
  }

  IEnumerator DodgeCoroutine()
  {
    isDodging = true;
    isOnCooldown = true;
    mainEntity.IsInvulnerable = true;
    mainEntity.OnDodge?.Invoke();
    mainEntity.OnAnimationTrigger?.Invoke("Dodge");
    yield return new WaitForSeconds(dodgeDuration);
    mainEntity.IsInvulnerable = false;
    isDodging = false;
    yield return new WaitForSeconds(dodgeCooldown);
    isOnCooldown = false;
  }

  void OnEnable()
  {
    mainEntity.OnDodgeInput += OnDodgeInputCallback;
  }

  void OnDisable()
  {
    mainEntity.OnDodgeInput -= OnDodgeInputCallback;
  }
}