using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Movement : EntitySystem
{

  private Rigidbody2D rb;
  private bool isKnockedBack = false;

  protected override void Awake()
  {
    base.Awake();

    rb = mainEntity.GetComponent<Rigidbody2D>();
  }

  void FixedUpdate()
  {
    if (isKnockedBack) return; // Ignore movement input if knocked back

    rb.linearVelocity = mainEntity.MovementDirection;

    if (rb.linearVelocity.magnitude > 0.01)
    {
      mainEntity.LastMovementDirection = new Vector2(mainEntity.MovementDirection.x, mainEntity.MovementDirection.y);
      mainEntity.OnMoved?.Invoke(new Vector3(mainEntity.MovementDirection.x, mainEntity.MovementDirection.y, rb.linearVelocity.magnitude));
    }
    else
    {
      mainEntity.OnMovementStopped?.Invoke();
    }
  }

  void MovementInputCallback(Vector2 movementInput)
  {
    if (isKnockedBack) return; // Ignore movement input if knocked back

    mainEntity.MovementDirection = movementInput.normalized * mainEntity.Speed;
  }
  
  // New method to handle knockback event
  void KnockbackCallback(Vector2 direction, float duration)
  {
    if (isKnockedBack) return; // Prevent multiple knockback events from stacking

    StartCoroutine(KnockbackCoroutine(direction, duration));
  }

  private IEnumerator KnockbackCoroutine(Vector2 direction, float duration)
  {
    isKnockedBack = true;
    rb.AddForce(direction, ForceMode2D.Impulse);

    yield return new WaitForSeconds(duration);

    isKnockedBack = false;
    mainEntity.MovementDirection = Vector2.zero; // Reset movement direction
  }

  void OnEnable()
  {
    isKnockedBack = false;
    mainEntity.OnMovementInput += MovementInputCallback;
    mainEntity.OnKnockback += KnockbackCallback;
  }

  void OnDisable()
  {
    StopAllCoroutines();
    mainEntity.OnMovementInput -= MovementInputCallback;
    mainEntity.OnKnockback -= KnockbackCallback;
  }
}