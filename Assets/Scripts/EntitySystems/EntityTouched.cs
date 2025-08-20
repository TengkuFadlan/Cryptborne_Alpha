using UnityEngine;

public class EntityTouched : EntitySystem
{
  void OnTriggerEnter2D(Collider2D other)
  {
    Debug.Log("Entity Touched");
    mainEntity.OnEntityTouched?.Invoke(other);
  }
}