using UnityEngine;
using UnityEngine.UI;

public class TargetHealthBarSystem : EntitySystem
{
    [Header("Health Bar Settings")]
    public GameObject targetHealthBarPrefab; // Drag your TargetHealthBar prefab here

    private Entity currentTarget;
    private GameObject instantiatedHealthBar;
    private Image radialHealthBarImage;
    public float maxTargetRange = 10f; // Use the same range as CameraFocusTarget for consistency

    protected override void Awake()
    {
        base.Awake();
        
    }

    void OnEnable()
    {
        // Resume operation when enabled
        UpdateTargetHealthBar();
    }

    void OnDisable()
    {
        // Clear the health bar when the script is disabled
        ClearTargetHealthBar();
    }

    void LateUpdate()
    {
        UpdateTargetHealthBar();
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
            if (entity.Dead || entity.Projectile) continue;
            if (!TeamManager.IsOpponent(mainEntity, entity)) continue;

            float dist = Vector2.Distance(transform.position, entityGameObject.transform.position);

            if (dist < closestDist && dist <= maxTargetRange)
            {
                closestDist = dist;
                closestEntity = entity;
            }
        }
        return closestEntity;
    }

    private void UpdateTargetHealthBar()
    {
        Entity newTarget = FindClosestValidTarget();

        if (newTarget != currentTarget)
        {
            // A new target is found or the old one is lost
            ClearTargetHealthBar();
            if (newTarget != null)
            {
                currentTarget = newTarget;
                InstantiateHealthBar();
            }
        }
        
        // This makes sure the health bar is updated even if the target is the same
        if (currentTarget != null && radialHealthBarImage != null)
        {
            float healthRatio = currentTarget.CurrentHealth / currentTarget.MaxHealth;
            radialHealthBarImage.fillAmount = healthRatio;
        }
    }

    private void InstantiateHealthBar()
    {
        if (currentTarget == null) return;

        // Instantiate the prefab and parent it to the target entity
        instantiatedHealthBar = Instantiate(targetHealthBarPrefab, currentTarget.transform);
        instantiatedHealthBar.transform.localPosition = Vector3.zero; // Or adjust as needed

        // Find the Canvas component and set its Event Camera
        Canvas canvas = instantiatedHealthBar.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }

        // Find the RadialHealthBar Image component
        radialHealthBarImage = instantiatedHealthBar.GetComponentInChildren<Image>();

        if (radialHealthBarImage == null)
        {
            Debug.LogError("RadialHealthBar Image component not found in prefab children!");
        }
    }

    private void ClearTargetHealthBar()
    {
        if (instantiatedHealthBar != null)
        {
            Destroy(instantiatedHealthBar);
            instantiatedHealthBar = null;
            radialHealthBarImage = null;
        }

        // Unsubscribe from events to prevent memory leaks, if we ever subscribed to them
        currentTarget = null;
    }
}