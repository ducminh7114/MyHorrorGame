using UnityEngine;

public class ColliderFinder : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject obj = hit.collider.gameObject;

        Debug.Log(
            "PLAYER BLOCKED BY: " + obj.name +
            "\nPath: " + GetPath(obj.transform) +
            "\nCollider: " + hit.collider.GetType().Name,
            obj
        );
    }

    string GetPath(Transform obj)
    {
        string path = obj.name;

        while (obj.parent != null)
        {
            obj = obj.parent;
            path = obj.name + "/" + path;
        }

        return path;
    }
}