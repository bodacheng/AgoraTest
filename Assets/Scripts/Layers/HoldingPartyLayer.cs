using UnityEngine;
using Cysharp.Threading.Tasks;

public class HoldingPartyLayer : UILayer
{
    // 不带房间名进入，大体代表随机参加
    public override async UniTask LayerEnter()
    {
        // 随机获取现有房间得依靠playfab的远程存储，所以相关处理其实在PlayFabHander.VoiceRoomJoinedProcess里
        PlayFabHander.Login(PlayFabHander.VoiceRoomJoinedProcess);
    }

    // 带房间名进入，意思是建房
    public override async UniTask LayerEnter<T>(T t)
    {
        VoicePartyCenter.currentVoiceRoom = t.ToString();
        VoicePartyCenter.Instance.GetIRtcEngine().JoinChannel(VoicePartyCenter.currentVoiceRoom, "extra", 0);
        uiDirector.RefeshUI(GetLayerMark());
        Debug.Log("CreateCurrentrooom :" + VoicePartyCenter.currentVoiceRoom);
        PlayFabHander.Login(PlayFabHander.VoiceRoomCreatedProcess);
    }

    public override async UniTask LayerEnd()
    {

    }
}

