using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class IcoSphere : MonoBehaviour {
	public struct Triangle {
		public int v1;
		public int v2;
		public int v3;

		public Triangle(int v1, int v2, int v3) {
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
		}
	}

	private int GetMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius) {
		bool firstIsSmaller = p1 < p2;
		long smallerIndex = firstIsSmaller ? p1 : p2;
		long greaterIndex = firstIsSmaller ? p2 : p1;
		long key = (smallerIndex << 32) + greaterIndex;

		int ret;
		if (cache.TryGetValue(key, out ret)) {
			return ret;
		}
		
		Vector3 point1 = vertices[p1];
		Vector3 point2 = vertices[p2];
		Vector3 middle = new Vector3 (
			(point1.x + point2.x) / 2f,
			(point1.y + point2.y) / 2f,
			(point1.z + point2.z) / 2f
		);
		
		int i = vertices.Count;
		vertices.Add(middle.normalized * radius);
		
		cache.Add(key, i);

		return i;
	}

	public void Create() {
		MeshFilter filter = GetComponent<MeshFilter>();
		Mesh mesh = filter.mesh = new Mesh();

		Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

		int recursionLevel = 0;
		float radius = 1f;

		// create 12 vertices of a icosahedron
		float t = (1f + Mathf.Sqrt(5f)) / 2f;

		List<Vector3> vertList = new List<Vector3> {
			new Vector3(-1f, t, 0f).normalized * radius,
			new Vector3(1f, t, 0f).normalized * radius,
			new Vector3(-1f, -t, 0f).normalized * radius,
			new Vector3(1f, -t, 0f).normalized * radius,

			new Vector3(0f, -1f, t).normalized * radius,
			new Vector3(0f, 1f, t).normalized * radius,
			new Vector3(0f, -1f, -t).normalized * radius,
			new Vector3(0f, 1f, -t).normalized * radius,

			new Vector3(t, 0f, -1f).normalized * radius,
			new Vector3(t, 0f, 1f).normalized * radius,
			new Vector3(-t, 0f, -1f).normalized * radius,
			new Vector3(-t, 0f, 1f).normalized * radius
		};

		List<Triangle> faces = new List<Triangle> {
			new Triangle(0, 11, 5),
			new Triangle(0, 5, 1),
			new Triangle(0, 1, 7),
			new Triangle(0, 7, 10),
			new Triangle(0, 10, 11),

			new Triangle(1, 5, 9),
			new Triangle(5, 11, 4),
			new Triangle(11, 10, 2),
			new Triangle(10, 7, 6),
			new Triangle(7, 1, 8),

			new Triangle(3, 9, 4),
			new Triangle(3, 4, 2),
			new Triangle(3, 2, 6),
			new Triangle(3, 6, 8),
			new Triangle(3, 8, 9),

			new Triangle(4, 9, 5),
			new Triangle(2, 4, 11),
			new Triangle(6, 2, 10),
			new Triangle(8, 6, 7),
			new Triangle(9, 8, 1)
		};


		for (int i = 0; i < recursionLevel; i++) {
			List<Triangle> faces2 = new List<Triangle>();
			foreach (var tri in faces) {
				int a = GetMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
				int b = GetMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
				int c = GetMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

				faces2.Add(new Triangle(tri.v1, a, c));
				faces2.Add(new Triangle(tri.v2, b, a));
				faces2.Add(new Triangle(tri.v3, c, b));
				faces2.Add(new Triangle(a, b, c));
			}
			faces = faces2;
		}

		mesh.vertices = vertList.ToArray();
		List<int> triList = new List<int>();
		for (int i = 0; i < faces.Count; i++) {
			triList.Add(faces[i].v1);
			triList.Add(faces[i].v2);
			triList.Add(faces[i].v3);
		}
		mesh.triangles = triList.ToArray();

		Vector3[] normales = new Vector3[vertList.Count];
		for (int i = 0; i < normales.Length; i++)
			normales[i] = vertList[i].normalized;

		mesh.normals = normales;
		mesh.RecalculateBounds();
	}

	private void Start() {
		Create();
	}
}
