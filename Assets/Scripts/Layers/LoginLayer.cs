using Cysharp.Threading.Tasks;
using UnityEngine;
using PlayFab;
using System.Collections.Generic;

public class LoginLayer : UILayer
{
    // 不带房间名进入，大体代表随机参加
    public override async UniTask LayerEnter()
    {
        uiDirector.RefeshUI(LayerMark.Null);
        PlayFabHander.DisposeCustomOnPlayFabLogin();
        PlayFabHander.CustomOnPlayFabLogin += ToLobby;
        PlayFabHander.Login();        
    }

    public override async UniTask LayerEnd()
    {
    }

    async void ToLobby(PlayFab.ClientModels.LoginResult loginResult)
    {
        PlayFabHander.myPlayFabId = loginResult.PlayFabId;
        var request = new PlayFab.ClientModels.GetUserDataRequest();
        request.Keys = new List<string>() { "OnPlanet" };
        PlayFabClientAPI.GetUserData(request, OnGetUser, null);
    }

    async void OnGetUser(PlayFab.ClientModels.GetUserDataResult response)
    {
        Debug.Log("以下是从playfab获得的玩家情报：" + response.Data);
        foreach (var kv in response.Data)
        {
            Debug.Log(kv.Key + " : " + kv.Value.Value);
        }
        Debug.Log("以上");

        if (response.Data.ContainsKey("OnPlanet"))
        {
            // 加入相应的星星
            if (response.Data["OnPlanet"].Value == SystemInfo.deviceUniqueIdentifier)
            {
                Debug.Log("我在自己星球的party：" + response.Data["OnPlanet"].Value);
                PlayFabHander.EnterAChannel(response.Data["OnPlanet"].Value);
            }
            else
            {
                Debug.Log("他人の星のパティー：" + response.Data["OnPlanet"].Value);
                // 他人的星星
                PlayFabHander.EnterAChannel(response.Data["OnPlanet"].Value);
            }
        }
        else
        {
            await LayerRunner.Main.ChangeProcess(LayerMark.Lobby);
        }
    }
}