using UnityEngine;

public class GrenadeGizmoController : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public Color rangeIndicatorColor = new Color(1f, 0.3f, 0f, 0.3f);
    public float explosionRadius;
    public int circleSegments = 64;
    public bool showFill = true;

    private GameObject indicatorObject;
    private LineRenderer lineRenderer;

    void Start()
    {
        GrenadeScript grenadeScript = GetComponent<GrenadeScript>();
        if (grenadeScript != null)
        {
            explosionRadius = grenadeScript.explosionRadius;
        }
        
        CreateIndicator();
        ShowIndicator();
    }

    public void CreateIndicator()
    {
        if (indicatorObject != null) return;

        indicatorObject = new GameObject("RangeIndicator");
        indicatorObject.transform.SetParent(transform);
        indicatorObject.transform.localPosition = Vector3.zero;

        lineRenderer = indicatorObject.AddComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.positionCount = circleSegments;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = false;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = mat;
        lineRenderer.startColor = new Color(1f, 0.3f, 0f, 1f);
        lineRenderer.endColor = new Color(1f, 0.3f, 0f, 1f);
        lineRenderer.sortingLayerName = "Gizmos";
        lineRenderer.sortingOrder = 10;

        DrawCircle();

        if (showFill)
        {
            GameObject fill = new GameObject("RangeFill");
            fill.transform.SetParent(indicatorObject.transform);
            fill.transform.localPosition = Vector3.zero;

            MeshFilter mf = fill.AddComponent<MeshFilter>();
            MeshRenderer mr = fill.AddComponent<MeshRenderer>();

            Material fillMat = new Material(Shader.Find("Sprites/Default"));
            fillMat.color = rangeIndicatorColor;
            mr.material = fillMat;
            mr.sortingLayerName = "Gizmos";
            mr.sortingOrder = 9;

            mf.mesh = CreateFilledCircleMesh();
        }
    }

    public void DrawCircle()
    {
        if (lineRenderer == null) return;

        float angleStep = 360f / circleSegments;

        for (int i = 0; i < circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * explosionRadius;
            float y = Mathf.Sin(angle) * explosionRadius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public Mesh CreateFilledCircleMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[circleSegments + 1];
        int[] triangles = new int[circleSegments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < circleSegments; i++)
        {
            float angle = 2 * Mathf.PI * i / circleSegments;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * explosionRadius, Mathf.Sin(angle) * explosionRadius, 0);
        }
        
        for(int i = 0; i < circleSegments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 2 > circleSegments) ? 1 : i + 2;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

        public void ShowIndicator()
    {
        if (indicatorObject != null)
            indicatorObject.SetActive(true);
    }

    public void HideIndicator()
    {
        if (indicatorObject != null)
            indicatorObject.SetActive(false);
    }

    public void UpdateRadius(float newRadius)
    {
        explosionRadius = newRadius;
        DrawCircle();
        
        if (indicatorObject != null)
        {
            var fill = indicatorObject.transform.Find("RangeFill");
            if (fill != null)
            {
                fill.GetComponent<MeshFilter>().mesh = CreateFilledCircleMesh();
            }
        }
    }
}
