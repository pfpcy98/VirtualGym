using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{

    private string sceneName = "TestScene";

    private void FixedUpdate()
    {
        if(Input.GetMouseButtonDown(1) && (SceneManager.GetActiveScene().name != "TitleScene"))
        {
            OnLoadTitleScene();
        }
    }

    public void SetSceneName(string name)
    {
        sceneName = name;
    }

    public void OnLoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnLoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}

