using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;

public class SceneMenu 
{
    [MenuItem("Scenes/Lobby")]
    static void OpenLobby()
    {
        OpenScene(SceneInfo.Names.Lobby);
    }

    [MenuItem("Scenes/CowboySpaceship")]
    static void OpenCowboySpaceship()
    {
        OpenScene(SceneInfo.Names.CowboySpaceship);
    }

    [MenuItem("Scenes/GridRoom")]
    static void OpenGridRoom()
    {
        OpenScene(SceneInfo.Names.GridRoom);
    }

    static void OpenScene(string name)
    {
        Scene persistentScene = EditorSceneManager.OpenScene("Assets/Scenes/" + SceneInfo.Names.XRPersistent + ".unity", OpenSceneMode.Single);
        Scene currentScene = EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity", OpenSceneMode.Additive);
        SceneInfo.AlignXRRig(persistentScene, currentScene);
    }

    
}
