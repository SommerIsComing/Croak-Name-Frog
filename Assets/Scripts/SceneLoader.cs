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

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

}
