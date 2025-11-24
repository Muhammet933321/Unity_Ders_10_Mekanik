using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ProceduralTrack : MonoBehaviour
{
    [Header("Dimensions")]
    [Tooltip("Track length in meters")] public float length = 500f;
    [Tooltip("Track width in meters")] public float width = 6f;
    [Range(16, 4000)] public int segments = 600;

    [Header("Curves & Hills")]
    [Tooltip("Left-right curve amplitude (meters)")] public float curveAmplitude = 25f;
    [Tooltip("Left-right curve frequency (cycles over full length)")] public float curveCycles = 2f;
    [Tooltip("Up-down height amplitude (meters)")] public float heightAmplitude = 3.5f;
    [Tooltip("Up-down height frequency (cycles over full length)")] public float heightCycles = 6f;

    Mesh mesh;

    void Awake() { EnsureComponents(); Generate(); }
    void OnValidate() { EnsureComponents(); Generate(); }

    void EnsureComponents()
    {
        var mf = GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mesh = new Mesh();
            mesh.name = "ProceduralTrack";
            mf.sharedMesh = mesh;
        }
        else mesh = mf.sharedMesh;

        var mc = GetComponent<MeshCollider>();
        if (mc.sharedMesh != mf.sharedMesh) mc.sharedMesh = mf.sharedMesh;
    }

    public void Generate()
    {
        if (mesh == null) return;
        int vertsCount = (segments + 1) * 2;
        Vector3[] verts = new Vector3[vertsCount];
        Vector3[] norms = new Vector3[vertsCount];
        Vector2[] uvs = new Vector2[vertsCount];
        int[] tris = new int[segments * 6];

        float step = length / segments;
        for (int i = 0; i <= segments; i++)
        {
            float z = i * step;
            float t = (float)i / segments; // 0..1 along the track

            // Centerline with curves and hills
            float x = Mathf.Sin(t * Mathf.PI * 2f * curveCycles) * curveAmplitude;
            float y = Mathf.Sin(t * Mathf.PI * 2f * heightCycles) * heightAmplitude;
            Vector3 center = new Vector3(x, y, z);

            // Tangent for left-right offset: approximate via derivative
            float t2 = Mathf.Clamp01((float)(i + 1) / segments);
            float x2 = Mathf.Sin(t2 * Mathf.PI * 2f * curveCycles) * curveAmplitude;
            float y2 = Mathf.Sin(t2 * Mathf.PI * 2f * heightCycles) * heightAmplitude;
            Vector3 centerNext = new Vector3(x2, y2, (i + 1) * step);
            Vector3 forward = (centerNext - center).normalized;
            Vector3 left = Vector3.Cross(Vector3.up, forward).normalized; // approximate local left

            Vector3 vL = center + left * (width * 0.5f);
            Vector3 vR = center - left * (width * 0.5f);

            int vi = i * 2;
            verts[vi + 0] = vL;
            verts[vi + 1] = vR;
            norms[vi + 0] = Vector3.up;
            norms[vi + 1] = Vector3.up;
            uvs[vi + 0] = new Vector2(0, t * (length / 2f));
            uvs[vi + 1] = new Vector2(1, t * (length / 2f));
        }

        int ti = 0;
        for (int i = 0; i < segments; i++)
        {
            int vi = i * 2;
            // quad: (L0,R0,L1,R1)
            tris[ti++] = vi + 0;
            tris[ti++] = vi + 2;
            tris[ti++] = vi + 1;

            tris[ti++] = vi + 1;
            tris[ti++] = vi + 2;
            tris[ti++] = vi + 3;
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();

        // Update collider
        var mc = GetComponent<MeshCollider>();
        mc.sharedMesh = null; // force refresh
        mc.sharedMesh = mesh;
    }
}
