using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Singleton<T>
{
   private static T instance;

   public static T Instance => instance;

   protected virtual void Awake()
   {
      if(instance!=null)
         Destroy(gameObject);
      else
         instance = (T)this;
      
   }
   
   public static bool IsInitialized => instance != null;

   protected virtual void OnDestroy()
   {
      if(instance == this)
         instance = null;
   }
}
