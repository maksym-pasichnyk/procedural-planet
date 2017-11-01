using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI ();

		World world = (World)target;
		if (!Application.isPlaying) {
			if (GUILayout.Button ("Update")) {
				world.StartCoroutine(world.GenerateVoxel());
			}

			if (GUILayout.Button("Save to assets")) {
				world.Save();
			}
		}
	}
}
