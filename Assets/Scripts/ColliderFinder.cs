using UnityEngine;

public class ColliderFinder : MonoBehaviour
{
        [Header("Collision Settings")]
        public bool showDebugLog = true;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("[ColliderFinder] Item collided with: " + other.gameObject.name);

            if (showDebugLog)
            {
                Debug.Log("Tag: " + other.tag);
                Debug.Log("Layer: " + LayerMask.LayerToName(other.gameObject.layer));
                Debug.Log("Collider: " + other.GetType().Name);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (showDebugLog)
            {
                Debug.Log("Currently touching: " + other.gameObject.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Item stopped colliding with: " + other.gameObject.name);
        }
 }
