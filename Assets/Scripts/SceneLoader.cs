using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
public class SceneLoader : Singleton<SceneLoader>
{
    public Material screenFade = null;

    [Min(0.001f)]
    public float fadeSpeed = 1.0f;

    [Range(0.0f, 5.0f)]
    public float addedWaitTime = 2.0f;

    UnityEvent onLoadStart = new UnityEvent();
    UnityEvent onLoadFinish = new UnityEvent();

    bool m_isLoading = false;
    float m_fadeAmount = 0.0f;
    Coroutine m_fadeCoroutine = null;
    static readonly int m_fadeAmountPropID = Shader.PropertyToID("_FadeAmount");

    Scene m_persistentScene;

    private void Awake()
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

    void SetActiveScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
        SceneInfo.AlignXRRig(m_persistentScene, scene);
    }

    IEnumerator Load(string name)
    {
        m_isLoading = true;
        onLoadStart?.Invoke();
        yield return FadeOut();
        yield return StartCoroutine(UnLoadCurrentScene());

        yield return new WaitForSeconds(addedWaitTime);

        yield return StartCoroutine(LoadNewScene(name));
        yield return FadeIn();
        m_isLoading = false;
        onLoadFinish?.Invoke();
    }

    IEnumerator UnLoadCurrentScene()
    {
        AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        while (!unload.isDone)
            yield return null;
    }

    IEnumerator LoadNewScene(string name)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!load.isDone)
        yield return null;
    }

    IEnumerator FadeOut()
    {
        if(m_fadeCoroutine != null)
        {
            StopCoroutine(m_fadeCoroutine);
        }
        m_fadeCoroutine = StartCoroutine(Fade(1.0f));
        yield return m_fadeCoroutine;
    }

    IEnumerator FadeIn()
    {
        if (m_fadeCoroutine != null)
        {
            StopCoroutine(m_fadeCoroutine);
        }
        m_fadeCoroutine = StartCoroutine(Fade(0.0f));
        yield return m_fadeCoroutine;
    }

    IEnumerator Fade(float target)
    {
        while(Mathf.Approximately(m_fadeAmount, target))
        {
            m_fadeAmount = Mathf.MoveTowards(m_fadeAmount, target, fadeSpeed * Time.deltaTime);
            screenFade.SetFloat(m_fadeAmountPropID, m_fadeAmount);
            yield return null;
        }

    }

}
