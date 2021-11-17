using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ImageGenerator))]
public class ImageGeneratorEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        
		ImageGenerator myScript = (ImageGenerator)target;
		if(GUILayout.Button("Generate"))
		{
			myScript.Generate();
		}

	}
}