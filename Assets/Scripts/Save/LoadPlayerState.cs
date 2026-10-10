using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayerState : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        if (GameSaveManager.Instance == null)
            yield break;

        if (!GameSaveManager.Instance.HasSaveGame())
            yield break;

        string savedScene =
            GameSaveManager.Instance.GetSavedScene();

        string currentScene =
            SceneManager.GetActiveScene().name;

        if (savedScene != currentScene)
            yield break;

        CharacterController controller =
            GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        transform.position =
            GameSaveManager.Instance.GetSavedPosition();

        transform.rotation =
            Quaternion.Euler(
                GameSaveManager.Instance.GetSavedRotation()
            );

        if (controller != null)
            controller.enabled = true;

        Debug.Log(
            "Player restored to: " +
            transform.position
        );
    }
}
