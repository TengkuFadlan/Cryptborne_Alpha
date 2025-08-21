using UnityEngine;

public class CameraFocusTarget : EntitySystem
{
  private Transform mainCamera;
  private Entity currentTarget;

  [Header("Camera Settings")]
  public float smoothSpeed = 5f;
  public float maxTargetRange = 10f;
  public float zoomSmoothSpeed = 5f;

  [Header("Zoom Settings")]
  public float minZoom = 5f;
  public float maxZoom = 12.5f;
  public float zoomPadding = 2f;

  protected override void Awake()
  {
    base.Awake();
    FindMainCamera();
  }

  private void FindMainCamera()
  {
    Camera cam = Camera.main;
    if (cam != null)
    {
      mainCamera = cam.transform;
    }
    else
    {
      Debug.LogError("Main camera not found! Make sure your camera has the 'MainCamera' tag.");
    }
  }

  private Entity FindClosestValidTarget()
  {
    GameObject[] entityGameObjects = GameObject.FindGameObjectsWithTag("Entity");
    Entity closestEntity = null;
    float closestDist = float.MaxValue;

    foreach (GameObject entityGameObject in entityGameObjects)
    {
      if (entityGameObject == mainEntity.gameObject) continue;

      if (!entityGameObject.TryGetComponent<Entity>(out var entity)) continue;
      if (entity.Dead) continue;
      if (!TeamManager.IsOpponent(mainEntity, entity)) continue;

      float dist = Vector2.Distance(transform.position, entityGameObject.transform.position);

      // Add a range check
      if (dist < closestDist && dist <= maxTargetRange)
      {
        closestDist = dist;
        closestEntity = entity;
      }
    }
    return closestEntity;
  }

  void LateUpdate()
  {
    if (mainCamera == null) return;

    currentTarget = FindClosestValidTarget();

    if (currentTarget != null)
    {
      Vector3 midPoint = (mainEntity.transform.position + currentTarget.transform.position) / 2f;
      Vector3 newPosition = new Vector3(midPoint.x, midPoint.y, mainCamera.position.z);
      mainCamera.position = Vector3.Lerp(mainCamera.position, newPosition, smoothSpeed * Time.deltaTime);

      float distance = Vector2.Distance(mainEntity.transform.position, currentTarget.transform.position);
      float targetZoom = Mathf.Clamp(distance + zoomPadding, minZoom, maxZoom);
      mainCamera.GetComponent<Camera>().orthographicSize = Mathf.Lerp(mainCamera.GetComponent<Camera>().orthographicSize, targetZoom, zoomSmoothSpeed * Time.deltaTime);
    }
    else
    {
      Vector3 newPosition = new Vector3(mainEntity.transform.position.x, mainEntity.transform.position.y, mainCamera.position.z);
      mainCamera.position = Vector3.Lerp(mainCamera.position, newPosition, smoothSpeed * Time.deltaTime);

      // Zoom back to the default level when no target is in range
      mainCamera.GetComponent<Camera>().orthographicSize = Mathf.Lerp(mainCamera.GetComponent<Camera>().orthographicSize, minZoom, zoomSmoothSpeed * Time.deltaTime);
    }
  }
}