using System;
using TXDCL.Combat;
using TXDCL.Map;
using UnityEngine;

namespace TXDCL.Transition
{
    public class Teleport : MonoBehaviour
    {
        public SceneData_SO sceneToGo;
        public Vector3 positionToGo;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !CombatManager.Instance.isCombating && !other.isTrigger)
            {
                EventHandler.CallSceneLoadedEvent(sceneToGo, positionToGo);
            }
        }
    }
}