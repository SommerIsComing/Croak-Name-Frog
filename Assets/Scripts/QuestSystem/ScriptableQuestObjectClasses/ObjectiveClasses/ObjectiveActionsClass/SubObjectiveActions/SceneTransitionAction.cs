using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/Objective Actions/SceneTransitionAction")]
public class SceneTransitionAction : ObjectiveAction
{
    public string sceneName;
    public override void ExecuteAction()
    {
        Debug.Log("Tried Loading Scene");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
