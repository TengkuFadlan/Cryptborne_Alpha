using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Animation8D : EntitySystem
{

  private Animator animator;

  protected override void Awake()
  {
    base.Awake();

    animator = GetComponent<Animator>();
  }

  void OnAnimationTriggerCallback(string AnimationName)
  {
    animator.SetTrigger(AnimationName);
  }

  void OnFocusAnimationTriggerCallback(string AnimationName, Vector2 Direction)
  {
    animator.SetFloat("FocusDirectionX", Direction.x);
    animator.SetFloat("FocusDirectionY", Direction.y);
    OnAnimationTriggerCallback(AnimationName);
  }

  void OnMovedCallback(Vector3 MovementData)
  {
    float DirectionX = MovementData.x;
    float DirectionY = MovementData.y;
    float Velocity = MovementData.z;

    animator.SetFloat("DirectionX", DirectionX);
    animator.SetFloat("DirectionY", DirectionY);
    animator.SetFloat("Velocity", Velocity);
  }

  void OnMovementStoppedCallback()
  {
    float LastDirectionX = mainEntity.LastMovementDirection.x;
    float LastDirectionY = mainEntity.LastMovementDirection.y;

    animator.SetFloat("LastDirectionX", LastDirectionX);
    animator.SetFloat("LastDirectionY", LastDirectionY);
    animator.SetFloat("Velocity", 0);
  }

  void OnDeathCallback()
  {
    OnAnimationTriggerCallback("Death");
  }

  void OnDamagedCallback(float _)
  {
    OnAnimationTriggerCallback("Hurt");
  }

  void OnEnable()
  {
    mainEntity.OnMoved += OnMovedCallback;
    mainEntity.OnMovementStopped += OnMovementStoppedCallback;
    mainEntity.OnAnimationTrigger += OnAnimationTriggerCallback;
    mainEntity.OnFocusAnimationTrigger += OnFocusAnimationTriggerCallback;
    mainEntity.OnDeath += OnDeathCallback;
    mainEntity.OnHealthDamaged += OnDamagedCallback;
  }

  void OnDisable()
  {
    mainEntity.OnMoved -= OnMovedCallback;
    mainEntity.OnMovementStopped -= OnMovementStoppedCallback;
    mainEntity.OnAnimationTrigger -= OnAnimationTriggerCallback;
    mainEntity.OnFocusAnimationTrigger -= OnFocusAnimationTriggerCallback;
    mainEntity.OnDeath -= OnDeathCallback;
    mainEntity.OnHealthDamaged -= OnDamagedCallback;
  }
}
