using Cysharp.Threading.Tasks;
using PlayFab;
using UnityEngine;
using System.Collections.Generic;

public class LobbyLayer : UILayer
{
    // 查找远程信息从而确认是否在房间内
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
