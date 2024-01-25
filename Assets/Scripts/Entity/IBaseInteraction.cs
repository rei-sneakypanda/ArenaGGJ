using Cysharp.Threading.Tasks;

public interface IBaseInteraction
{
    public UniTask Interact(GameEntity entity, GameEntity otherEntity);
}