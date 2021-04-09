using Cysharp.Threading.Tasks;
using PlayFab;
using UnityEngine;
using System.Collections.Generic;

public class SingleModeLayer : UILayer
{
    // 查找远程信息从而确认是否在房间内
    public override async UniTask LayerEnter()
    {
        uiDirector.RefeshUI(GetLayerMark());
        var request = new PlayFab.ClientModels.GetUserDataRequest();
        request.Keys = new List<string>() { "OnPlanet" };
        PlayFabClientAPI.GetUserData(request, OnGetUser, null);
    }

    void OnGetUser(PlayFab.ClientModels.GetUserDataResult response)
    {
        if (response.Data.ContainsKey("OnPlanet"))
        {
            PlayFabHander.EnterAChannel(response.Data["OnPlanet"].Value);

            // 加入相应的星星
            if (response.Data["OnPlanet"].Value == SystemInfo.deviceUniqueIdentifier)
            {
                // 自己的星星
            }
            else
            {
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
