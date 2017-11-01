using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Triangle {
	public int vertex0, vertex1, vertex2;
}

public class MeshVisualizer : MonoBehaviour {
	public const float phi = 1.61803399f;

	public List<Vector3> vertices = new List<Vector3>();
	public List<Triangle> triangles = new List<Triangle>();

	public Color vertexColor, triangleColor;
	[Range(0, 1)] public float vertexSize;

	private void OnDrawGizmos() {
		var center = transform.position;

		Gizmos.color = triangleColor;
		foreach (var triangle in triangles) {
			Gizmos.DrawLine(center + vertices[triangle.vertex0], center + vertices[triangle.vertex1]);
			Gizmos.DrawLine(center + vertices[triangle.vertex1], center + vertices[triangle.vertex2]);
			Gizmos.DrawLine(center + vertices[triangle.vertex2], center + vertices[triangle.vertex0]);
		}

		Gizmos.color = vertexColor;
		foreach (var vertex in vertices) {
			Gizmos.DrawSphere(center + vertex, vertexSize);
		}
	}
}
