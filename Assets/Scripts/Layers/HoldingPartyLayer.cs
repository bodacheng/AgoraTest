using UnityEngine;
using Cysharp.Threading.Tasks;

public class HoldingPartyLayer : UILayer
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

