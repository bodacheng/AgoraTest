using Cysharp.Threading.Tasks;
using PlayFab;
using UnityEngine;
using System.Collections.Generic;

public class LobbyLayer : UILayer
{
    // 查找远程信息从而确认是否在房间内
    public override async UniTask LayerEnter()
    {
        var request = new PlayFab.ClientModels.GetUserDataRequest();
        request.Keys = new List<string>() { "OnPlanet" };
        PlayFabClientAPI.GetUserData(request, OnGetUser, null);
        uiDirector.RefeshUI(GetLayerMark());
    }

    void OnGetUser(PlayFab.ClientModels.GetUserDataResult response)
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
                Debug.Log("我在自己星球的party："+ response.Data["OnPlanet"].Value);
                PlayFabHander.EnterAChannel(response.Data["OnPlanet"].Value);
            }
            else
            {
                Debug.Log("他人の星のパティー：" + response.Data["OnPlanet"].Value);
                // 他人的星星
            }
        }
        else
        {
            // Do Nothing。 维持画面
        }
    }

    public override async UniTask LayerEnter<T>(T t)
    {

    }

    public override async UniTask LayerEnd()
    {
    }
}
