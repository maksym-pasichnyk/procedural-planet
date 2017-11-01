using System.Collections;
using UnityEditor;
using UnityEngine;

public class World : MonoBehaviour {
	public int seed;

	public int chunkSize;
	public int octaves;
	public float scale;
	public float persistence;
	public float lacunarity;
	public float heightMultiplayer;
	public Vector2 offset;

	public MeshFilter meshFilter;

	public int GetHeight(float x, float z) {
		float amplitude = 1f;
		float frequency = 1f;

		float y = 0f;

		for (int i = 0; i < octaves; i++) {
			y += Mathf.PerlinNoise(x / scale * frequency, z / scale * frequency) * amplitude;

			amplitude *= persistence;
			frequency *= lacunarity;
		}
		return (int)(y * 128);
	}

	public IEnumerator GenerateVoxel() {
		var meshBuilder = new MeshBuilder();
		int[,] map = new int[chunkSize, chunkSize];

		for (int z = 0, index = 0; z < chunkSize; z++) {
			for (int x = 0; x < chunkSize; x++, index += 4) {
				int y = GetHeight(x, z);
				Debug.LogFormat("[{0}, {1}, {2}]", x, y, z);

				meshBuilder.AddVertex(-0.5f + x, y, -0.5f + z);
				meshBuilder.AddVertex(0.5f + x, y, -0.5f + z);
				meshBuilder.AddVertex(-0.5f + x, y, 0.5f + z);
				meshBuilder.AddVertex(0.5f + x, y, 0.5f + z);

				meshBuilder.AddUV(0, 0);
				meshBuilder.AddUV(1, 0);
				meshBuilder.AddUV(0, 1);
				meshBuilder.AddUV(1, 1);

				meshBuilder.AddTriangle(index + 2, index + 1, index + 0);
				meshBuilder.AddTriangle(index + 3, index + 1, index + 2);

			}
		}

		meshFilter.mesh = meshBuilder.Create();
		yield return null;
	}

	public void Generate() {
		int meshSize = chunkSize + 1;

		int halfSize = meshSize / 2;
		float xOffset = offset.x;
		float zOffset = offset.y;

		var meshBuilder = new MeshBuilder();
		for (int z = 0, i = 0; z < meshSize; z++) {
			for (int x = 0; x < meshSize; x++, i++) {
				int octaves = this.octaves;

				float amplitude = 1f;
				float frequency = 1f;

				float y = 0f;

				while (octaves-- > 0) {
					float sx = (x + xOffset) / scale * frequency;
					float sz = (z + zOffset) / scale * frequency;

					float noise = Mathf.PerlinNoise (sx, sz) * 2f - 1f;
					y += noise * amplitude;

					amplitude *= persistence;
					frequency *= lacunarity;
				}

				meshBuilder.AddVertex (x - halfSize, y * heightMultiplayer, z - halfSize);
				meshBuilder.AddUV (x / (float) meshSize, z / (float) meshSize);
				if (z < chunkSize && x < chunkSize) {
					meshBuilder.AddTriangle (i + meshSize, i + 1, i);
					meshBuilder.AddTriangle (i + meshSize + 1, i + 1, i + meshSize);
				}
			}
		}

		meshFilter.mesh = meshBuilder.Create ();
	}

	public void Save() {
		AssetDatabase.CreateAsset(GetComponent<MeshFilter>().mesh, "Assets/world.asset");
		AssetDatabase.SaveAssets();
	}
}

[System.Serializable]
public class WorldSetting {
	
}