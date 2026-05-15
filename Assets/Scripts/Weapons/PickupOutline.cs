using UnityEngine;

public class PickupOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    public float outlineScale = 1f;

    [Header("Glow Colors (use HDR)")]
    [ColorUsage(true, true)] public Color defaultColor = new Color(0f, 5f, 5f, 1f);   // HDR cyan
    [ColorUsage(true, true)] public Color hoveredColor = new Color(0f, 8f, 2f, 1f);   // HDR green

    [Header("Pulse when hovered")]
    public float pulseSpeed = 4f;
    public float pulseMinIntensity = 4f;
    public float pulseMaxIntensity = 8f;

    public SpriteRenderer mainSr;
    public Sprite outlineSprite;

    private GameObject outlineObject;
    private SpriteRenderer outlineSr;
    private Material glowMaterial;
    private bool isHovered;

    private void Start()
    {
        mainSr = GetComponent<SpriteRenderer>();
        if (mainSr == null)
            mainSr = GetComponentInChildren<SpriteRenderer>();

        if (mainSr == null)
        {
            Debug.LogWarning($"[PickupOutline] No SpriteRenderer found on {gameObject.name}");
            return;
        }

        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one * outlineScale;

        outlineSr = outlineObject.AddComponent<SpriteRenderer>();
        outlineSr.sprite = outlineSprite;

        glowMaterial = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
        outlineSr.material = glowMaterial;
        outlineSr.color = defaultColor;

        outlineSr.sortingLayerName = mainSr.sortingLayerName;
        outlineSr.sortingOrder = mainSr.sortingOrder - 1;
    }

    private void Update()
    {
        if (!isHovered || glowMaterial == null) return;

        float intensity = Mathf.Lerp(
            pulseMinIntensity,
            pulseMaxIntensity,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f
        );

        Color baseColor = new Color(
            hoveredColor.r / hoveredColor.maxColorComponent,
            hoveredColor.g / hoveredColor.maxColorComponent,
            hoveredColor.b / hoveredColor.maxColorComponent,
            1f
        );

        outlineSr.color = baseColor * intensity;
    }

    public void SetHovered(bool hovered)
    {
        isHovered = hovered;

        if (!hovered && outlineSr != null)
            outlineSr.color = defaultColor;
    }
}