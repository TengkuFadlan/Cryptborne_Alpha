using UnityEngine;
using UnityEngine.UI;

public class SliderHealth : EntitySystem
{
  [Header("UI Health Bar Settings")]
  public string healthBarTag = "PlayerHealthBar";

  private Slider healthSlider;

  protected override void Awake()
  {
    base.Awake();
    FindHealthBarUI();
  }

  private void FindHealthBarUI()
  {
    GameObject healthBarObject = GameObject.FindWithTag(healthBarTag);

    if (healthBarObject != null)
    {
      healthSlider = healthBarObject.GetComponentInChildren<Slider>();
      if (healthSlider == null)
      {
        Debug.LogError("Slider component not found in children of the health bar object with tag: " + healthBarTag);
      }
    }
    else
    {
      Debug.LogError("Health bar UI object not found with tag: " + healthBarTag);
    }
  }

  void Update()
  {
    if (healthSlider != null)
    {
      float healthRatio = mainEntity.CurrentHealth / mainEntity.MaxHealth;
      healthSlider.value = healthRatio;
    }
  }
}