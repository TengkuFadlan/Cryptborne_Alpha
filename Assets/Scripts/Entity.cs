using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
  [Header("Team")]
  public Team team;

  [Header("Health System")]
  public float CurrentHealth = 100f;
  public float MaxHealth = 100f;
  public bool Dead = false;
  public Action<float> OnHealthDamaged;
  public Action<float> OnHealthHealed;
  public Action OnDeath;
  public Action<float> OnRecieveDamage;
  public Action<float> OnRecieveHeal;
  public bool IsInvulnerable = false;

  [Header("Movement System")]
  public float Speed;
  public Vector2 MovementDirection;
  public Vector2 LastMovementDirection;
  public Action<Vector3> OnMoved; // Parameters: Vector3 -> MovementDirectionX, MovementDirectionY, Velocity
  public Action OnMovementStopped;

  [Header("Dodge System")]
  public Action OnDodge;

  [Header("Animation8D System")]
  public Action<string> OnAnimationTrigger;
  public Action<string, Vector2> OnFocusAnimationTrigger;

  [Header("Input Systems")]
  public Action OnBasicAttackInput;
  public Action OnPrimarySkillInput;
  public Action OnSecondarySkillInput;
  public Action OnUltimateInput;
  public Action<Vector2> OnMovementInput;
  public Action OnDodgeInput;
}
