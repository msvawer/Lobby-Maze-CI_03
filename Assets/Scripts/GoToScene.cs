using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToScene : MonoBehaviour
{
    public SceneInfo.sceneID nextScene = SceneInfo.sceneID.Lobby;

    public void Go()
    {
        SceneLoader.Instance.LoadScene(SceneInfo.scenes[(int)nextScene]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
