using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : EntitySystem, PlayerInputActions.IKeyboardActions
{
  private PlayerInputActions inputActions;

  protected override void Awake()
  {
    base.Awake();
    inputActions = new PlayerInputActions();
    inputActions.Keyboard.SetCallbacks(this);
  }

  void OnEnable()
  {
    inputActions.Keyboard.Enable();
  }

  void OnDisable()
  {
    inputActions.Keyboard.Disable();
  }

  public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
  {
    Vector2 input = context.ReadValue<Vector2>();
    mainEntity.OnMovementInput?.Invoke(input);
  }

  public void OnBasicAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
  {
    if (context.performed)
      mainEntity.OnBasicAttackInput?.Invoke();
  }

  public void OnDodge(UnityEngine.InputSystem.InputAction.CallbackContext context)
  {
    if (context.performed)
      mainEntity.OnDodgeInput?.Invoke();
  }

  public void OnPrimarySkill(UnityEngine.InputSystem.InputAction.CallbackContext context)
  {
    if (context.performed)
      mainEntity.OnPrimarySkillInput?.Invoke();
  }

  public void OnSecondarySkill(UnityEngine.InputSystem.InputAction.CallbackContext context)
  {
    if (context.performed)
      mainEntity.OnSecondarySkillInput?.Invoke();
  }

  public void OnUltimate(UnityEngine.InputSystem.InputAction.CallbackContext context)
  {
    if (context.performed)
      mainEntity.OnUltimateInput?.Invoke();
  }
}