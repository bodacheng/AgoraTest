using Cysharp.Threading.Tasks;

public class PartyLayer : UILayer
{
    // 不带房间名进入，大体代表随机参加
    public override async UniTask LayerEnter()
    {
        uiDirector.RefeshUI(LayerMark.Null);

        PlayFabHander.DisposeCustomOnPlayFabJoinAgoraChannel();
        PlayFabHander.CustomOnPlayFabJoinAgoraChannel += PlayFabHander.VoiceRoomsCheck;

        PlayFabHander.DisposeProcessAfterGetGroups();
        PlayFabHander.ProcessAfterGetGroups += PlayFabHander.CheckGroupsBeforeJoin;
        PlayFabHander.TryStartJoinAgoraProcess();
    }

    // 带房间名进入，意思是建房
    public override async UniTask LayerEnter<T>(T t)
    {
        PlayFabHander.targetGroupName = t.ToString();

        PlayFabHander.DisposeCustomOnPlayFabJoinAgoraChannel();
        PlayFabHander.CustomOnPlayFabJoinAgoraChannel += PlayFabHander.VoiceRoomsCheck;

        PlayFabHander.DisposeProcessAfterGetGroups();
        PlayFabHander.ProcessAfterGetGroups += PlayFabHander.CheckGroupsBeforeCreate;
        PlayFabHander.TryStartJoinAgoraProcess();
    }

    public override async UniTask LayerEnd()
    {

    }
}