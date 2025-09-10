using UnityEngine;
using UnityEditor;

public class MyEditorScript {
    static string[] scenes = new[]{ "Assets/Scenes/SampleScene.unity" };
    static string buildPath = "Builds/MyGame.app";

    [MenuItem("CI/BuildOSX")]
    public static void PerformBuild() {
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneOSX, BuildOptions.None);
    }
}