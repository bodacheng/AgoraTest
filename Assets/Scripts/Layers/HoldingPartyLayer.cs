using UnityEngine;
using Cysharp.Threading.Tasks;

public class HoldingPartyLayer : UILayer
{
    public override async UniTask LayerEnter()
    {
        PlayFabHander.Login(PlayFabHander.VoiceRoomJoinedProcess);
    }

    public override async UniTask LayerEnter<T>(T t)
    {
        VoicePartyCenter.currentVoiceRoom = t.ToString();
        PlayFabHander.Login(PlayFabHander.VoiceRoomCreatedProcess);
        uiDirector.RefeshUI(GetLayerMark());
        Debug.Log("CreateCurrentrooom :" + VoicePartyCenter.currentVoiceRoom);
    }

    public override async UniTask LayerEnd()
    {
    }
}

