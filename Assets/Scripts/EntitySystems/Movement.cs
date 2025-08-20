using UnityEngine;
using UnityEngine.Events;

public class Movement : EntitySystem
{

  private Rigidbody2D rb;

  protected override void Awake()
  {
    base.Awake();

    rb = mainEntity.GetComponent<Rigidbody2D>();
  }

  void FixedUpdate()
  {
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
    mainEntity.MovementDirection = movementInput.normalized * mainEntity.Speed;
  }

  void OnEnable()
  {
    mainEntity.OnMovementInput += MovementInputCallback;
  }

  void OnDisable()
  {
    mainEntity.OnMovementInput -= MovementInputCallback;
  }
}