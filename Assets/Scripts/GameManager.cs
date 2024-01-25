using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }
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


  public void Quit()
  {
    Application.Quit();
  }

  public async UniTask Restart()
  {
    await SceneHandler.UnloadScene(SceneConst.GAME);
    SceneHandler.LoadSceneSingle(SceneConst.GAME)
      .Forget();
  }

  private void OnDestroy()
  {
    Instance = null;
  }
}
