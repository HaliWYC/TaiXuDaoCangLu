using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    private int sceneIndex = -1;
    GUIContent [] sceneNames;
    private AddressableAssetGroup sceneGroup;

    private readonly string[] scenePathSplit = { "/", ".unity" };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(sceneGroup.entries.Count == 0) return;
        if (sceneIndex==-1)
            GetSceneNameArray(property);
        var oldIndex = sceneIndex;
        sceneIndex =EditorGUI.Popup(position, label, sceneIndex, sceneNames);
        if(oldIndex != sceneIndex)
            property.stringValue = sceneNames[sceneIndex].text;
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        var Scene = sceneGroup.entries.Select(scene => scene.address).ToArray();
        //初始化
        sceneNames = new GUIContent[Scene.Length];
        for (var i = 0; i < Scene.Length; i++)
        {
            sceneNames[i] = new GUIContent(Scene[i]);
        }

        if (sceneNames.Length == 0)
        {
            sceneNames = new[] { new GUIContent("Check your build settings") };
        }
        if (!string.IsNullOrEmpty(property.stringValue))
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
