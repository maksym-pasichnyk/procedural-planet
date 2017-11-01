using System.Collections.Generic;
using UnityEngine;

public class MeshCube : MonoBehaviour {
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uvs;
	private int index;

	public MeshFilter meshFilter;

	void Start() {
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
		index = 0;

		Block(new Vector3(0, 0, 0));
		Block(new Vector3(1, 0, 0));
		Block(new Vector3(0, 0, 1));
		Block(new Vector3(1, 0, 1));

		meshFilter.mesh = new Mesh {
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray(),
			uv = uvs.ToArray()
		};
	}

	public void BlockFace(Vector3 origin, Vector3 width, Vector3 length) {
		var normal = Vector3.Cross(length, width).normalized;

		vertices.AddRange(new Vector3[] { origin, origin + length, origin + length + width, origin + width });
		uvs.AddRange(new Vector2[] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right });
		triangles.AddRange(new int[] { index + 0, index + 1, index + 2, index + 0, index + 2, index + 3 });
		index += 4;
	}

	public void Block(Vector3 center) {
		var offset = Vector3.one / 2;
		var width = Vector3.right;
		var length = Vector3.forward;
		var height = Vector3.up;

		BlockFace(center - offset, length, width);
		BlockFace(center - offset, width, height);
		BlockFace(center - offset, height, length);
		BlockFace(center + offset, -width, -length);
		BlockFace(center + offset, -height, -width);
		BlockFace(center + offset, -length, -height);
	}
}
