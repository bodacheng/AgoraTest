using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoginLayer : UILayer
{
    // 不带房间名进入，大体代表随机参加
    public override async UniTask LayerEnter()
    {
        PlayFabHander.CustomOnPlayFabLogin += ToLobby;
        PlayFabHander.Login();        
    }

    public override async UniTask LayerEnd()
    {
    }

    async void ToLobby(PlayFab.ClientModels.LoginResult loginResult)
    {
        PlayFabHander.myPlayFabId = loginResult.PlayFabId;
        Debug.Log("登陆playfab后尝试转入大厅.我的PlayFabID："+ loginResult.PlayFabId);
        await LayerRunner.Main.ChangeProcess(LayerMark.Lobby);
    }
}