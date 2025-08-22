using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Dodge : EntitySystem
{
  [Header("Dodge Settings")]
  public float dodgeDuration = 0.5f;
  public float dodgeCooldown = 1.0f;

  // Use a float to store the timestamp of the last dodge
  private float lastDodgeTime = 0f;

  void Update()
  {
    float totalCooldown = dodgeCooldown + dodgeDuration;
    float progress = Mathf.Clamp((Time.time - lastDodgeTime) / totalCooldown, 0, 1);
    mainEntity.OnDodgeProgress?.Invoke(progress);
  }

  public void OnDodgeInputCallback()
  {
    // Check if the cooldown period has passed
    if (Time.time < lastDodgeTime + dodgeCooldown + dodgeDuration)
    {
      return;
    }

    StartCoroutine(DodgeCoroutine());
    // Update the timestamp of the last dodge
    lastDodgeTime = Time.time;
  }

  IEnumerator DodgeCoroutine()
  {
    mainEntity.IsInvulnerable = true;
    mainEntity.OnDodge?.Invoke();
    mainEntity.OnAnimationTrigger?.Invoke("Dodge");

    yield return new WaitForSeconds(dodgeDuration);

    mainEntity.IsInvulnerable = false;
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