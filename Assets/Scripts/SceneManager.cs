using System.Collections;
using System.Collections.Generic;
using extOSC;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private float sceneSwitchDelay = 0.1f;
    [SerializeField] private bool isSceneReloadDebug;

    [SerializeField] private float sceneTimer = 3600;
    [SerializeField] private float currentSceneTimer;

    private void Start()
    {
        currentSceneTimer = sceneTimer;
    }

    private void Update()
    {
        TimerCountdown();
    }

    private void SceneDebug()
    {
        if (isSceneReloadDebug)
        {
            isSceneReloadDebug = false;
            SwitchScene();
        }
    }

    private void TimerCountdown()
    {

        if (currentSceneTimer > 0)
        {
            currentSceneTimer -= Time.deltaTime;
        }
        else
        {
            if (InputAdapter.Instance.idle) SwitchScene();
        }

        if (Input.GetKeyDown(KeyCode.Space)) SwitchScene();
    }

    void SwitchScene()
    {
        StartCoroutine(SwitchSceneAfterDelay(sceneName));
        // Find the animator with the tag "CrossFade"
        Animator animator = GameObject.FindGameObjectWithTag("CrossFade")?.GetComponent<Animator>();
        // Trigger the animation if the animator is found
        if (animator != null)
        {
            animator.SetTrigger("CrossFadeStart");
        }
    }

    IEnumerator SwitchSceneAfterDelay(string nextSceneName)
    {

        yield return new WaitForSeconds(sceneSwitchDelay);

        // Load the next scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}