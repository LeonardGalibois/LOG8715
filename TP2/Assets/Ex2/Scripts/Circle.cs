using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class Circle : MonoBehaviour
{
    [FormerlySerializedAs("I")] [HideInInspector]
    public int i;

    [FormerlySerializedAs("J")] [HideInInspector]
    public int j;

    public float Health { get; private set; }

    private const float BaseHealth = 1000;

    private const float HealingPerSecond = 1;
    private const float HealingRange = 3;

    private GridShape grid;
    private SpriteRenderer spriteRenderer;
    private List<Circle> nearbyCircles;

    // Start is called before the first frame update
    private void Start()
    {
        Health = BaseHealth;

        grid = FindObjectOfType<GridShape>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nearbyCircles = new List<Circle>();

        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, HealingRange);
        foreach (var collider in nearbyColliders)
        {
            if (collider.TryGetComponent(out Circle circle)) nearbyCircles.Add(circle);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateColor();
        HealNearbyShapes();
    }

    private void UpdateColor()
    {
        // Here, looking for the GridShape and SpriteRenderer every frame when those two objects can
        // be known when simulation starts and don't change is way too costly
        spriteRenderer.color = grid.Colors[i, j] * Health / BaseHealth;
    }

    private void HealNearbyShapes()
    {
        foreach (var circle in nearbyCircles) circle.ReceiveHp(HealingPerSecond * Time.deltaTime);
    }

    public void ReceiveHp(float hpReceived)
    {
        Health += hpReceived;
        Health = Mathf.Clamp(Health, 0, BaseHealth);
    }
}
