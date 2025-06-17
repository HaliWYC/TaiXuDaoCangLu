using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public AssetReference Persistant;
    public AssetReference UI;
    private void Awake()
    {
        Addressables.LoadSceneAsync(Persistant);
        Addressables.LoadSceneAsync(UI, LoadSceneMode.Additive);
    }
}