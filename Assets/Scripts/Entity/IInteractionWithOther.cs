using Cysharp.Threading.Tasks;

public interface IInteractionWithOther
{
    public UniTask Interact(GameEntity entity, GameEntity otherEntity);
}

public interface IInteractionOnSelf
{
    public UniTask Interact(GameEntity entity);
}

public static class ProbabilityExtensions
{
    public static bool GenerateRNDOption(this float chance)
    {
        return UnityEngine.Random.Range(0,1f) <= chance;
    }
}