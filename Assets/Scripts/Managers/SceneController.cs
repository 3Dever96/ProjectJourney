using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [SerializeField] private Camera fadeCamera;

    [SerializeField] private string[] defaultScenesToLoad;
    [SerializeField] private List<string> currentScenes;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed;

    private void Awake()
    {
        instance = this;

        LoadScenes(defaultScenesToLoad, null, true);
    }

    public void LoadScenes(string[] scenesToLoad, string[] scenesToUnload, bool useFade)
    {
        StartCoroutine(ChangeScenes(scenesToLoad, scenesToUnload, useFade));
    }

    public IEnumerator ChangeScenes(string[] scenesToLoad, string[] scenesToUnload, bool useFade)
    {
        float a = fadeImage.color.a;

        if (useFade)
        {
            // 1. FADE OUT TO BLACK (Only if we aren't already black)
            while (a < 1f)
            {
                a += fadeSpeed * Time.deltaTime;
                // Clamp to prevent exceeding 1.0
                a = Mathf.Min(a, 1f);
                fadeImage.color = new Color(0, 0, 0, a);
                yield return null;
            }
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }

        // 2. UNLOAD OLD SCENES (Track operations to wait for completion)
        if (scenesToUnload != null && scenesToUnload.Length > 0)
        {
            List<AsyncOperation> unloadOperations = new List<AsyncOperation>();

            for (var i = 0; i < scenesToUnload.Length; i++)
            {
                if (currentScenes.Contains(scenesToUnload[i]))
                {
                    AsyncOperation op = SceneManager.UnloadSceneAsync(scenesToUnload[i]);
                    if (op != null) unloadOperations.Add(op);
                    currentScenes.Remove(scenesToUnload[i]);
                }
            }

            // Wait until every single unload operation is 100% complete
            foreach (var op in unloadOperations)
            {
                while (!op.isDone) yield return null;
            }
        }

        // 3. LOAD NEW SCENES
        if (scenesToLoad != null && scenesToLoad.Length > 0)
        {
            List<AsyncOperation> loadOperations = new List<AsyncOperation>();

            for (var i = 0; i < scenesToLoad.Length; i++)
            {
                if (!currentScenes.Contains(scenesToLoad[i]))
                {
                    AsyncOperation op = SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
                    if (op != null) loadOperations.Add(op);
                    currentScenes.Add(scenesToLoad[i]);
                }
            }

            // Wait until every single new scene is completely loaded into memory
            foreach (var op in loadOperations)
            {
                while (!op.isDone) yield return null;
            }
        }

        // 4. CAMERA HANDSHAKE PLACEHOLDER
        // This is the exact moment where you find the new camera 
        // and link your overlay stack or adjust priorities before the player sees anything!
        Camera newMainCamera = Camera.main;
        if (newMainCamera != null)
        {
            UniversalAdditionalCameraData fade = fadeCamera.GetUniversalAdditionalCameraData();
            fade.renderType = CameraRenderType.Overlay;

            UniversalAdditionalCameraData main = newMainCamera.GetUniversalAdditionalCameraData();
            main.cameraStack.Add(fadeCamera);
        }

        if (useFade)
        {
            // 5. FADE IN TO REVEAL GAMEPLAY
            while (a > 0f)
            {
                a -= fadeSpeed * Time.deltaTime;
                // Clamp to prevent dropping below 0.0
                a = Mathf.Max(a, 0f);
                fadeImage.color = new Color(0, 0, 0, a);
                yield return null;
            }
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }
}
