using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    public string SceneName;
    public int levelNumber;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetInt("level", levelNumber);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneName);
        }
    }
}
