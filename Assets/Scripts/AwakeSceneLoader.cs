using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeSceneLoader : MonoBehaviour
{
    void Awake()
    {
        SceneLoader.Instance.Awake();
    }
}
