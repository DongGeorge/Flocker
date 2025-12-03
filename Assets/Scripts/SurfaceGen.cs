using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor; // Required for saving assets
#endif

public class SurfaceGen : MonoBehaviour
{
    // Start is called before the first frame update
    public Material defaultMaterial;
    public Material red;
    public Material green;
    public Material blue;
    public bool showSampledPoints = false;
    public bool showControlPoints = false;
    public bool saveObject = false;
    public float stepSize = 0.1f;
    private Vector3 controlPointScale = new Vector3(0.25f, 0.25f, 0.25f);
    private Vector3 samplePointScale = new Vector3(0.1f, 0.1f, 0.1f);
    void Start()
    {
        CreateScalyWing();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Tests ---------------------------------------------
    void TestBezierLine()
    {
        float stepSize = 0.01f;
        Vector3[] controlPoints = new Vector3[4]
        {
            new Vector3(0f, 0f, 0f),    // P0: Starting point
            new Vector3(-20f, 2f, 0f),    // P1: First interior point
            new Vector3(3f, 5f, 0f),    // P2: Second interior point
            new Vector3(4f, 0f, 0f)     // P3: Ending point
        };

        PlotPoints(controlPoints, new Vector3(1f, 1f, 1f), red);

        int numPoints = (int)(1.0f / stepSize);
        Vector3[] samples = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            samples[i] = PointOnBezier(stepSize * i,
                controlPoints[0],
                controlPoints[1],
                controlPoints[2],
                controlPoints[3]);
        }

        Vector3 newScale = new Vector3(0.25f, 0.25f, 0.25f);
        PlotPoints(samples, newScale, blue);
    }

    void TestBezierPatchPoints()
    {
        float stepSize = 0.025f;
        Vector3[,] controlPoints = new Vector3[4, 4];
        Vector3[] controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(-2f, 2f, 0f),
            new Vector3(3f, 5f, 0f),
            new Vector3(5f, 0f, 0f)
        };
        Vector3[] controlPoints1 = new Vector3[4]
        {
            new Vector3(-2f, 0f, 1f),
            new Vector3(-2f, 4f, 1f),
            new Vector3(3f, 5f, 1f),
            new Vector3(3f, 0f, 1f)
        };
        Vector3[] controlPoints2 = new Vector3[4]
        {
            new Vector3(-5f, 0f, 2f),
            new Vector3(-2f, 6f, 2f),
            new Vector3(3f, 5f, 2f),
            new Vector3(3f, 0f, 2f)
        };
        Vector3[] controlPoints3 = new Vector3[4]
        {
            new Vector3(0f, 0f, 3f),
            new Vector3(-2f, 4f, 3f),
            new Vector3(3f, 5f, 3f),
            new Vector3(4f, 0f, 3f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }

        PlotMatrixPoints(controlPoints, new Vector3(1f, 1f, 1f), red);

        int numPoints = (int)(1.0f / stepSize) + 2;
        Vector3[] samples = new Vector3[(int)Math.Pow(numPoints, 2)];

        for (int i = 0; i < numPoints; i++)
        {
            for (int j = 0; j < numPoints; j++)
            {
                samples[i * numPoints + j] = PointOnPatch(i * stepSize, j * stepSize, controlPoints);
            }
        }

        Vector3 newScale = new Vector3(0.25f, 0.25f, 0.25f);
        PlotPoints(samples, newScale, blue);
    }

    void TestBezierPatch()
    {
        float stepSize = 0.05f;
        List<Vector3> allVertices = new List<Vector3>();
        List<int> allTriangles = new List<int>();

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Control Point Definition
        Vector3[] controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(-2f, 2f, 0f),
            new Vector3(3f, 5f, 0f),
            new Vector3(5f, 0f, 0f)
        };
        Vector3[] controlPoints1 = new Vector3[4]
        {
            new Vector3(-2f, 0f, 1f),
            new Vector3(-2f, 4f, 1f),
            new Vector3(3f, 5f, 1f),
            new Vector3(3f, 0f, 1f)
        };
        Vector3[] controlPoints2 = new Vector3[4]
        {
            new Vector3(-5f, 0f, 2f),
            new Vector3(-2f, 6f, 2f),
            new Vector3(3f, 5f, 2f),
            new Vector3(3f, 0f, 2f)
        };
        Vector3[] controlPoints3 = new Vector3[4]
        {
            new Vector3(0f, 0f, 3f),
            new Vector3(-2f, 4f, 3f),
            new Vector3(3f, 5f, 3f),
            new Vector3(4f, 0f, 3f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        #endregion

        PlotMatrixPoints(controlPoints, new Vector3(0.25f, 0.25f, 0.25f), red);

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = CreateSurfaceMech(controlPoints, stepSize);
        addTriangles(allTriangles, triangles, allVertices.Count);
        addVertices(allVertices, vertices);

        #region Control Point Definition
        controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 3f),
            new Vector3(-2f, 4f, 3f),
            new Vector3(3f, 5f, 3f),
            new Vector3(4f, 0f, 3f)
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(5f, 0f, 4f),
            new Vector3(-2f, 2f, 4f),
            new Vector3(3f, 5f, 4f),
            new Vector3(5f, 0f, 4f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(5f, -2f, 5f),
            new Vector3(-2f, 0f, 5f),
            new Vector3(3f, 3f, 5f),
            new Vector3(4f, -2f, 5f)
        };
        controlPoints3 = new Vector3[4]
        {
            new Vector3(4f, -4f, 6f),
            new Vector3(-2f, -1f, 6f),
            new Vector3(3f, -1f, 6f),
            new Vector3(4f, -4f, 6f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        #endregion

        PlotMatrixPoints(controlPoints, new Vector3(0.25f, 0.25f, 0.25f), red);

        (vertices, triangles, uvs) = CreateSurfaceMech(controlPoints, stepSize);
        addTriangles(allTriangles, triangles, allVertices.Count);
        addVertices(allVertices, vertices);

        PlotPoints(allVertices.ToArray(), new Vector3(0.1f, 0.1f, 0.1f), blue);

        Mesh mesh = new Mesh();
        mesh.vertices = allVertices.ToArray();
        mesh.triangles = allTriangles.ToArray();
        mesh.RecalculateNormals();

        GameObject surface = new GameObject("Surface");
        surface.transform.position = Vector3.zero;
        surface.AddComponent<MeshFilter>().mesh = mesh;
        surface.AddComponent<MeshRenderer>();

        MeshRenderer renderer = surface.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = defaultMaterial;
    }

    void TestGenSurface()
    {
        float stepSize = 0.05f;
        List<Vector3[,]> allControlPoints = new List<Vector3[,]>();

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Control Point Definition
        Vector3[] controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(-2f, 2f, 0f),
            new Vector3(3f, 5f, 0f),
            new Vector3(5f, 0f, 0f)
        };
        Vector3[] controlPoints1 = new Vector3[4]
        {
            new Vector3(-2f, 0f, 1f),
            new Vector3(-2f, 4f, 1f),
            new Vector3(3f, 5f, 1f),
            new Vector3(3f, 0f, 1f)
        };
        Vector3[] controlPoints2 = new Vector3[4]
        {
            new Vector3(-5f, 0f, 2f),
            new Vector3(-2f, 6f, 2f),
            new Vector3(3f, 5f, 2f),
            new Vector3(3f, 0f, 2f)
        };
        Vector3[] controlPoints3 = new Vector3[4]
        {
            new Vector3(0f, 0f, 3f),
            new Vector3(-2f, 4f, 3f),
            new Vector3(3f, 5f, 3f),
            new Vector3(4f, 0f, 3f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPoints);

        Vector3[,] controlPointsv1 = new Vector3[4, 4];
        #region Control Point Definition
        controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 3f),
            new Vector3(-2f, 4f, 3f),
            new Vector3(3f, 5f, 3f),
            new Vector3(4f, 0f, 3f)
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(5f, 0f, 4f),
            new Vector3(-2f, 2f, 4f),
            new Vector3(3f, 5f, 4f),
            new Vector3(5f, 0f, 4f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(5f, -2f, 5f),
            new Vector3(-2f, 0f, 5f),
            new Vector3(3f, 3f, 5f),
            new Vector3(4f, -2f, 5f)
        };
        controlPoints3 = new Vector3[4]
        {
            new Vector3(4f, -4f, 6f),
            new Vector3(-2f, -1f, 6f),
            new Vector3(3f, -1f, 6f),
            new Vector3(4f, -4f, 6f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv1[0, i] = controlPoints0[i];
            controlPointsv1[1, i] = controlPoints1[i];
            controlPointsv1[2, i] = controlPoints2[i];
            controlPointsv1[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv1);

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = GenerateMultipleSurfaces(allControlPoints, stepSize);

        RenderSurface(vertices, triangles, defaultMaterial, uvs);
    }
    #endregion // ---------------------------------------------------------------------------------

    #region Point Displays ------------------------------------
    void PlotMatrixPoints(Vector3[,] points, Vector3 scale, Material mat)
    {
        for (int r = 0; r < points.GetLength(0); r++)
        {
            for (int c = 0; c < points.GetLength(1); c++)
            {
                GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newSphere.transform.position = points[r, c];
                newSphere.transform.localScale = scale;

                MeshRenderer renderer = newSphere.GetComponent<MeshRenderer>();
                renderer.material = mat;
            }
        }
    }

    void PlotPoints(Vector3[] points, Vector3 scale, Material mat)
    {
        foreach (Vector3 point in points)
        {
            GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newSphere.transform.position = point;
            newSphere.transform.localScale = scale;

            MeshRenderer renderer = newSphere.GetComponent<MeshRenderer>();
            renderer.material = mat;
        }
    }
    #endregion // ---------------------------------------------------------------------------------

    #region Bezier Functions ----------------------------------
    public Vector3 PointOnBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float p0Weight = (float)Math.Pow(1.0f - t, 3.0f);
        float p1Weight = 3.0f * t * (float)Math.Pow(1.0f - t, 2.0f);
        float p2Weight = 3.0f * (float)Math.Pow(t, 2.0f) * (1.0f - t);
        float p3Weight = (float)Math.Pow(t, 3.0f);

        Vector3 point =
            p0Weight * p0 +
            p1Weight * p1 +
            p2Weight * p2 +
            p3Weight * p3;
        return point;
    }

    public Vector3 PointOnPatch(float t, float s, Vector3[,] controlPoints)
    {
        Vector3 q0 = PointOnBezier(t,
            controlPoints[0, 0],
            controlPoints[0, 1],
            controlPoints[0, 2],
            controlPoints[0, 3]);
        Vector3 q1 = PointOnBezier(t,
            controlPoints[1, 0],
            controlPoints[1, 1],
            controlPoints[1, 2],
            controlPoints[1, 3]);
        Vector3 q2 = PointOnBezier(t,
            controlPoints[2, 0],
            controlPoints[2, 1],
            controlPoints[2, 2],
            controlPoints[2, 3]);
        Vector3 q3 = PointOnBezier(t,
            controlPoints[3, 0],
            controlPoints[3, 1],
            controlPoints[3, 2],
            controlPoints[3, 3]);

        return PointOnBezier(s, q0, q1, q2, q3);
    }

    public (Vector3[], int[], Vector2[]) CreateSurfaceMech(Vector3[,] controlPoints, float stepSize)
    {
        int numSteps = (int)(1.0f / stepSize) + 1;
        int numVerticesPerSide = numSteps + 1;

        int numTotalVertices = (int)Math.Pow(numVerticesPerSide, 2);
        Vector3[] vertices = new Vector3[numTotalVertices];

        int numTotalTriangles = (int)Math.Pow(numSteps, 2) * 6;
        int[] triangles = new int[numTotalTriangles];

        Vector2[] uvs = new Vector2[numTotalVertices];

        for (int i = 0; i < numVerticesPerSide; i++)
        {
            for (int j = 0; j < numVerticesPerSide; j++)
            {
                float u = i * stepSize;
                float v = j * stepSize;

                vertices[i * numVerticesPerSide + j] = PointOnPatch(u, v, controlPoints);

                uvs[i * numVerticesPerSide + j] = new Vector2((float)i / (numVerticesPerSide - 1), (float)j / (numVerticesPerSide - 1));
            }
        }

        int triIndex = 0;

        for (int i = 0; i < numSteps; i++)
        {
            for (int j = 0; j < numSteps; j++)
            {
                int v_cols = numVerticesPerSide;

                int v0 = i * v_cols + j;
                int v1 = v0 + 1;
                int v2 = (i + 1) * v_cols + j;
                int v3 = v2 + 1;

                triangles[triIndex++] = v0;
                triangles[triIndex++] = v1;
                triangles[triIndex++] = v2;

                triangles[triIndex++] = v2;
                triangles[triIndex++] = v1;
                triangles[triIndex++] = v3;
            }
        }

        return (vertices, triangles, uvs);
    }
    #endregion // ---------------------------------------------------------------------------------

    #region Helper Methods ------------------------------------
    void addVertices(List<Vector3> oldVertices, Vector3[] newVertices)
    {
        for (int i = 0; i < newVertices.Length; i++)
        {
            oldVertices.Add(newVertices[i]);
        }
    }

    void addTriangles(List<int> oldTriangles, int[] newTriangles, int vertexOffset)
    {
        for (int i = 0; i < newTriangles.Length; i++)
        {
            oldTriangles.Add(vertexOffset + newTriangles[i]);
        }
    }

    void addUVs(List<Vector2> oldUVs, Vector2[] newUVs)
    {
        for (int i = 0; i < newUVs.Length; i++)
        {
            oldUVs.Add(newUVs[i]);
        }
    }
    
    public (Vector3[], int[], Vector2[]) GenerateMultipleSurfaces(List<Vector3[,]> controlPoints, float stepSize)
    {
        List<Vector3> allVertices = new List<Vector3>();
        List<int> allTriangles = new List<int>();
        List<Vector2> allUVs = new List<Vector2>();

        foreach (Vector3[,] controlPointSet in controlPoints)
        {
            (Vector3[] vertices, int[] triangles, Vector2[] uvs) = CreateSurfaceMech(controlPointSet, stepSize);
            addTriangles(allTriangles, triangles, allVertices.Count);
            addVertices(allVertices, vertices);
            addUVs(allUVs, uvs);
        }

        return (allVertices.ToArray(), allTriangles.ToArray(), allUVs.ToArray());
    }

    public void RenderSurface(Vector3[] vertices, int[] triangles, Material mat, Vector2[] uvs = null)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        if (uvs != null)
		{
            mesh.uv = uvs;
		}
        mesh.RecalculateNormals();

        GameObject surface = new GameObject("Surface");
        surface.transform.position = Vector3.zero;
        surface.AddComponent<MeshFilter>().mesh = mesh;
        surface.AddComponent<MeshRenderer>();

        MeshRenderer renderer = surface.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = mat;

        if (!saveObject)
		{
            return;
		}
#if UNITY_EDITOR
            // 1. Define paths (ensure the "Assets/Generated" folder exists or change this path)
            // Using a timestamp or random number prevents overwriting if you generate multiple
            string meshPath = "Assets/meshes/ProceduralMesh_" + System.DateTime.Now.Ticks + ".asset";
            string prefabPath = "Assets/prefabs/ProceduralSurface_" + System.DateTime.Now.Ticks + ".prefab";

            // 2. Save the Mesh as a real file on disk
            AssetDatabase.CreateAsset(mesh, meshPath);
            AssetDatabase.SaveAssets();

            // 3. Save the GameObject as a Prefab linked to that new mesh
            PrefabUtility.SaveAsPrefabAsset(surface, prefabPath);

            Debug.Log($"Saved Prefab to {prefabPath}");
#endif
    }
    #endregion // ---------------------------------------------------------------------------------

    #region Creature Parts ------------------------------------
    void CreateSquare()
    {
        float stepSize = 0.1f;
        List<Vector3[,]> allControlPoints = new List<Vector3[,]>();
        Vector3[] controlPoints0, controlPoints1, controlPoints2, controlPoints3;

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Control Point Definition
        controlPoints0 = new Vector3[4]
        {
            new Vector3(3f, 0f, 0f),
            new Vector3(3f, 0f, 1f),
            new Vector3(3f, 0f, 2f),
            new Vector3(3f, 0f, 3f)
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(3f, 1f, 0f),
            new Vector3(3f, 1f, 1f),
            new Vector3(3f, 1f, 2f),
            new Vector3(3f, 1f, 3f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(3f, 2f, 0f),
            new Vector3(3f, 2f, 1f),
            new Vector3(3f, 2f, 2f),
            new Vector3(3f, 2f, 3f)
        };
        controlPoints3 = new Vector3[4]
        {
            new Vector3(3f, 3f, 0f),
            new Vector3(3f, 3f, 1f),
            new Vector3(3f, 3f, 2f),
            new Vector3(3f, 3f, 3f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPoints);

        Vector3[,] controlPointsv1 = new Vector3[4, 4];
        #region Control Point Definition
        controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 2f),
            new Vector3(0f, 0f, 3f)
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 1f, 1f),
            new Vector3(0f, 1f, 2f),
            new Vector3(0f, 1f, 3f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(0f, 2f, 0f),
            new Vector3(0f, 2f, 1f),
            new Vector3(0f, 2f, 2f),
            new Vector3(0f, 2f, 3f)
        };
        controlPoints3 = new Vector3[4]
        {
            new Vector3(0f, 3f, 0f),
            new Vector3(0f, 3f, 1f),
            new Vector3(0f, 3f, 2f),
            new Vector3(0f, 3f, 3f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv1[0, i] = controlPoints3[i];
            controlPointsv1[1, i] = controlPoints2[i];
            controlPointsv1[2, i] = controlPoints1[i];
            controlPointsv1[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv1);

        Vector3[,] controlPointsv2 = new Vector3[4, 4];
        #region Control Point Definition
        controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 2f),
            new Vector3(0f, 0f, 3f)
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 1f),
            new Vector3(1f, 0f, 2f),
            new Vector3(1f, 0f, 3f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(2f, 0f, 0f),
            new Vector3(2f, 0f, 1f),
            new Vector3(2f, 0f, 2f),
            new Vector3(2f, 0f, 3f)
        };
        controlPoints3 = new Vector3[4]
        {
            new Vector3(3f, 0f, 0f),
            new Vector3(3f, 0f, 1f),
            new Vector3(3f, 0f, 2f),
            new Vector3(3f, 0f, 3f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv2[0, i] = controlPoints0[i];
            controlPointsv2[1, i] = controlPoints1[i];
            controlPointsv2[2, i] = controlPoints2[i];
            controlPointsv2[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv2);

        Vector3[,] controlPointsv3 = new Vector3[4, 4];
        #region Control Point Definition
        controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 3f, 0f),
            new Vector3(0f, 3f, 1f),
            new Vector3(0f, 3f, 2f),
            new Vector3(0f, 3f, 3f)
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(1f, 3f, 0f),
            new Vector3(1f, 3f, 1f),
            new Vector3(1f, 3f, 2f),
            new Vector3(1f, 3f, 3f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(2f, 3f, 0f),
            new Vector3(2f, 3f, 1f),
            new Vector3(2f, 3f, 2f),
            new Vector3(2f, 3f, 3f)
        };
        controlPoints3 = new Vector3[4]
        {
            new Vector3(3f, 3f, 0f),
            new Vector3(3f, 3f, 1f),
            new Vector3(3f, 3f, 2f),
            new Vector3(3f, 3f, 3f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv3[0, i] = controlPoints3[i];
            controlPointsv3[1, i] = controlPoints2[i];
            controlPointsv3[2, i] = controlPoints1[i];
            controlPointsv3[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv3);

        Vector3[,] controlPointsv4 = new Vector3[4, 4];
        #region Control Point Definition
        controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 2f, 0f),
            new Vector3(0f, 3f, 0f)
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(1f, 2f, 0f),
            new Vector3(1f, 3f, 0f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(2f, 0f, 0f),
            new Vector3(2f, 1f, 0f),
            new Vector3(2f, 2f, 0f),
            new Vector3(2f, 3f, 0f)
        };
        controlPoints3 = new Vector3[4]
        {
            new Vector3(3f, 0f, 0f),
            new Vector3(3f, 1f, 0f),
            new Vector3(3f, 2f, 0f),
            new Vector3(3f, 3f, 0f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv4[0, i] = controlPoints3[i];
            controlPointsv4[1, i] = controlPoints2[i];
            controlPointsv4[2, i] = controlPoints1[i];
            controlPointsv4[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv4);

        Vector3[,] controlPointsv5 = new Vector3[4, 4];
        #region Control Point Definition
        controlPoints0 = new Vector3[4]
        {
            new Vector3(0f, 0f, 3f),
            new Vector3(0f, 1f, 3f),
            new Vector3(0f, 2f, 3f),
            new Vector3(0f, 3f, 3f)
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(1f, 0f, 3f),
            new Vector3(1f, 1f, 3f),
            new Vector3(1f, 2f, 3f),
            new Vector3(1f, 3f, 3f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(2f, 0f, 3f),
            new Vector3(2f, 1f, 3f),
            new Vector3(2f, 2f, 3f),
            new Vector3(2f, 3f, 3f)
        };
        controlPoints3 = new Vector3[4]
        {
            new Vector3(3f, 0f, 3f),
            new Vector3(3f, 1f, 3f),
            new Vector3(3f, 2f, 3f),
            new Vector3(3f, 3f, 3f)
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv5[0, i] = controlPoints0[i];
            controlPointsv5[1, i] = controlPoints1[i];
            controlPointsv5[2, i] = controlPoints2[i];
            controlPointsv5[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv5);

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = GenerateMultipleSurfaces(allControlPoints, stepSize);

        RenderSurface(vertices, triangles, defaultMaterial, uvs);
    }

    void CreateSomething()
    {
        List<Vector3[,]> allControlPoints = new List<Vector3[,]>();
        Vector3[] controlPoints0, controlPoints1, controlPoints2, controlPoints3;

        #region Corners
        Vector3 l_t_f_corner = new Vector3(0.3f, 2.7f, 0.3f);
        Vector3 l_b_f_corner = new Vector3(0.3f, 0.3f, 0.3f);

        Vector3 r_t_f_corner = new Vector3(5f, 3f, -2f);
        Vector3 r_b_f_corner = new Vector3(2.7f, 0.3f, 0.3f);

        Vector3 l_t_b_corner = new Vector3(2.7f, 2.7f, 2.7f);
        Vector3 l_b_b_corner = new Vector3(2.7f, 0.3f, 2.7f);

        Vector3 r_t_b_corner = new Vector3(0.3f, 2.7f, 2.7f);
        Vector3 r_b_b_corner = new Vector3(0.3f, 0.3f, 2.7f);
        #endregion

        #region Edges
        Vector3 t_f_1 = new Vector3(1f, 3f, 0f);
        Vector3 t_f_2 = new Vector3(2f, 3.25f, -0.25f);
        Vector3 b_f_1 = new Vector3(1f, 0f, 0f);
        Vector3 b_f_2 = new Vector3(2f, 0f, 0f);

        Vector3 t_l_1 = new Vector3(0f, 3f, 2f);
        Vector3 t_l_2 = new Vector3(0f, 3f, 1f);
        Vector3 b_l_1 = new Vector3(0f, 0f, 2f);
        Vector3 b_l_2 = new Vector3(0f, 0f, 1f);

        Vector3 t_b_1 = new Vector3(2f, 3f, 3f);
        Vector3 t_b_2 = new Vector3(1f, 3f, 3f);
        Vector3 b_b_1 = new Vector3(2f, 0f, 3f);
        Vector3 b_b_2 = new Vector3(1f, 0f, 3f);

        Vector3 t_r_1 = new Vector3(3.25f, 3.25f, 1f);
        Vector3 t_r_2 = new Vector3(3f, 3f, 2f);
        Vector3 b_r_1 = new Vector3(3f, 0f, 1f);
        Vector3 b_r_2 = new Vector3(3f, 0f, 2f);
        #endregion

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Rightside
        controlPoints0 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(3f, 1f, 0f),
            new Vector3(4f, 1f, 1f),
            new Vector3(4f, 1f, 2f),
            new Vector3(3f, 1f, 3f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(3.25f, 2f, -0.25f),
            new Vector3(4f, 2f, 1f),
            new Vector3(4f, 2f, 2f),
            new Vector3(3f, 2f, 3f)
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            l_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPoints);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPoints, controlPointScale, red);
        }

        Vector3[,] controlPointsv1 = new Vector3[4, 4];
        #region Leftside
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            r_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            new Vector3(0f, 1f, 0f),
            new Vector3(-1f, 1f, 1f),
            new Vector3(-1f, 1f, 2f),
            new Vector3(0f, 1f, 3f)
        };
        controlPoints2 = new Vector3[4]
        {
            new Vector3(0f, 2f, 0f),
            new Vector3(-1f, 2f, 1f),
            new Vector3(-1f, 2f, 2f),
            new Vector3(0f, 2f, 3f)
        };
        controlPoints3 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv1[0, i] = controlPoints3[i];
            controlPointsv1[1, i] = controlPoints2[i];
            controlPointsv1[2, i] = controlPoints1[i];
            controlPointsv1[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv1);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv1, controlPointScale, red);
        }

        Vector3[,] controlPointsv2 = new Vector3[4, 4];
        #region Bottom
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            r_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, -1f, 1f),
            new Vector3(1f, -1f, 2f),
            b_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, -1f, 1f),
            new Vector3(2f, -1f, 2f),
            b_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            l_b_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv2[0, i] = controlPoints0[i];
            controlPointsv2[1, i] = controlPoints1[i];
            controlPointsv2[2, i] = controlPoints2[i];
            controlPointsv2[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv2);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv2, controlPointScale, green);
        }

        Vector3[,] controlPointsv3 = new Vector3[4, 4];
        #region Top
        controlPoints0 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            r_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            t_f_1,
            new Vector3(1f, 4f, 1f),
            new Vector3(1f, 4f, 2f),
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            t_f_2,
            new Vector3(2f, 5f, 1f),
            new Vector3(2f, 4f, 2f),
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            l_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv3[0, i] = controlPoints3[i];
            controlPointsv3[1, i] = controlPoints2[i];
            controlPointsv3[2, i] = controlPoints1[i];
            controlPointsv3[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv3);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv3, controlPointScale, green);
        }

        Vector3[,] controlPointsv4 = new Vector3[4, 4];
        #region Front
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 2f, 0f),
            l_t_f_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, 1f, -1f),
            new Vector3(1f, 2f, -1f),
            t_f_1
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, 1f, -1f),
            new Vector3(2f, 2f, -1f),
            t_f_2
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            new Vector3(3f, 1f, 0f),
            new Vector3(3.25f, 2f, -0.25f),
            r_t_f_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv4[0, i] = controlPoints3[i];
            controlPointsv4[1, i] = controlPoints2[i];
            controlPointsv4[2, i] = controlPoints1[i];
            controlPointsv4[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv4);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv4, controlPointScale, blue);
        }

        Vector3[,] controlPointsv5 = new Vector3[4, 4];
        #region Back
        controlPoints0 = new Vector3[4]
        {
            r_b_b_corner,
            new Vector3(0f, 1f, 3f),
            new Vector3(0f, 2f, 3f),
            r_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_b_2,
            new Vector3(1f, 1f, 4f),
            new Vector3(1f, 2f, 4f),
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_b_1,
            new Vector3(2f, 1f, 4f),
            new Vector3(2f, 2f, 4f),
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            l_b_b_corner,
            new Vector3(3f, 1f, 3f),
            new Vector3(3f, 2f, 3f),
            l_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv5[0, i] = controlPoints0[i];
            controlPointsv5[1, i] = controlPoints1[i];
            controlPointsv5[2, i] = controlPoints2[i];
            controlPointsv5[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv5);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv5, controlPointScale, blue);
        }

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = GenerateMultipleSurfaces(allControlPoints, stepSize);

        if (showSampledPoints)
        {
            PlotPoints(vertices, new Vector3(0.1f, 0.1f, 0.1f), blue);
        }

        RenderSurface(vertices, triangles, defaultMaterial, uvs);
    }

    /// <summary>
	/// Use as a template to create other complex objects
	/// </summary>
    void CreateSquareUpdated()
    {
        List<Vector3[,]> allControlPoints = new List<Vector3[,]>();
        Vector3[] controlPoints0, controlPoints1, controlPoints2, controlPoints3;

        #region Corners
        Vector3 l_t_f_corner = new Vector3(0f, 3f, 0f);
        Vector3 l_b_f_corner = new Vector3(0f, 0f, 0f);

        Vector3 l_t_b_corner = new Vector3(0f, 3f, 3f);
        Vector3 l_b_b_corner = new Vector3(0f, 0f, 3f);

        Vector3 r_t_f_corner = new Vector3(3f, 3f, 0f);
        Vector3 r_b_f_corner = new Vector3(3f, 0f, 0f);

        Vector3 r_t_b_corner = new Vector3(3f, 3f, 3f);
        Vector3 r_b_b_corner = new Vector3(3f, 0f, 3f);
        #endregion

        #region Edges
        Vector3 t_f_1 = new Vector3(1f, 3f, 0f);
        Vector3 t_f_2 = new Vector3(2f, 3f, 0f);
        Vector3 b_f_1 = new Vector3(1f, 0f, 0f);
        Vector3 b_f_2 = new Vector3(2f, 0f, 0f);

        Vector3 t_l_1 = new Vector3(0f, 3f, 2f);
        Vector3 t_l_2 = new Vector3(0f, 3f, 1f);
        Vector3 b_l_1 = new Vector3(0f, 0f, 2f);
        Vector3 b_l_2 = new Vector3(0f, 0f, 1f);

        Vector3 t_b_1 = new Vector3(2f, 3f, 3f);
        Vector3 t_b_2 = new Vector3(1f, 3f, 3f);
        Vector3 b_b_1 = new Vector3(2f, 0f, 3f);
        Vector3 b_b_2 = new Vector3(1f, 0f, 3f);

        Vector3 t_r_1 = new Vector3(3f, 3f, 1f);
        Vector3 t_r_2 = new Vector3(3f, 3f, 2f);
        Vector3 b_r_1 = new Vector3(3f, 0f, 1f);
        Vector3 b_r_2 = new Vector3(3f, 0f, 2f);

        Vector3 f_r_1 = new Vector3(3f, 2f, 0f);
        Vector3 f_r_2 = new Vector3(3f, 1f, 0f);
        Vector3 f_l_1 = new Vector3(0f, 2f, 0f);
        Vector3 f_l_2 = new Vector3(0f, 1f, 0f);

        Vector3 back_r_1 = new Vector3(3f, 2f, 3f);
        Vector3 back_r_2 = new Vector3(3f, 1f, 3f);
        Vector3 back_l_1 = new Vector3(0f, 2f, 3f);
        Vector3 back_l_2 = new Vector3(0f, 1f, 3f);
        #endregion

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Rightside -------------------------------- (+X)
        controlPoints0 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_r_2,
            new Vector3(3f, 1f, 1f),
            new Vector3(3f, 1f, 2f),
            back_r_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_r_1,
            new Vector3(3f, 2f, 1f),
            new Vector3(3f, 2f, 2f),
            back_r_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPoints);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPoints, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv1 = new Vector3[4, 4];
        #region Leftside --------------------------------- (-X)
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_l_2,
            new Vector3(0f, 1f, 1f),
            new Vector3(0f, 1f, 2f),
            back_l_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_l_1,
            new Vector3(0f, 2f, 1f),
            new Vector3(0f, 2f, 2f),
            back_l_1
        };
        controlPoints3 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv1[0, i] = controlPoints3[i];
            controlPointsv1[1, i] = controlPoints2[i];
            controlPointsv1[2, i] = controlPoints1[i];
            controlPointsv1[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv1);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv1, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv2 = new Vector3[4, 4];
        #region Bottom ----------------------------------- (-Y)
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, 0f, 1f),
            new Vector3(1f, 0f, 2f),
            b_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, 0f, 1f),
            new Vector3(2f, 0f, 2f),
            b_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv2[0, i] = controlPoints0[i];
            controlPointsv2[1, i] = controlPoints1[i];
            controlPointsv2[2, i] = controlPoints2[i];
            controlPointsv2[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPointsv2);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv2, controlPointScale, green);
        }
        #endregion


        Vector3[,] controlPointsv3 = new Vector3[4, 4];
        #region Top -------------------------------------- (+Y)
        controlPoints0 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            t_f_1,
            new Vector3(1f, 3f, 1f),
            new Vector3(1f, 3f, 2f),
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            t_f_2,
            new Vector3(2f, 3f, 1f),
            new Vector3(2f, 3f, 2f),
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv3[0, i] = controlPoints3[i];
            controlPointsv3[1, i] = controlPoints2[i];
            controlPointsv3[2, i] = controlPoints1[i];
            controlPointsv3[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv3);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv3, controlPointScale, green);
        }
        #endregion

        Vector3[,] controlPointsv4 = new Vector3[4, 4];
        #region Front ------------------------------------ (-Z)
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            f_l_2,
            f_l_1,
            l_t_f_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, 1f, 0f),
            new Vector3(1f, 2f, 0f),
            t_f_1
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, 1f, 0f),
            new Vector3(2f, 2f, 0f),
            t_f_2
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            f_r_2,
            f_r_1,
            r_t_f_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv4[0, i] = controlPoints3[i];
            controlPointsv4[1, i] = controlPoints2[i];
            controlPointsv4[2, i] = controlPoints1[i];
            controlPointsv4[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv4);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv4, controlPointScale, blue);
        }

        Vector3[,] controlPointsv5 = new Vector3[4, 4];
        #region Back ------------------------------------- (+Z)
        controlPoints0 = new Vector3[4]
        {
            l_b_b_corner,
            back_l_2,
            back_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_b_2,
            new Vector3(1f, 1f, 3f),
            new Vector3(1f, 2f, 3f),
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_b_1,
            new Vector3(2f, 1f, 3f),
            new Vector3(2f, 2f, 3f),
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_b_corner,
            back_r_2,
            back_r_1,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv5[0, i] = controlPoints0[i];
            controlPointsv5[1, i] = controlPoints1[i];
            controlPointsv5[2, i] = controlPoints2[i];
            controlPointsv5[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv5);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv5, controlPointScale, blue);
        }

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = GenerateMultipleSurfaces(allControlPoints, stepSize);

        if (showSampledPoints)
        {
            PlotPoints(vertices, new Vector3(0.1f, 0.1f, 0.1f), blue);
        }

        RenderSurface(vertices, triangles, defaultMaterial, uvs);
    }

    void CreateThorax()
    {
        List<Vector3[,]> allControlPoints = new List<Vector3[,]>();
        Vector3[] controlPoints0, controlPoints1, controlPoints2, controlPoints3;

        #region Corners
        Vector3 l_t_f_corner = new Vector3(0.2f, 3f, 0.2f);
        Vector3 l_b_f_corner = new Vector3(0.5f, 0f, 0.25f);

        Vector3 l_t_b_corner = new Vector3(0.2f, 3f, 1.8f);
        Vector3 l_b_b_corner = new Vector3(0.5f, 0f, 1.75f);

        Vector3 r_t_f_corner = new Vector3(2.6f, 3f, 0.2f);
        Vector3 r_b_f_corner = new Vector3(2.6f, 0f, 0.2f);

        Vector3 r_t_b_corner = new Vector3(2.6f, 3f, 1.8f);
        Vector3 r_b_b_corner = new Vector3(2.6f, 0f, 1.8f);
        #endregion

        #region Edges
        Vector3 t_f_1 = new Vector3(1f, 3f, 0f);
        Vector3 t_f_2 = new Vector3(2f, 3f, 0f);
        Vector3 b_f_1 = new Vector3(1f, 0f, 0f);
        Vector3 b_f_2 = new Vector3(2f, 0f, 0f);

        Vector3 t_l_1 = new Vector3(0f, 3f, 1.33f);
        Vector3 t_l_2 = new Vector3(0f, 3f, 0.66f);
        Vector3 b_l_1 = new Vector3(0.5f, 0f, 1.33f);
        Vector3 b_l_2 = new Vector3(0.5f, 0f, 0.66f);

        Vector3 t_b_1 = new Vector3(2f, 3f, 2f);
        Vector3 t_b_2 = new Vector3(1f, 3f, 2f);
        Vector3 b_b_1 = new Vector3(2f, 0f, 2f);
        Vector3 b_b_2 = new Vector3(1f, 0f, 2f);

        Vector3 t_r_1 = new Vector3(3f, 3f, 0.66f);
        Vector3 t_r_2 = new Vector3(3f, 3f, 1.33f);
        Vector3 b_r_1 = new Vector3(3f, 0f, 0.66f);
        Vector3 b_r_2 = new Vector3(3f, 0f, 1.33f);

        Vector3 f_r_1 = new Vector3(3f, 2f, 0f);
        Vector3 f_r_2 = new Vector3(3f, 1f, 0f);
        Vector3 f_l_1 = new Vector3(0f, 2f, 0f);
        Vector3 f_l_2 = new Vector3(0f, 1f, 0f);

        Vector3 back_r_1 = new Vector3(3f, 2f, 2f);
        Vector3 back_r_2 = new Vector3(3f, 1f, 2f);
        Vector3 back_l_1 = new Vector3(0f, 2f, 2f);
        Vector3 back_l_2 = new Vector3(0f, 1f, 2f);
        #endregion

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Rightside
        controlPoints0 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_r_2,
            new Vector3(4f, 0.5f, 0.66f),
            new Vector3(4f, 0.5f, 1.33f),
            back_r_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_r_1,
            new Vector3(3.3f, 2.5f, 0.66f),
            new Vector3(3.3f, 2.5f, 1.33f),
            back_r_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPoints);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPoints, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv1 = new Vector3[4, 4];
        #region Leftside
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_l_2,
            new Vector3(0f, 1f, 0.66f),
            new Vector3(0f, 1f, 1.33f),
            back_l_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_l_1,
            new Vector3(0f, 2f, 0.66f),
            new Vector3(0f, 2f, 1.33f),
            back_l_1
        };
        controlPoints3 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv1[0, i] = controlPoints3[i];
            controlPointsv1[1, i] = controlPoints2[i];
            controlPointsv1[2, i] = controlPoints1[i];
            controlPointsv1[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv1);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv1, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv2 = new Vector3[4, 4];
        #region Bottom
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, -1f, 0.66f),
            new Vector3(1f, -1f, 1.33f),
            b_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, -1f, 0.66f),
            new Vector3(2f, -1f, 1.33f),
            b_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv2[0, i] = controlPoints0[i];
            controlPointsv2[1, i] = controlPoints1[i];
            controlPointsv2[2, i] = controlPoints2[i];
            controlPointsv2[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPointsv2);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv2, controlPointScale, green);
        }
        #endregion


        Vector3[,] controlPointsv3 = new Vector3[4, 4];
        #region Top
        controlPoints0 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            t_f_1,
            new Vector3(0.5f, 15f, 0.66f),
            new Vector3(0.5f, 15f, 1.33f),
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            t_f_2,
            new Vector3(2.5f, 7f, 0.66f),
            new Vector3(2.5f, 7f, 1.33f),
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv3[0, i] = controlPoints3[i];
            controlPointsv3[1, i] = controlPoints2[i];
            controlPointsv3[2, i] = controlPoints1[i];
            controlPointsv3[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv3);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv3, controlPointScale, green);
        }
        #endregion

        Vector3[,] controlPointsv4 = new Vector3[4, 4];
        #region Front
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            f_l_2,
            f_l_1,
            l_t_f_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, 1f, 0f),
            new Vector3(1f, 2f, 0f),
            t_f_1
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, 1f, 0f),
            new Vector3(2f, 2f, 0f),
            t_f_2
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            f_r_2,
            f_r_1,
            r_t_f_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv4[0, i] = controlPoints3[i];
            controlPointsv4[1, i] = controlPoints2[i];
            controlPointsv4[2, i] = controlPoints1[i];
            controlPointsv4[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv4);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv4, controlPointScale, blue);
        }

        Vector3[,] controlPointsv5 = new Vector3[4, 4];
        #region Back
        controlPoints0 = new Vector3[4]
        {
            l_b_b_corner,
            back_l_2,
            back_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_b_2,
            new Vector3(1f, 1f, 2f), // Z=2f
            new Vector3(1f, 2f, 2f), // Z=2f
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_b_1,
            new Vector3(2f, 1f, 2f), // Z=2f
            new Vector3(2f, 2f, 2f), // Z=2f
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_b_corner,
            back_r_2,
            back_r_1,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv5[0, i] = controlPoints0[i];
            controlPointsv5[1, i] = controlPoints1[i];
            controlPointsv5[2, i] = controlPoints2[i];
            controlPointsv5[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv5);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv5, controlPointScale, blue);
        }

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = GenerateMultipleSurfaces(allControlPoints, stepSize);

        if (showSampledPoints)
        {
            PlotPoints(vertices, new Vector3(0.1f, 0.1f, 0.1f), blue);
        }

        RenderSurface(vertices, triangles, defaultMaterial, uvs);
    }

    void CreateScalyWing()
    {
        List<Vector3[,]> allControlPoints = new List<Vector3[,]>();
        Vector3[] controlPoints0, controlPoints1, controlPoints2, controlPoints3;

        #region Corners
        Vector3 l_t_f_corner = new Vector3(0.2f, 3f, 0.2f);
        Vector3 l_b_f_corner = new Vector3(0.75f, 0f, 0.5f);

        Vector3 l_t_b_corner = new Vector3(0.2f, 3f, 1.8f);
        Vector3 l_b_b_corner = new Vector3(0.5f, 0f, 1.75f);

        Vector3 r_t_f_corner = new Vector3(2.6f, 3f, 0.2f);
        Vector3 r_b_f_corner = new Vector3(2.3f, 0f, 0.4f);

        Vector3 r_t_b_corner = new Vector3(2.6f, 3f, 1.8f);
        Vector3 r_b_b_corner = new Vector3(2.6f, 0f, 1.8f);
        #endregion

        #region Edges
        Vector3 t_f_1 = new Vector3(1f, 3f, 0f);
        Vector3 t_f_2 = new Vector3(2f, 3f, 0f);
        Vector3 b_f_1 = new Vector3(1.2f, 0f, 0.2f);
        Vector3 b_f_2 = new Vector3(1.8f, 0f, 0.2f);

        Vector3 t_l_1 = new Vector3(0f, 3f, 1.33f);
        Vector3 t_l_2 = new Vector3(0f, 3f, 0.66f);
        Vector3 b_l_1 = new Vector3(0.5f, 0f, 1.33f);
        Vector3 b_l_2 = new Vector3(0.5f, 0f, 0.66f);

        Vector3 t_b_1 = new Vector3(2f, 3f, 2f);
        Vector3 t_b_2 = new Vector3(1f, 3f, 2f);
        Vector3 b_b_1 = new Vector3(2f, 0f, 2f);
        Vector3 b_b_2 = new Vector3(1f, 0f, 2f);

        Vector3 t_r_1 = new Vector3(3f, 3f, 0.66f);
        Vector3 t_r_2 = new Vector3(3f, 3f, 1.33f);
        Vector3 b_r_1 = new Vector3(3f, 0f, 0.66f);
        Vector3 b_r_2 = new Vector3(3f, 0f, 1.33f);

        Vector3 f_r_1 = new Vector3(3f, 2f, 0f);
        Vector3 f_r_2 = new Vector3(3f, 1f, 0f);
        Vector3 f_l_1 = new Vector3(0f, 2f, 0f);
        Vector3 f_l_2 = new Vector3(0f, 1f, 0f);

        Vector3 back_r_1 = new Vector3(3f, 2f, 2f);
        Vector3 back_r_2 = new Vector3(3f, 1f, 2f);
        Vector3 back_l_1 = new Vector3(0f, 2f, 2f);
        Vector3 back_l_2 = new Vector3(0f, 1f, 2f);
        #endregion

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Rightside
        controlPoints0 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_r_2,
            new Vector3(4f, 0.5f, 0.66f),
            new Vector3(4f, 0.5f, 1.33f),
            back_r_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_r_1,
            new Vector3(3.3f, 2.5f, 0.66f),
            new Vector3(3.3f, 2.5f, 1.33f),
            back_r_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPoints);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPoints, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv1 = new Vector3[4, 4];
        #region Leftside
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_l_2,
            new Vector3(0f, 1f, 0.66f),
            new Vector3(0f, 1f, 1.33f),
            back_l_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_l_1,
            new Vector3(0f, 2f, 0.66f),
            new Vector3(0f, 2f, 1.33f),
            back_l_1
        };
        controlPoints3 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv1[0, i] = controlPoints3[i];
            controlPointsv1[1, i] = controlPoints2[i];
            controlPointsv1[2, i] = controlPoints1[i];
            controlPointsv1[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv1);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv1, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv2 = new Vector3[4, 4];
        #region Bottom
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, -1f, 0.66f),
            new Vector3(1f, -1f, 1.33f),
            b_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, 2f, 0.66f),
            new Vector3(2f, -10f, 3f),
            b_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv2[0, i] = controlPoints0[i];
            controlPointsv2[1, i] = controlPoints1[i];
            controlPointsv2[2, i] = controlPoints2[i];
            controlPointsv2[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPointsv2);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv2, controlPointScale, green);
        }
        #endregion


        Vector3[,] controlPointsv3 = new Vector3[4, 4];
        #region Top
        controlPoints0 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            t_f_1,
            new Vector3(0.5f, 15f, 0.66f),
            new Vector3(0.5f, 15f, 1.33f),
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            t_f_2,
            new Vector3(15f, 7f, 0.66f),
            new Vector3(15f, 7f, 1.33f),
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv3[0, i] = controlPoints3[i];
            controlPointsv3[1, i] = controlPoints2[i];
            controlPointsv3[2, i] = controlPoints1[i];
            controlPointsv3[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv3);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv3, controlPointScale, green);
        }
        #endregion

        Vector3[,] controlPointsv4 = new Vector3[4, 4];
        #region Front
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            f_l_2,
            f_l_1,
            l_t_f_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, 1f, 0f),
            new Vector3(1f, 2f, 0f),
            t_f_1
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, 1f, 0f),
            new Vector3(2f, 2f, 0f),
            t_f_2
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            f_r_2,
            f_r_1,
            r_t_f_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv4[0, i] = controlPoints3[i];
            controlPointsv4[1, i] = controlPoints2[i];
            controlPointsv4[2, i] = controlPoints1[i];
            controlPointsv4[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv4);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv4, controlPointScale, blue);
        }

        Vector3[,] controlPointsv5 = new Vector3[4, 4];
        #region Back
        controlPoints0 = new Vector3[4]
        {
            l_b_b_corner,
            back_l_2,
            back_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_b_2,
            new Vector3(1f, 1f, 2f), // Z=2f
            new Vector3(1f, 2f, 2f), // Z=2f
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_b_1,
            new Vector3(2f, 1f, 2f), // Z=2f
            new Vector3(2f, 2f, 2f), // Z=2f
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_b_corner,
            back_r_2,
            back_r_1,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv5[0, i] = controlPoints0[i];
            controlPointsv5[1, i] = controlPoints1[i];
            controlPointsv5[2, i] = controlPoints2[i];
            controlPointsv5[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv5);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv5, controlPointScale, blue);
        }

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = GenerateMultipleSurfaces(allControlPoints, stepSize);

        if (showSampledPoints)
        {
            PlotPoints(vertices, new Vector3(0.1f, 0.1f, 0.1f), blue);
        }

        RenderSurface(vertices, triangles, defaultMaterial, uvs);
    }

    void CreateHemelytraWing()
    {
        List<Vector3[,]> allControlPoints = new List<Vector3[,]>();
        Vector3[] controlPoints0, controlPoints1, controlPoints2, controlPoints3;

        #region Corners
        Vector3 l_t_f_corner = new Vector3(0.2f, 3f, 0.2f);
        Vector3 l_b_f_corner = new Vector3(0.7f, 0f, 0.25f);

        Vector3 l_t_b_corner = new Vector3(0.2f, 3f, 1.8f);
        Vector3 l_b_b_corner = new Vector3(0.7f, 0f, 1.75f);

        Vector3 r_t_f_corner = new Vector3(2.6f, 3f, 0.2f);
        Vector3 r_b_f_corner = new Vector3(2.6f, 0f, 0.2f);

        Vector3 r_t_b_corner = new Vector3(2.6f, 3f, 1.8f);
        Vector3 r_b_b_corner = new Vector3(2.6f, 0f, 1.8f);
        #endregion

        #region Edges
        Vector3 t_f_1 = new Vector3(1f, 3f, 0f);
        Vector3 t_f_2 = new Vector3(2f, 3f, 0f);
        Vector3 b_f_1 = new Vector3(1f, 0f, 0f);
        Vector3 b_f_2 = new Vector3(2f, 0f, 0f);

        Vector3 t_l_1 = new Vector3(0f, 3f, 1.33f);
        Vector3 t_l_2 = new Vector3(0f, 3f, 0.66f);
        Vector3 b_l_1 = new Vector3(0.5f, 0f, 1.33f);
        Vector3 b_l_2 = new Vector3(0.5f, 0f, 0.66f);

        Vector3 t_b_1 = new Vector3(2f, 3f, 2f);
        Vector3 t_b_2 = new Vector3(1f, 3f, 2f);
        Vector3 b_b_1 = new Vector3(2f, 0f, 2f);
        Vector3 b_b_2 = new Vector3(1f, 0f, 2f);

        Vector3 t_r_1 = new Vector3(3f, 6f, 0.66f);
        Vector3 t_r_2 = new Vector3(3f, 6f, 1.33f);
        Vector3 b_r_1 = new Vector3(3f, 0f, 0.66f);
        Vector3 b_r_2 = new Vector3(3f, 0f, 1.33f);

        Vector3 f_r_1 = new Vector3(3f, 2f, 0f);
        Vector3 f_r_2 = new Vector3(3f, 1f, 0f);
        Vector3 f_l_1 = new Vector3(0f, 2f, 0f);
        Vector3 f_l_2 = new Vector3(0f, 1f, 0f);

        Vector3 back_r_1 = new Vector3(3f, 2f, 2f);
        Vector3 back_r_2 = new Vector3(3f, 1f, 2f);
        Vector3 back_l_1 = new Vector3(0f, 2f, 2f);
        Vector3 back_l_2 = new Vector3(0f, 1f, 2f);
        #endregion

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Rightside
        controlPoints0 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_r_2,
            new Vector3(3.5f, 0.5f, 0.66f),
            new Vector3(3.5f, 0.5f, 1.33f),
            back_r_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_r_1,
            new Vector3(5f, 3.5f, 0.66f),
            new Vector3(5f, 3.5f, 1.33f),
            back_r_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPoints);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPoints, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv1 = new Vector3[4, 4];
        #region Leftside
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_l_2,
            new Vector3(0f, 1f, 0.66f),
            new Vector3(0f, 1f, 1.33f),
            back_l_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_l_1,
            new Vector3(0f, 2f, 0.66f),
            new Vector3(0f, 2f, 1.33f),
            back_l_1
        };
        controlPoints3 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv1[0, i] = controlPoints3[i];
            controlPointsv1[1, i] = controlPoints2[i];
            controlPointsv1[2, i] = controlPoints1[i];
            controlPointsv1[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv1);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv1, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv2 = new Vector3[4, 4];
        #region Bottom
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, -3f, 0.66f),
            new Vector3(1f, -3f, 1.33f),
            b_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, -1f, 0.66f),
            new Vector3(2f, -1f, 1.33f),
            b_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv2[0, i] = controlPoints0[i];
            controlPointsv2[1, i] = controlPoints1[i];
            controlPointsv2[2, i] = controlPoints2[i];
            controlPointsv2[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPointsv2);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv2, controlPointScale, green);
        }
        #endregion


        Vector3[,] controlPointsv3 = new Vector3[4, 4];
        #region Top
        controlPoints0 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            t_f_1,
            new Vector3(0.5f, 15f, 0.66f),
            new Vector3(0.5f, 15f, 1.33f),
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            t_f_2,
            new Vector3(2.5f, 7f, 0.66f),
            new Vector3(2.5f, 7f, 1.33f),
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv3[0, i] = controlPoints3[i];
            controlPointsv3[1, i] = controlPoints2[i];
            controlPointsv3[2, i] = controlPoints1[i];
            controlPointsv3[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv3);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv3, controlPointScale, green);
        }
        #endregion

        Vector3[,] controlPointsv4 = new Vector3[4, 4];
        #region Front
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            f_l_2,
            f_l_1,
            l_t_f_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, 1f, 0f),
            new Vector3(1f, 2f, 0f),
            t_f_1
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, 1f, 0f),
            new Vector3(2f, 2f, 0f),
            t_f_2
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            f_r_2,
            f_r_1,
            r_t_f_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv4[0, i] = controlPoints3[i];
            controlPointsv4[1, i] = controlPoints2[i];
            controlPointsv4[2, i] = controlPoints1[i];
            controlPointsv4[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv4);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv4, controlPointScale, blue);
        }

        Vector3[,] controlPointsv5 = new Vector3[4, 4];
        #region Back
        controlPoints0 = new Vector3[4]
        {
            l_b_b_corner,
            back_l_2,
            back_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_b_2,
            new Vector3(1f, 1f, 2f), // Z=2f
            new Vector3(1f, 2f, 2f), // Z=2f
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_b_1,
            new Vector3(2f, 1f, 2f), // Z=2f
            new Vector3(2f, 2f, 2f), // Z=2f
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_b_corner,
            back_r_2,
            back_r_1,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv5[0, i] = controlPoints0[i];
            controlPointsv5[1, i] = controlPoints1[i];
            controlPointsv5[2, i] = controlPoints2[i];
            controlPointsv5[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv5);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv5, controlPointScale, blue);
        }

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = GenerateMultipleSurfaces(allControlPoints, stepSize);

        if (showSampledPoints)
        {
            PlotPoints(vertices, new Vector3(0.1f, 0.1f, 0.1f), blue);
        }

        RenderSurface(vertices, triangles, defaultMaterial, uvs);
    }

    void CreateBanana()
    {
        List<Vector3[,]> allControlPoints = new List<Vector3[,]>();
        Vector3[] controlPoints0, controlPoints1, controlPoints2, controlPoints3;

        #region Corners
        Vector3 l_t_f_corner = new Vector3(0.2f, 3f, 0.2f);
        Vector3 l_b_f_corner = new Vector3(0.5f, 0f, 0.25f);

        Vector3 l_t_b_corner = new Vector3(0.2f, 3f, 1.8f);
        Vector3 l_b_b_corner = new Vector3(0.5f, 0f, 1.75f);

        Vector3 r_t_f_corner = new Vector3(2.6f, 3f, 0.2f);
        Vector3 r_b_f_corner = new Vector3(2.6f, 0f, 0.2f);

        Vector3 r_t_b_corner = new Vector3(2.6f, 3f, 1.8f);
        Vector3 r_b_b_corner = new Vector3(2.6f, 0f, 1.8f);
        #endregion

        #region Edges
        Vector3 t_f_1 = new Vector3(1f, 3f, 0f);
        Vector3 t_f_2 = new Vector3(2f, 3f, 0f);
        Vector3 b_f_1 = new Vector3(1f, 0f, 0f);
        Vector3 b_f_2 = new Vector3(2f, 0f, 0f);

        Vector3 t_l_1 = new Vector3(0f, 3f, 1.33f);
        Vector3 t_l_2 = new Vector3(0f, 3f, 0.66f);
        Vector3 b_l_1 = new Vector3(0.5f, 0f, 1.33f);
        Vector3 b_l_2 = new Vector3(0.5f, 0f, 0.66f);

        Vector3 t_b_1 = new Vector3(2f, 3f, 2f);
        Vector3 t_b_2 = new Vector3(1f, 3f, 2f);
        Vector3 b_b_1 = new Vector3(2f, 0f, 2f);
        Vector3 b_b_2 = new Vector3(1f, 0f, 2f);

        Vector3 t_r_1 = new Vector3(3f, 3f, 0.66f);
        Vector3 t_r_2 = new Vector3(3f, 3f, 1.33f);
        Vector3 b_r_1 = new Vector3(3f, 0f, 0.66f);
        Vector3 b_r_2 = new Vector3(3f, 0f, 1.33f);

        Vector3 f_r_1 = new Vector3(2.5f, 2f, 0.2f);
        Vector3 f_r_2 = new Vector3(2.5f, 1f, 0.2f);
        Vector3 f_l_1 = new Vector3(0f, 2f, 0f);
        Vector3 f_l_2 = new Vector3(0f, 1f, 0f);

        Vector3 back_r_1 = new Vector3(2.5f, 2f, 1.8f);
        Vector3 back_r_2 = new Vector3(2.5f, 1f, 1.8f);
        Vector3 back_l_1 = new Vector3(0f, 2f, 2f);
        Vector3 back_l_2 = new Vector3(0f, 1f, 2f);
        #endregion

        Vector3[,] controlPoints = new Vector3[4, 4];
        #region Rightside
        controlPoints0 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_r_2,
            new Vector3(2.5f, 1f, 0.66f),
            new Vector3(2.5f, 1f, 1.33f),
            back_r_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_r_1,
            new Vector3(2.5f, 2f, 0.66f),
            new Vector3(2.5f, 2f, 1.33f),
            back_r_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPoints[0, i] = controlPoints0[i];
            controlPoints[1, i] = controlPoints1[i];
            controlPoints[2, i] = controlPoints2[i];
            controlPoints[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPoints);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPoints, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv1 = new Vector3[4, 4];
        #region Leftside
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            f_l_2,
            new Vector3(-0.5f, 1f, 0.66f),
            new Vector3(-0.5f, 1f, 1.33f),
            back_l_2
        };
        controlPoints2 = new Vector3[4]
        {
            f_l_1,
            new Vector3(-0.5f, 2f, 0.66f),
            new Vector3(-0.5f, 2f, 1.33f),
            back_l_1
        };
        controlPoints3 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv1[0, i] = controlPoints3[i];
            controlPointsv1[1, i] = controlPoints2[i];
            controlPointsv1[2, i] = controlPoints1[i];
            controlPointsv1[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv1);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv1, controlPointScale, red);
        }
        #endregion

        Vector3[,] controlPointsv2 = new Vector3[4, 4];
        #region Bottom
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            b_l_2,
            b_l_1,
            l_b_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(3f, -5f, 0.66f),
            new Vector3(3f, -5f, 1.33f),
            b_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(4f, -4f, 0.66f),
            new Vector3(4f, -4f, 1.33f),
            b_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            b_r_1,
            b_r_2,
            r_b_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv2[0, i] = controlPoints0[i];
            controlPointsv2[1, i] = controlPoints1[i];
            controlPointsv2[2, i] = controlPoints2[i];
            controlPointsv2[3, i] = controlPoints3[i];
        }
        allControlPoints.Add(controlPointsv2);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv2, controlPointScale, green);
        }
        #endregion


        Vector3[,] controlPointsv3 = new Vector3[4, 4];
        #region Top
        controlPoints0 = new Vector3[4]
        {
            l_t_f_corner,
            t_l_2,
            t_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            t_f_1,
            new Vector3(3f, 8f, 0.66f),
            new Vector3(3f, 8f, 1.33f),
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            t_f_2,
            new Vector3(4f, 7f, 0.66f),
            new Vector3(4f, 7f, 1.33f),
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_t_f_corner,
            t_r_1,
            t_r_2,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv3[0, i] = controlPoints3[i];
            controlPointsv3[1, i] = controlPoints2[i];
            controlPointsv3[2, i] = controlPoints1[i];
            controlPointsv3[3, i] = controlPoints0[i];
        }
        allControlPoints.Add(controlPointsv3);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv3, controlPointScale, green);
        }
        #endregion

        Vector3[,] controlPointsv4 = new Vector3[4, 4];
        #region Front
        controlPoints0 = new Vector3[4]
        {
            l_b_f_corner,
            f_l_2,
            f_l_1,
            l_t_f_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_f_1,
            new Vector3(1f, 1f, 0f),
            new Vector3(1f, 2f, 0f),
            t_f_1
        };
        controlPoints2 = new Vector3[4]
        {
            b_f_2,
            new Vector3(2f, 1f, 0f),
            new Vector3(2f, 2f, 0f),
            t_f_2
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_f_corner,
            f_r_2,
            f_r_1,
            r_t_f_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv4[0, i] = controlPoints3[i];
            controlPointsv4[1, i] = controlPoints2[i];
            controlPointsv4[2, i] = controlPoints1[i];
            controlPointsv4[3, i] = controlPoints0[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv4);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv4, controlPointScale, blue);
        }

        Vector3[,] controlPointsv5 = new Vector3[4, 4];
        #region Back
        controlPoints0 = new Vector3[4]
        {
            l_b_b_corner,
            back_l_2,
            back_l_1,
            l_t_b_corner
        };
        controlPoints1 = new Vector3[4]
        {
            b_b_2,
            new Vector3(1f, 1f, 2f), // Z=2f
            new Vector3(1f, 2f, 2f), // Z=2f
            t_b_2
        };
        controlPoints2 = new Vector3[4]
        {
            b_b_1,
            new Vector3(2f, 1f, 2f), // Z=2f
            new Vector3(2f, 2f, 2f), // Z=2f
            t_b_1
        };
        controlPoints3 = new Vector3[4]
        {
            r_b_b_corner,
            back_r_2,
            back_r_1,
            r_t_b_corner
        };

        for (int i = 0; i < 4; i++)
        {
            controlPointsv5[0, i] = controlPoints0[i];
            controlPointsv5[1, i] = controlPoints1[i];
            controlPointsv5[2, i] = controlPoints2[i];
            controlPointsv5[3, i] = controlPoints3[i];
        }
        #endregion
        allControlPoints.Add(controlPointsv5);
        if (showControlPoints)
        {
            PlotMatrixPoints(controlPointsv5, controlPointScale, blue);
        }

        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = GenerateMultipleSurfaces(allControlPoints, stepSize);

        if (showSampledPoints)
        {
            PlotPoints(vertices, new Vector3(0.1f, 0.1f, 0.1f), blue);
        }

        RenderSurface(vertices, triangles, defaultMaterial, uvs);
    }

    #endregion // ---------------------------------------------------------------------------------
}
