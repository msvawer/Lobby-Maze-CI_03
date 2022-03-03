using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneInfo 
{
    public enum sceneID { Lobby, cowboySpaceship, GridRoom}

    public static readonly string[] scenes = { Names.Lobby, Names.CowboySpaceship, Names.GridRoom };
    public static class Names
    {
        public static readonly string XRPersistent = "XRPersistent";
        public static readonly string CowboySpaceship = "CowboyUp";
        public static readonly string GridRoom = "GridRoom";
        public static readonly string Lobby = "Lobby";

    }

    public static void AlignXRRig(Scene persistentScene, Scene currentScene)
    {
        GameObject[] currentObjects = currentScene.GetRootGameObjects();
        GameObject[] persistentObjects = persistentScene.GetRootGameObjects();

        foreach (var origin in currentObjects)
        {
            if (origin.CompareTag("XRRigOrigin"))
            {
                foreach (var rig in persistentObjects)
                {
                    if (rig.CompareTag("XRRig"))
                    {
                        rig.transform.position = origin.transform.position;
                        rig.transform.rotation = origin.transform.rotation;
                        return;
                    }
                }
            }
        }
    }

}
