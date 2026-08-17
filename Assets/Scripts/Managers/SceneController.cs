using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for accessing URP camera data and camera stacking.
using UnityEngine.SceneManagement;      // Required for additive scene loading and unloading management.
using UnityEngine.UI;                 // Required for managing the UI Image component used for screen transitions.

public class SceneController : MonoBehaviour
{
    // Singleton pattern instance allowing global access from anywhere in the project.
    public static SceneController instance;

    [Header("Camera Settings")]
    // The persistent UI/Fade camera dedicated to rendering the black overlay plane.
    [SerializeField] private Camera fadeCamera;

    [Header("Scene Collections")]
    // Array containing names of the primary scenes automatically loaded on game launch.
    [SerializeField] private string[] defaultScenesToLoad;

    // Tracks currently active sub-scenes to prevent duplicate additive loading conflicts.
    [SerializeField] private List<string> currentScenes;

    [Header("UI Visual Configuration")]
    // Fullscreen graphic panel used to block out gameplay during transitions.
    [SerializeField] private Image fadeImage;

    // Interpolation multiplier regulating the duration of visual fade-ins and fade-outs.
    [SerializeField] private float fadeSpeed;

    private void Awake()
    {
        // Enforces basic Singleton pattern structure. 
        // Note: Missing standard safety checks or DontDestroyOnLoad if switching root managers.
        instance = this;

        // Immediately initializes game world by dynamically appending default persistent sub-scenes.
        LoadScenes(defaultScenesToLoad, null, true);
    }

    /// <summary>
    /// Safe public wrapper to safely trigger asynchronous scene transitions without invoking coroutines directly.
    /// </summary>
    public void LoadScenes(string[] scenesToLoad, string[] scenesToUnload, bool useFade)
    {
        StartCoroutine(ChangeScenes(scenesToLoad, scenesToUnload, useFade));
    }

    /// <summary>
    /// Step-by-step master coroutine coordinating visual fading, asynchronous unloads, and clean URP camera re-stacking.
    /// </summary>
    public IEnumerator ChangeScenes(string[] scenesToLoad, string[] scenesToUnload, bool useFade)
    {
        // Extracts the starting transparency of the fading canvas panel layer.
        float a = fadeImage.color.a;

        if (useFade)
        {
            // 1. FADE OUT TO BLACK (Only if we aren't already black)
            // Loops until alpha value reaches solid opacity (1.0).
            while (a < 1f)
            {
                // Smoothly increments visibility linearly relative to delta frame durations.
                a += fadeSpeed * Time.deltaTime;

                // Clamp to prevent exceeding 1.0
                a = Mathf.Min(a, 1f);
                fadeImage.color = new Color(0, 0, 0, a);

                // Suspends execution and yields control back to Unity until the subsequent frame update.
                yield return null;
            }
        }
        else
        {
            // Instantly blanks out screen opacity to absolute transparency if visual fade is disabled.
            fadeImage.color = new Color(0, 0, 0, 0);
        }

        // 2. UNLOAD OLD SCENES (Track operations to wait for completion)
        // Sanitizes parameters to guarantee safe container iterations.
        if (scenesToUnload != null && scenesToUnload.Length > 0)
        {
            // Collection storing ongoing memory release threads.
            List<AsyncOperation> unloadOperations = new List<AsyncOperation>();

            for (var i = 0; i < scenesToUnload.Length; i++)
            {
                // Only discards a scene if it is explicitly registered as currently active.
                if (currentScenes.Contains(scenesToUnload[i]))
                {
                    // Triggers multi-threaded asynchronous unload thread safely from engine memory.
                    AsyncOperation op = SceneManager.UnloadSceneAsync(scenesToUnload[i]);

                    // Tracks non-null operations to monitor real-time background progress.
                    if (op != null) unloadOperations.Add(op);

                    // Removes tracking string immediately to free up future allocation cycles.
                    currentScenes.Remove(scenesToUnload[i]);
                }
            }

            // Wait until every single unload operation is 100% complete
            // Traverses tracking collection to freeze scene flow execution until memory releases conclude.
            foreach (var op in unloadOperations)
            {
                while (!op.isDone) yield return null; // Pauses frame loop execution until individual operation marks isDone.
            }
        }

        // 3. LOAD NEW SCENES
        if (scenesToLoad != null && scenesToLoad.Length > 0)
        {
            // Collection storing upcoming async scene assembly tasks.
            List<AsyncOperation> loadOperations = new List<AsyncOperation>();

            for (var i = 0; i < scenesToLoad.Length; i++)
            {
                // Protects against performance drops or error spam caused by accidentally double-loading scenes.
                if (!currentScenes.Contains(scenesToLoad[i]))
                {
                    // Asynchronously bundles assets using Additive mode to retain root persistent controllers.
                    AsyncOperation op = SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
                    if (op != null) loadOperations.Add(op);
                    currentScenes.Add(scenesToLoad[i]);
                }
            }

            // Wait until every single new scene is completely loaded into memory
            // Freezes sequence progression to guarantee newly populated GameObjects exist before script linkages happen.
            foreach (var op in loadOperations)
            {
                while (!op.isDone) yield return null;
            }
        }

        // 4. CAMERA HANDSHAKE PLACEHOLDER
        // This is the exact moment where you find the new camera 
        // and link your overlay stack or adjust priorities before the player sees anything!
        Camera newMainCamera = Camera.main; // Evaluates active world using standard main camera tagging lookup.
        if (newMainCamera != null)
        {
            // Forces the fade camera into URP Overlay mode to render on top of default scenes.
            UniversalAdditionalCameraData fade = fadeCamera.GetUniversalAdditionalCameraData();
            fade.renderType = CameraRenderType.Overlay;

            // Grabs structural URP setup parameters from the newly discovered target camera.
            UniversalAdditionalCameraData main = newMainCamera.GetUniversalAdditionalCameraData();

            // Appends the fade camera onto the active render queue stack so UI transitions mask everything correctly.
            main.cameraStack.Add(fadeCamera);
        }

        if (useFade)
        {
            // 5. FADE IN TO REVEAL GAMEPLAY
            // Decrements overlay transparency back down to transparent zero.
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
            // Guarantees zero residual visual overlay artifacts remain.
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }
}
