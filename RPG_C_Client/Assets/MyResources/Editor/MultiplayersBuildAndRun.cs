using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiplayersBuildAndRun
{
	[MenuItem("Tools/Build/Android/Standard")]
	static void PerformAndroidBuild1()
	{
		PerformAndroidBuild();
	}

    [MenuItem("Tools/Build/Window/Standard")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }

    [MenuItem("Tools/Build/Window/2 Players")]
	static void PerformWin64Build2()
	{
		PerformWin64Build(2);
	}

	[MenuItem("Tools/Build/Window/3 Players")]
	static void PerformWin64Build3()
	{
		PerformWin64Build(3);
	}

	static void PerformWin64Build(int playerCount)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(
			BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

		for (int i = 1; i <= playerCount; i++)
		{
			BuildPipeline.BuildPlayer(GetScenePaths(),
				"Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
				BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
		}
	}

	static void PerformAndroidBuild()
	{
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Android, BuildTarget.Android);

		BuildPipeline.BuildPlayer(GetScenePaths(),
			$"Builds/Android/{GetProjectName()}_{DateTime.Now.ToString("yyyyMMddHHmm")}.apk",
				BuildTarget.Android, BuildOptions.None);
    }

	static string GetProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		return s[s.Length - 2];
	}

	static string[] GetScenePaths()
	{
		string[] scenes = new string[EditorBuildSettings.scenes.Length];

		for (int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}

		return scenes;
	}
}
