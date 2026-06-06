using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered sceneloader");
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        Debug.Log("Loading: "+ sceneToLoad);

        if(sceneToLoad == "Credits")
        {
            GameObject temp = new GameObject();
            DontDestroyOnLoad(temp);

            Scene ddolScene = temp.scene;

            foreach (GameObject obj in ddolScene.GetRootGameObjects())
            {
                if (obj != temp)
                {
                    Destroy(obj);
                }
            }

            Destroy(temp);
        }
    
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }


}
