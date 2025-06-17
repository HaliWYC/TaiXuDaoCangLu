using System;
using UnityEngine;

namespace TXDCL.Transition
{
    public class Teleport : MonoBehaviour
    {
        public MapData_SO sceneToGo;
        public Vector3 positionToGo;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                EventHandler.CallSceneLoadedEvent(sceneToGo, positionToGo);
            }
        }
    }
}