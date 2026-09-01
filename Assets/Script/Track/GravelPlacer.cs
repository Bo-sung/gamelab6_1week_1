using UnityEngine;
using UnityEngine.UIElements;

public class GravelPlacer : MonoBehaviour
{
    [SerializeField]
    int gravelHorizontalCount = 5;
    [SerializeField]
    int gravelVerticalCount = 5;
    [SerializeField]
    float gravelSpacing = 1.0f;
    [SerializeField]
    float gravelScale = 0.1f;
    [SerializeField]
    float gravelScaleMin = 0.5f;
    [SerializeField]
    float gravelScaleMax = 1.5f;
    [SerializeField]
    GameObject gravelPrefab;
    [SerializeField]
    float gravelAreaWidth;
    [SerializeField]
    float gravelAreaHeight;

    private void Awake()
    {
        if (gravelPrefab == null)
        {
            gravelPrefab = new GameObject("GravelPrefab");
            var mf = gravelPrefab.AddComponent<MeshFilter>();
            var mr = gravelPrefab.AddComponent<MeshRenderer>();
            mf.mesh = CreateCubeMesh();
            mr.material = new Material(Shader.Find("lit"));
            var bc = gravelPrefab.AddComponent<BoxCollider>();
            bc.enabled = true;

        }
        PlaceGravelInArea(transform.position, gravelAreaWidth, gravelAreaHeight);
    }

    // 사각형 영역 내 큐브를 배치하고. 각 큐브에 랜덤 크기와 랜덤 회전을 적용해서 자연스러운 자갈 배치 효과를 구현

    public void PlaceGravelInArea(Vector3 center, float width, float height)
    {
        Vector3 startPosition = center - new Vector3(width / 2, 0, height / 2);
        Vector3 endPosition = center + new Vector3(width / 2, 0, height / 2);
        PlaceGravel();
    }

    public void PlaceGravel()
    {
        for (int j = 0; j < gravelHorizontalCount; j++)
        {
            for (int k = 0; k < gravelVerticalCount; k++)
            {
                Vector3 offset = new Vector3(j * gravelSpacing, 0, k * gravelSpacing);
                // 랜덤크기 및 랜덤회전 적용
                float randomScale = Random.Range(gravelScaleMin, gravelScaleMax);
                Quaternion randomRotation = Random.rotation;
                Instantiate(gravelPrefab, transform.position + offset, randomRotation, transform).transform.localScale = Vector3.one * gravelScale * randomScale;
            }
        }
    }

    private Mesh CreateCubeMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f)
        };
        int[] triangles = {
            0, 2, 1,
            0, 3, 2,
            4, 5, 6,
            4, 6, 7,
            4, 7, 3,
            4, 3, 0,
            1, 2, 6,
            1, 6, 5,
            2, 3, 7,
            2, 7, 6,
            4, 0, 1,
            4, 1, 5
        };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
