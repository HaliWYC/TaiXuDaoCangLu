using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    private int sceneIndex = -1;
    GUIContent [] sceneNames;

    private readonly string[] scenePathSplit = { "/", ".unity" };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(EditorBuildSettings.scenes.Length == 0) return;
        if (sceneIndex==-1)
            GetSceneNameArray(property);
        var oldIndex = sceneIndex;
        sceneIndex =EditorGUI.Popup(position, label, sceneIndex, sceneNames);
        if(oldIndex != sceneIndex)
            property.stringValue = sceneNames[sceneIndex].text;
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        var scenes = EditorBuildSettings.scenes;
        //初始化
        sceneNames = new GUIContent[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            var path = scenes[i].path;
            var splitPath = path.Split(scenePathSplit, StringSplitOptions.RemoveEmptyEntries);
            var sceneName = "";
            sceneName = splitPath.Length > 0 ? splitPath[^1] : "Deleted Scene";
            sceneNames[i] = new GUIContent(sceneName);
        }

        if (sceneNames.Length == 0)
        {
            sceneNames = new[] { new GUIContent("Check your build settings") };
        }

        if (string.IsNullOrEmpty(property.stringValue))
        {
            var nameFound = false;
            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text == property.stringValue)
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }
            }

            if (!nameFound)
            {
                sceneIndex = 0;
            }
        }
        else
        {
            sceneIndex = 0;
        }
        property.stringValue = sceneNames[sceneIndex].text;
    }
}
