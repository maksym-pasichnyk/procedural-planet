using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
	public int seed;

	public int chunkSize;
	public int octaves;
	public float scale;
	public float persistence;
	public float lacunarity;
	public float heightMultiplayer;
	public Vector3 offset;

	public MeshFilter meshFilter;

	public float PerlinNoise3D(float x, float y, float z) {
		float xy = Mathf.PerlinNoise(x, y);
		float yx = Mathf.PerlinNoise(y, x);

		float yz = Mathf.PerlinNoise(y, z);
		float zy = Mathf.PerlinNoise(z, y);

		float zx = Mathf.PerlinNoise(z, x);
		float xz = Mathf.PerlinNoise(x, z);

		return (xy + yz + yz + zy + zx + xz) / 3f - 1f;
	}

	public float GetHeight3D(float x, float y, float z) {
		float amplitude = 1f;
		float frequency = 1f;

		float noise = 0f;

		for (int i = 0; i < octaves; i++) {
			noise += PerlinNoise3D((x + offset.x) / scale * frequency, (y + offset.y) / scale * frequency, (z + offset.z) / scale * frequency) * amplitude;

			amplitude *= persistence;
			frequency *= lacunarity;
		}
		return noise;
	}

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

	private void Start() {

	}

	public System.Collections.IEnumerator Generate() {
		yield return meshFilter.mesh = Icosahedron(10);
	}

	public Mesh TriangleMesh(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2) {
		var normal = Vector3.Cross((vertex1 - vertex0), (vertex2 - vertex0)).normalized;
		var mesh = new Mesh {
			vertices = new[] { vertex0, vertex1, vertex2 },
			normals = new[] { normal, normal, normal },
			uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) },
			triangles = new[] { 0, 1, 2 }
		};
		return mesh;
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
		Vector3 middle = new Vector3(
			(point1.x + point2.x) / 2f,
			(point1.y + point2.y) / 2f,
			(point1.z + point2.z) / 2f
		);

		int i = vertices.Count;
		vertices.Add(middle.normalized * radius);

		cache.Add(key, i);

		return i;
	}

	public Mesh Icosahedron(float radius) {
		/*var magicAngle = Mathf.PI * 26.565f / 180;
		var segmentAngle = Mathf.PI * 72 / 180;
		var currentAngle = 0f;
		
		for (var i = 1; i < 6; i++) {
			v[i] = new Vector3(radius * Mathf.Sin(currentAngle) * Mathf.Cos(magicAngle), radius * Mathf.Sin(magicAngle), radius * Mathf.Cos(currentAngle) * Mathf.Cos(magicAngle));
			currentAngle += segmentAngle;
		}
		
		currentAngle = Mathf.PI * 36 / 180;
		for (var i = 6; i < 11; i++) {
			v[i] = new Vector3(radius * Mathf.Sin(currentAngle) * Mathf.Cos(-magicAngle), radius * Mathf.Sin(-magicAngle), radius * Mathf.Cos(currentAngle) * Mathf.Cos(-magicAngle));
			currentAngle += segmentAngle;
		}*/

		float t = (1f + Mathf.Sqrt(5f)) / 2f;
		var r = Quaternion.Euler(58, 0, 0);
		/*var v = new Vector3[12];
		v[0] = r * new Vector3(0, 1, 0);
		v[1] = r * new Vector3(0, 0.4472128f, 0.8944276f);
		v[2] = r * new Vector3(0.8506512f, 0.4472128f, 0.2763933f);
		v[3] = r * new Vector3(0.5257313f, 0.4472128f, -0.7236072f);
		v[4] = r * new Vector3(-0.5257314f, 0.4472128f, -0.7236071f);
		v[5] = r * new Vector3(-0.8506511f, 0.4472128f, 0.2763934f);
		v[6] = r * new Vector3(0.5257313f, -0.4472128f, 0.7236071f);
		v[7] = r * new Vector3(0.8506511f, -0.4472128f, -0.2763934f);
		v[8] = r * new Vector3(-7.819335E-08f, -0.4472128f, -0.8944276f);
		v[9] = r * new Vector3(-0.8506511f, -0.4472128f, -0.2763934f);
		v[10] = r * new Vector3(-0.5257314f, -0.4472128f, 0.7236071f);
		v[11] = r * new Vector3(0, -1, 0);*/

		/*var vertices = new List<Vector3> {
			new Vector3(0f, 0.5299193f, 0.8480481f),
			new Vector3(0f, -0.5215309f, 0.8532324f),
			new Vector3(0.8506512f, 0.002591879f, 0.5257241f),

			new Vector3(0.5257313f, 0.8506404f, -0.00419546f),
			new Vector3(-0.5257314f, 0.8506403f, -0.004195428f),
			new Vector3(-0.8506511f, 0.002591778f, 0.5257242f),

			new Vector3(0.5257313f, -0.8506403f, 0.004195428f),
			new Vector3(0.8506511f, -0.002591778f, -0.5257242f),
			new Vector3(-7.819335E-08f, 0.5215309f, -0.8532324f),

			new Vector3(-0.8506511f, -0.002591778f, -0.5257242f),
			new Vector3(-0.5257314f, -0.8506403f, 0.004195428f),
			new Vector3(0f, -0.5299193f, -0.8480481f)
		};*/



		/*List<Vector3> vertices = new List<Vector3> {
			new Vector3(-1f, t, 0f).normalized,
			new Vector3(1f, t, 0f).normalized,
			new Vector3(-1f, -t, 0f).normalized,
			new Vector3(1f, -t, 0f).normalized,

			new Vector3(0f, -1f, t).normalized,
			new Vector3(0f, 1f, t).normalized,
			new Vector3(0f, -1f, -t).normalized,
			new Vector3(0f, 1f, -t).normalized,

			new Vector3(t, 0f, -1f).normalized,
			new Vector3(t, 0f, 1f).normalized,
			new Vector3(-t, 0f, -1f).normalized,
			new Vector3(-t, 0f, 1f).normalized
		};*/

		var vertices = new List<Vector3> {
			new Vector3(0f, 1f, t).normalized,
			new Vector3(0f, -1f, t).normalized,
			new Vector3(t, 0f, 1f).normalized,

			new Vector3(1f, t, 0f).normalized,
			new Vector3(-1f, t, 0f).normalized,
			new Vector3(-t, 0f, 1f).normalized,

			new Vector3(1f, -t, 0f).normalized,
			new Vector3(t, 0f, -1f).normalized,
			new Vector3(0f, 1f, -t).normalized,

			new Vector3(-t, 0f, -1f).normalized,
			new Vector3(-1f, -t, 0f).normalized,
			new Vector3(0f, -1f, -t).normalized
		};

		var faces = new List<Triangle>() {
			new Triangle(0, 1, 2),
			new Triangle(0, 2, 3),
			new Triangle(0, 3, 4),
			new Triangle(0, 4, 5),
			new Triangle(0, 5, 1),

			new Triangle(11, 7, 6),
			new Triangle(11, 8, 7),
			new Triangle(11, 9, 8),
			new Triangle(11, 10, 9),
			new Triangle(11, 6, 10),

			new Triangle(2, 1, 6),
			new Triangle(3, 2, 7),
			new Triangle(4, 3, 8),
			new Triangle(5, 4, 9),
			new Triangle(1, 5, 10),

			new Triangle(6, 7, 2),
			new Triangle(7, 8, 3),
			new Triangle(8, 9, 4),
			new Triangle(9, 10, 5),
			new Triangle(10, 6, 1)
		};

		Dictionary<Vector3, int> cache = new Dictionary<Vector3, int>();

		for (int i = 0; i < 5; i++) {
			var faces2 = new List<Triangle>();
			foreach (var face in faces) {
				var point1 = vertices[face.v1];
				var point2 = vertices[face.v2];
				var point3 = vertices[face.v3];

				var middle1 = (point1 + point2) / 2f;
				var middle2 = (point2 + point3) / 2f;
				var middle3 = (point3 + point1) / 2f;

				int a;
				if (cache.TryGetValue(middle1, out a) == false) {
					a = vertices.Count;
					cache.Add(middle1, a);
					vertices.Add(middle1);
				}

				int b;
				if (cache.TryGetValue(middle2, out b) == false) {
					b = vertices.Count;
					cache.Add(middle2, b);
					vertices.Add(middle2);
				}

				int c;
				if (cache.TryGetValue(middle3, out c) == false) {
					c = vertices.Count;
					cache.Add(middle3, c);
					vertices.Add(middle3);
				}

				faces2.Add(new Triangle(face.v1, a, c));
				faces2.Add(new Triangle(face.v2, b, a));
				faces2.Add(new Triangle(face.v3, c, b));
				faces2.Add(new Triangle(a, b, c));
			}

			faces = faces2;
		}

		var uv = new List<Vector2>();
		var normals = new List<Vector3>();
		var triangles = new List<int>();
		foreach (var face in faces) {
			triangles.Add(face.v1);
			triangles.Add(face.v2);
			triangles.Add(face.v3);
		}

		for (int i = 0; i < vertices.Count; i++) {
			var vertex = vertices[i];
			normals.Add(vertex.normalized);

			float noise = 1f + GetHeight3D(vertex.x, vertex.y, vertex.z) * heightMultiplayer;

			vertices[i] = vertices[i].normalized * noise;
		}
		var mesh = new Mesh {
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray(),
			normals = normals.ToArray()
		};
		mesh.RecalculateTangents();
		mesh.RecalculateBounds();
		return mesh;
	}
}