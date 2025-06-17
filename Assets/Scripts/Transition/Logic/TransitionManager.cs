using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace TXDCL.Transition
{
   public class TransitionManager : Singleton<TransitionManager>
   {
      public AssetReference startScene;
      public Vector3 startPosition;
      private AssetReference currentScene;

      private async void Start()
      {
         await LoadStartSceneTask();
      }

      private void OnEnable()
      {
         EventHandler.SceneLoadedEvent += OnSceneLoadedEvent;
      }
   
      private void OnDisable()
      {
         EventHandler.SceneLoadedEvent -= OnSceneLoadedEvent;
      }

      private async void OnSceneLoadedEvent(MapData_SO data, Vector3 position)
      {
         currentScene = data.SceneToLoad;
         await UnloadSceneTask();
         EventHandler.CallMoveToPositionEvent(position);
         await LoadSceneTask();
      }
      
      /// <summary>
      /// 加载目标场景，并且设置为激活
      /// </summary>
      private async Awaitable LoadSceneTask()
      {
         var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
         await s.Task;
         if (s.Status == AsyncOperationStatus.Succeeded)
         {
            FadePanel.Instance.FadeOut(0.4f);
            SceneManager.SetActiveScene(s.Result.Scene);
            EventHandler.CallAfterSceneLoadEvent();
         }
      }
      
      private async Awaitable UnloadSceneTask()
      {
         EventHandler.CallBeforeSceneLoadEvent();
         FadePanel.Instance.FadeIn(1f);
         await Awaitable.WaitForSecondsAsync(1.5f);
         await Awaitable.FromAsyncOperation(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene())!);
      }
   
      private async Awaitable LoadStartSceneTask()
      {
         if (currentScene != null)
            await UnloadSceneTask();
         currentScene = startScene;
         EventHandler.CallMoveToPositionEvent(startPosition);
         await LoadSceneTask();
      }
   }

}
