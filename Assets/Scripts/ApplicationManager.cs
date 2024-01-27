using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
  public static ApplicationManager Instance { get; private set; }
  public SceneHandler SceneHandler = new(true);

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    SceneHandler.LoadSceneAdditive(SceneConst.GAME)
      .Forget();
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
      Quit();
  }

  public void Quit()
  {
    Application.Quit();
  }

  public async UniTask Restart()
  {
    await SceneHandler.UnloadScene(SceneConst.GAME);
    await UniTask.DelayFrame(1);
    SceneHandler.LoadSceneSingle(SceneConst.GAME)
      .Forget();
  }

  private void OnDestroy()
  {
    Instance = null;
  }
}
