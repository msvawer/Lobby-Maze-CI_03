using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GoToScene : MonoBehaviour
{
    public SceneInfo.sceneID nextScene = SceneInfo.sceneID.Lobby;

    public void Go()
    {
        SceneLoader.Instance.LoadScene(SceneInfo.scenes[(int)nextScene]);
    }

    
}
