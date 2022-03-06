using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckWin : MonoBehaviour
{

    public SceneInfo.sceneID nextScene = SceneInfo.sceneID.Lobby;
    public UnityEvent onWin = new UnityEvent();
    // Start is called before the first frame update
    

    // Update is called once per frame
     private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("MainCamera"))
            {
            Debug.Log("you win or MainCamera tag entered");
            GoToLobby();
            onWin?.Invoke();
        }
    }

    public void GoToLobby()
    {
        SceneLoader.Instance.LoadScene(SceneInfo.scenes[(int)nextScene]);
    }
}
