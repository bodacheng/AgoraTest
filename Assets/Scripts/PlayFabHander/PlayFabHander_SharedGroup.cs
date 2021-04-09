using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public static partial class PlayFabHander
{
    static void CreateSharedGroup(string groupName)
    {
        PlayFabClientAPI.GetSharedGroupData(
            new GetSharedGroupDataRequest()
            {
                SharedGroupId = SystemInfo.deviceUniqueIdentifier
            },
            Temp,
            OnSharedError
        );
        void Temp(GetSharedGroupDataResult response)
        {
            bool normalChannelExist = false;
            foreach (var kv in response.Data)
            {
                if (kv.Key == "name")
                {
                    normalChannelExist = true;
                    Debug.Log("获取到房间名：" + kv.Value);
                }
                //Debug.Log(kv.Key + ":" + kv.Value);
            }
            if (!normalChannelExist)
            {
                Debug.Log("房间不存在或数值，尝试创建");
                PlayFabClientAPI.CreateSharedGroup(
                    new CreateSharedGroupRequest() {
                        SharedGroupId = SystemInfo.deviceUniqueIdentifier
                    },
                    OnCreateSharedGroup,
                    OnSharedError,
                    groupName
                );
            }
            else
            {
                // 加入
            };
        };
    }

    static void OnCreateSharedGroup(CreateSharedGroupResult response)
    {
        UpdateSharedGroupDataRequest request = new UpdateSharedGroupDataRequest
        {
            SharedGroupId = response.SharedGroupId,
            Data = new Dictionary<string, string>
            {
                { "name", response.CustomData.ToString() }
            }
        };
        PlayFabClientAPI.UpdateSharedGroupData(request, OnUpdateSharedGroupData, OnSharedError, request.SharedGroupId);
    }

    static void OnUpdateSharedGroupData(UpdateSharedGroupDataResult updateSharedGroupDataResult)
    {
        EnterAChannel(updateSharedGroupDataResult.CustomData.ToString());
    }

    static void AddSharedGroupMember(string SharedGroupId)
    {
        AddSharedGroupMembersRequest request = new AddSharedGroupMembersRequest() {
            SharedGroupId = SharedGroupId
        };
        PlayFabClientAPI.AddSharedGroupMembers(
            request,
            resultCallback => { Debug.Log("sucess"); },
            resultCallback => { Debug.Log("fail"); }
        );
    }
}
