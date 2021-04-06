using Cysharp.Threading.Tasks;

public class PartySettingLayer : UILayer
{
    public override async UniTask LayerEnter()
    {
        uiDirector.RefeshUI(GetLayerMark());
    }

    public override async UniTask LayerEnter<T>(T t)
    {
    }

    public override async UniTask LayerEnd()
    {
    }
}
