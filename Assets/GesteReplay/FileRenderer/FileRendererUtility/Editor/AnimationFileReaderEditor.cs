using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationFileReader))]
public class AnimationFileReaderEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        
		AnimationFileReader myScript = (AnimationFileReader)target;
		if(GUILayout.Button("Pause"))
		{
			myScript.Pause();
		}
		if(GUILayout.Button("Play"))
		{
			myScript.Play();
		}
		if(GUILayout.Button("Reset"))
		{
			myScript.ResetAnim();
		}
		if(GUILayout.Button("LoadNextGeste"))
		{
			myScript.LoadNewGeste(myScript.newGeste);
		}
	}
}