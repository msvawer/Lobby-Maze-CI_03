using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
public class SceneLoader : Singleton<SceneLoader>
{
    //screen fade material we plug in editor inspector 
    public Material screenFade = null;

    [Min(0.001f)]
    public float fadeSpeed = 1.0f;

    [Range(0.0f, 5.0f)]
    public float addedWaitTime = 2.0f;

    public UnityEvent onLoadStart = new UnityEvent();
    public UnityEvent onBeforeUnload = new UnityEvent();
    public UnityEvent onLoadFinish = new UnityEvent();

    bool m_isLoading = false;
    //bool to check before starting loading or unloading that a load or unload is not already in progress

     //controls if we are 0 or 1 with fade amount 
    float m_fadeAmount = 0.0f;
    //courotutine to keep track if we are fading or not
    Coroutine m_fadeCoroutine = null;
    //access shader property ID, where our fade amount float will go for our fade material 
    static readonly int m_fadeAmountPropID = Shader.PropertyToID("_FadeAmount");

    Scene m_persistentScene;

    override protected void  Awake()
    {
        SceneManager.sceneLoaded += SetActiveScene;
        m_persistentScene = SceneManager.GetActiveScene();

        if(!Application.isEditor)
        {
            SceneManager.LoadSceneAsync(SceneInfo.Names.Lobby, LoadSceneMode.Additive);
        }

    }

    private void Destroy()
    {
        SceneManager.sceneLoaded -= SetActiveScene;
    }


    public void LoadScene(string name)
    {
        if(!m_isLoading)
        {
            StartCoroutine(Load(name));
        }
    }

    //unlike in Editor we want the loaded scene to become the active scene
    void SetActiveScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
        SceneInfo.AlignXRRig(m_persistentScene, scene);
    }

    IEnumerator Load(string name)
    {
        //alert our bool that a load has started so as not to start others 
        m_isLoading = true;
        //invoke the onLoadStart Event, set to optional in case nothing is subscribed
        onLoadStart?.Invoke();
        //call screen fade
        yield return FadeOut();

        onBeforeUnload?.Invoke();
        yield return new WaitForSeconds(0);

        //unload current Scene
        yield return StartCoroutine(UnLoadCurrentScene());

        yield return new WaitForSeconds(addedWaitTime);

        //Load the new scene by name
        yield return StartCoroutine(LoadNewScene(name));
        //fade back into seeing newly loaded scene
        yield return FadeIn();
        
        //if anyt callbacks subscribed waiting for this to finish, we'll let them know
        onLoadFinish?.Invoke();
        m_isLoading = false;
    }

    IEnumerator UnLoadCurrentScene()
    {
        //get the active scene from Scene manager and unload it asynchronosly via scene manager
        AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //while it is unloading in progress we keep going until is done then we can get out of couroutine...
        while (!unload.isDone)
        {
            yield return null;
        }
            
    }

    IEnumerator LoadNewScene(string name)
    {
        //use scene manager LoadSceneAsync funtion and load by name in additive mode
        AsyncOperation load = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!load.isDone)
        {
            yield return null;
        }
       
    }

    IEnumerator FadeOut()
    {
        if(m_fadeCoroutine != null)
        {
            StopCoroutine(m_fadeCoroutine);
        }
        //start couroutine Fade and target is 1
        m_fadeCoroutine = StartCoroutine(Fade(1.0f));
        yield return m_fadeCoroutine;
    }

    IEnumerator FadeIn()
    {
        if (m_fadeCoroutine != null)
        {
            StopCoroutine(m_fadeCoroutine);
        }
        //starting Fade Couroutine and the target is 0
        m_fadeCoroutine = StartCoroutine(Fade(0.0f));
        yield return m_fadeCoroutine;
    }

    IEnumerator Fade(float target)
    {
        //while fade amount is not yet at target 
        while(Mathf.Approximately(m_fadeAmount, target))
        {
            //setting the fade amount float that will go to shader to go towards the target at the fadespeed float smoothed out by time.deltaTime
            m_fadeAmount = Mathf.MoveTowards(m_fadeAmount, target, fadeSpeed * Time.deltaTime);
            screenFade.SetFloat(m_fadeAmountPropID, m_fadeAmount);
            yield return null;
        }
        //just to ensure it gets set to target
        screenFade.SetFloat(m_fadeAmountPropID, target);

    }

}
