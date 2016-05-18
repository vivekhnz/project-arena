using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    public string TargetScene;

    public void Navigate()
    {
        SceneManager.LoadScene(TargetScene);
    }
}
