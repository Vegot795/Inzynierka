using UnityEngine;

public class GrenadeGizmoController : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public Color throwRangeColor = new Color(0f, 1f, 1f, 0.3f);
    public Color explosionRangeColor = new Color(1f, 0f, 0f, 0.3f);
    public int circleSegments = 64;

    private bool isAiming = false;
    private float throwRange;
    private float explosionRadius;
    private Vector2 targetPosition;
    private Transform playerTransform;

    private LineRenderer throwRangeCircle;
    private LineRenderer explosionRangeCircle;

    private void Awake()
    {
        playerTransform = transform;
        
        GameObject throwRangeObj = new GameObject("ThrowRangeGizmo");
        throwRangeObj.transform.SetParent(transform);
        throwRangeCircle = throwRangeObj.AddComponent<LineRenderer>();
        SetupLineRenderer(throwRangeCircle, throwRangeColor);

        GameObject explosionRangeObj = new GameObject("ExplosionRangeGizmo");
        explosionRangeObj.transform.SetParent(transform);
        explosionRangeCircle = explosionRangeObj.AddComponent<LineRenderer>();
        SetupLineRenderer(explosionRangeCircle, explosionRangeColor);

        HideGizmos();
    }

    private void SetupLineRenderer(LineRenderer lr, Color color)
    {
        lr.positionCount = circleSegments + 1;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.useWorldSpace = false;
        lr.loop = true;
        
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.sortingOrder = 100;
    }

    private void Update()
    {
        if (isAiming)
        {
            UpdateExplosionGizmo();
        }
    }

    public void ShowGizmos(float range, float explosion, Vector2 target)
    {
        isAiming = true;
        throwRange = range;
        explosionRadius = explosion;
        targetPosition = target;

        DrawCircle(throwRangeCircle, Vector3.zero, throwRange);
        throwRangeCircle.gameObject.SetActive(true);
        explosionRangeCircle.gameObject.SetActive(true);
    }

    public void HideGizmos()
    {
        isAiming = false;
        throwRangeCircle.gameObject.SetActive(false);
        explosionRangeCircle.gameObject.SetActive(false);
    }

    public void UpdateTargetPosition(Vector2 target)
    {
        targetPosition = target;
    }

    private void UpdateExplosionGizmo()
    {
        Vector3 localTarget = playerTransform.InverseTransformPoint(targetPosition);
        DrawCircle(explosionRangeCircle, localTarget, explosionRadius);
    }

    private void DrawCircle(LineRenderer lr, Vector3 center, float radius)
    {
        float angleStep = 360f / circleSegments;

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
