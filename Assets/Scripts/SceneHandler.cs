using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneHandler
{
    event Action OnSceneLoaded;
    event Action OnSceneUnloaded;
    event Action OnSceneHandlerDestroyed;
    UniTask UnloadScene(int sceneIndex, bool unloadUnusedAssets = true);
    UniTask LoadSceneAdditive(int sceneIndex, bool unloadPrevious);
    UniTask LoadSceneSingle(int sceneIndex);
}

public class SceneHandler : ISceneHandler
{
    /// <summary>
    /// Both single and additive
    /// </summary>
    public event Action OnSceneLoaded;
    public event Action OnSceneUnloaded;
    public event Action OnSceneHandlerDestroyed;

    private const float SCENE_LOADED_OFFSET = 0.9f;

    protected HashSet<int> _activeScenes = new HashSet<int>();
    private int _previousSceneBuildIndex = -1;
    public SceneHandler(bool hasPersistentScene)
    {
        if (hasPersistentScene == false)
        {
            if (Application.isPlaying)
                SceneManager.CreateScene("MUST HAVE ONE SCENE");
            Debug.LogWarning("You have to always have at least one scene active in order to unload scenes. there for we created you an empty scene\nPlease note that we recommend starting the game from scene 0!");
            _previousSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            _activeScenes.Add(_previousSceneBuildIndex);
        }
    }

    public int[] GetActiveScenes()
    {
        int[] _copyScene = new int[_activeScenes.Count];
        int _counter = -1;

        foreach (int activeScene in _activeScenes)
        {
            _counter++;
            _copyScene[_counter] = activeScene;
        }

        return _copyScene;
    }


    public async UniTask LoadSceneSingle(int sceneIndex)
    {
        await (LoadSceneSingleTask(sceneIndex));
    }
    public async UniTask LoadSceneAdditive(int sceneIndex)
    {
        await LoadSceneAdditive(sceneIndex, false);
    }
    public async UniTask LoadSceneAdditive(int sceneIndex, bool unloadPrevious)
    {
        await LoadSceneAdditiveTask(sceneIndex, unloadPrevious);
    }

    public async UniTask UnloadScene(int sceneIndex, bool unloadUnUsedAssets = true)
    {
        if (_activeScenes.Contains(sceneIndex))
            await UnLoadScene(sceneIndex, unloadUnUsedAssets);
    }

    #region Loading Tasks
    protected async UniTask LoadSceneSingleTask(int nextSceneIndex)
    {
        OnSceneUnloaded?.Invoke();
        OnSceneLoaded?.Invoke();
        await SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Single);
    }
    protected async UniTask LoadSceneAdditiveTask(int nextSceneIndex, bool unloadPrevious)
    {

        if (unloadPrevious && _activeScenes.Contains(_previousSceneBuildIndex))
        {
            await UnLoadScene(_previousSceneBuildIndex, false);
            await UniTask.Yield();
        }
        await Resources.UnloadUnusedAssets();

        await LoadSceneAdditiveTask(nextSceneIndex);

        _activeScenes.Add(nextSceneIndex);
        _previousSceneBuildIndex = nextSceneIndex;
    }
    protected async UniTask LoadSceneAdditiveTask(int addedScene)
    {
        UnityEngine.AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(addedScene, LoadSceneMode.Additive);

        asyncOperation.allowSceneActivation = false;

        do
        {
            await UniTask.Yield();

            if (asyncOperation.progress >= SCENE_LOADED_OFFSET)
                asyncOperation.allowSceneActivation = true;
        }
        while (!asyncOperation.isDone);

        await UniTask.Yield();

        OnSceneLoaded?.Invoke();
    }
    protected async UniTask UnLoadScene(int index, bool unloadUnUsedAssets)
    {
        //Notifying relevant listeners that scene will be Unloaded
        OnSceneUnloaded?.Invoke();
        //Unloading scene
        await SceneManager.UnloadSceneAsync(index);
        //Add it to active scenes
        _activeScenes.Remove(index);
        // unload any un unsed assets
        if (unloadUnUsedAssets)
            await Resources.UnloadUnusedAssets();
    }
    #endregion


    #region MonoBehaviour

    protected virtual void OnDestroy()
    {
        if (OnSceneHandlerDestroyed != null)
            OnSceneHandlerDestroyed.Invoke();


    }
    #endregion

}