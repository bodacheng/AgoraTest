using Cysharp.Threading.Tasks;

public class HoldingPartyLayer : UILayer
{
    // 不带房间名进入，大体代表随机参加
    public override async UniTask LayerEnter()
    {
        PlayFabHander.OnPlayFabLogin += PlayFabHander.VoiceRoomsCheck;
        PlayFabHander.ProcessAfterGetGroups += PlayFabHander.CheckGroupsBeforeJoin;
        // 随机获取现有房间得依靠playfab的远程存储，所以相关处理其实在PlayFabHander.VoiceRoomJoinedProcess里
        PlayFabHander.Login();
    }

    // 带房间名进入，意思是建房
    public override async UniTask LayerEnter<T>(T t)
    {
        VoicePartyCenter.targetVoiceRoom = t.ToString();
        PlayFabHander.OnPlayFabLogin += PlayFabHander.VoiceRoomsCheck;
        PlayFabHander.ProcessAfterGetGroups += PlayFabHander.CheckGroupsBeforeCreate;
        PlayFabHander.Login();
    }

    public override async UniTask LayerEnd()
    {
    }
}