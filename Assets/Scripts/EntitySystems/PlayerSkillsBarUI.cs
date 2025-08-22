using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillsBarUI : EntitySystem
{
  [Header("UI Sliders")]
  private Slider basicAttackSlider;
  private Slider primarySkillSlider;
  private Slider secondarySkillSlider;
  private Slider ultimateSlider;
  private Slider dodgeSlider;

  protected override void Awake()
  {
    base.Awake();
    FindSliders();
  }

  void OnEnable()
  {
    mainEntity.OnBasicAttackProgress += UpdateBasicAttackSlider;
    mainEntity.OnPrimarySkillProgress += UpdatePrimarySkillSlider;
    mainEntity.OnSecondarySkillProgress += UpdateSecondarySkillSlider;
    mainEntity.OnUltimateProgress += UpdateUltimateSlider;
    mainEntity.OnDodgeProgress += UpdateDodgeSlider;
  }

  void OnDisable()
  {
    mainEntity.OnBasicAttackProgress -= UpdateBasicAttackSlider;
    mainEntity.OnPrimarySkillProgress -= UpdatePrimarySkillSlider;
    mainEntity.OnSecondarySkillProgress -= UpdateSecondarySkillSlider;
    mainEntity.OnUltimateProgress -= UpdateUltimateSlider;
    mainEntity.OnDodgeProgress -= UpdateDodgeSlider;
  }

  private void FindSliders()
  {
    GameObject basicAttackBar = GameObject.FindWithTag("PlayerBasicAttackBar");
    if (basicAttackBar != null)
    {
      basicAttackSlider = basicAttackBar.GetComponentInChildren<Slider>();
    }

    GameObject primarySkillBar = GameObject.FindWithTag("PlayerPrimarySkillBar");
    if (primarySkillBar != null)
    {
      primarySkillSlider = primarySkillBar.GetComponentInChildren<Slider>();
    }

    GameObject secondarySkillBar = GameObject.FindWithTag("PlayerSecondarySkillBar");
    if (secondarySkillBar != null)
    {
      secondarySkillSlider = secondarySkillBar.GetComponentInChildren<Slider>();
    }

    GameObject ultimateBar = GameObject.FindWithTag("PlayerUltimateBar");
    if (ultimateBar != null)
    {
      ultimateSlider = ultimateBar.GetComponentInChildren<Slider>();
    }

    GameObject dodgeBar = GameObject.FindWithTag("PlayerDodgeBar");
    if (dodgeBar != null)
    {
      dodgeSlider = dodgeBar.GetComponentInChildren<Slider>();
    }
  }

  private void UpdateBasicAttackSlider(float progress)
  {
    if (basicAttackSlider != null)
    {
      basicAttackSlider.value = progress;
    }
  }

  private void UpdatePrimarySkillSlider(float progress)
  {
    if (primarySkillSlider != null)
    {
      primarySkillSlider.value = progress;
    }
  }

  private void UpdateSecondarySkillSlider(float progress)
  {
    if (secondarySkillSlider != null)
    {
      secondarySkillSlider.value = progress;
    }
  }

  private void UpdateUltimateSlider(float progress)
  {
    if (ultimateSlider != null)
    {
      ultimateSlider.value = progress;
    }
  }

  private void UpdateDodgeSlider(float progress)
  {
    if (dodgeSlider != null)
    {
      dodgeSlider.value = progress;
    }
  }
}