using System;
using TXDCL.Map;
using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    [SerializeField] private SceneData_SO SceneData;
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnAfterSceneLoadEvent()
    {
        InitCharacter();
    }
    /// <summary>
    /// 根据场景数据生成对应角色信息
    /// </summary>
    private void InitCharacter()
    {
        foreach (var character in SceneData.Characters)
        {
            Instantiate(character.character, character.transform, Quaternion.identity);
        }
    }
}
