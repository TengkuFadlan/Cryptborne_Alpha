using UnityEngine;

public class EntityTouched : EntitySystem
{
  void OnTriggerEnter2D(Collider2D other)
  {
    mainEntity.OnEntityTouched?.Invoke(other);
  }
}