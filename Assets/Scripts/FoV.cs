using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FoV : MonoBehaviour
{
    public float fov = 360f;
    public float viewDistance;
    public int rayCount = 72;
    public float outOfSightDelay = 5f; 
    public LayerMask obstacleMask;
    public LayerMask sightBlockerMask;
    public Color viewColor = new Color(1f, 1f, 0.4f, 0.25f);
    public enum ownerTypes
    {
        player,
        enemy,
        neutral
    }
    public ownerTypes ownerType;    

    public CharacterBase parent;
    public CharacterBase spotted;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private Color[] colors;
    private int[] triangles;


    private void Start()
    {
        parent = GetComponentInParent<CharacterBase>();
        SetOwnerType(parent);
        if (ownerType == ownerTypes.enemy)
        {
            this.viewDistance = parent.GetComponent<EnemyClass>().viewDistance;
        }

        mesh = new Mesh();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[rayCount + 2];
        uv = new Vector2[vertices.Length];
        colors = new Color[vertices.Length];
        triangles = new int[rayCount * 3];

        for (int i = 0; i < colors.Length; i++)
            colors[i] = viewColor;

        for (int i = 0; i < rayCount; i++)
        {
            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
    }

    private void LateUpdate()
    {
        Vector3 origin = transform.position;
        float angleIncrease = fov / rayCount;
        float angle = fov / 2f;

        vertices[0] = Vector3.zero;

        spotted = null;
        float spottedDistance = float.MaxValue;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 localDir = GetVectorFromAngle(angle);
            Vector3 worldDir = transform.TransformDirection(localDir);

            RaycastHit2D hit = Physics2D.Raycast(origin, worldDir, viewDistance, sightBlockerMask);
            float distance = hit.collider == null ? viewDistance : hit.distance;

            if (hit.collider != null && hit.distance < spottedDistance)
            {
                CharacterBase character = hit.collider.GetComponentInParent<CharacterBase>();
                if (character != null && character != parent)
                {
                    spotted = character;
                    spottedDistance = hit.distance;
                }
            }

            vertices[i + 1] = localDir * distance;
            angle -= angleIncrease;
        }

        if (parent is EnemyClass enemy)
        {
            if (spotted != null)
            {
                enemy.targetCharacter = spotted.transform;
                enemy.lastSpottedPosition = spotted.transform.position;
            }
            else
            {
                enemy.targetCharacter = null;
            }
        }        

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }



    public bool CanSee(Vector3 worldPoint)
    {
        Vector3 origin = transform.position;
        Vector3 toTarget = worldPoint - origin;
        toTarget.z = 0f;

        float distance = toTarget.magnitude;
        if (distance > viewDistance)
            return false;

        if (Vector3.Angle(transform.right, toTarget) > fov / 2f)
            return false;

        return Physics2D.Raycast(origin, toTarget.normalized, distance, sightBlockerMask).collider == null;
    }

    public bool CanSee(Transform target)
    {
        if (target == null)
            return false;

        Vector3 origin = transform.position;
        Vector3 toTarget = target.position - origin;
        toTarget.z = 0f;

        float distance = toTarget.magnitude;
        if (distance > viewDistance)
            return false;

        if (Vector3.Angle(transform.right, toTarget) > fov / 2f)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(origin, toTarget.normalized, distance, sightBlockerMask);
        return hit.collider == null || hit.collider.transform.IsChildOf(target);
    }

    public void SetAimDirection(Vector3 worldDirection)
    {
        if (worldDirection.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(worldDirection.y, worldDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public void SetOwnerType(CharacterBase ownerScript)
    {
        if (ownerScript is EnemyClass)
        {
            ownerType = ownerTypes.enemy;
            sightBlockerMask = obstacleMask | LayerMask.GetMask("Player");
        }
        else if (ownerScript is PC_Controller)
        {
            ownerType = ownerTypes.player;
            sightBlockerMask = obstacleMask | LayerMask.GetMask("Enemy");
        }
        else
        {
            ownerType = ownerTypes.neutral;
            sightBlockerMask = obstacleMask | LayerMask.GetMask("Player", "Enemy");

        }
    }
}
