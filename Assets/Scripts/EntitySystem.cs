using UnityEngine;

public abstract class EntitySystem : MonoBehaviour
{
  protected Entity mainEntity;

  protected virtual void Awake()
  {
    mainEntity = transform.root.GetComponent<Entity>();
  }
}