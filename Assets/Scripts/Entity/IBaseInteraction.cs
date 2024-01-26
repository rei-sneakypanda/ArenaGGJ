using Cysharp.Threading.Tasks;

public interface IBaseInteraction
{
    public UniTask Interact(GameEntity entity, GameEntity otherEntity);
}

public static class ProbabilityExtensions
{
    public static bool GenerateRNDOption(this float chance)
    {
        return UnityEngine.Random.Range(0,1f) <= chance;
    }
}