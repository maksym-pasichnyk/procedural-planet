using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI ();

		Planet planet = (Planet) target;
		if (!Application.isPlaying) {
			if (GUILayout.Button ("Update")) {
				planet.StartCoroutine(planet.Generate());
			}
		}
	}
}
