using Cysharp.Threading.Tasks;

public abstract class UILayer
{
    LayerMark layerMark;
    public void SetLayerMark(LayerMark value)
    {
        layerMark = value;
    }
    public LayerMark GetLayerMark()
    {
        return layerMark;
    }

    public UIDirector uiDirector;

    public virtual async UniTask LayerEnter()
    {
    }

    public virtual async UniTask LayerEnter<T>(T t)
    {
    }

    public virtual async UniTask LayerEnd()
    {
    }
}

public enum LayerMark
{
    Login,
    Lobby,
    PartySetting,
    Party
}