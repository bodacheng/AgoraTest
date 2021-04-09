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
            Debug.Log("获得的SharedGroupData如下："+ response.Data.ToString());
            bool normalChannelExist = false;

            foreach (var kv in response.Data)
            {
                if (kv.Key == "name")
                {
                    normalChannelExist = true;
                    Debug.Log("获取到房间名：" + kv.Value.ToJson());
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
        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest() {
            GetMembers = true,
            SharedGroupId = SharedGroupId
        }, Temp, null);

        void Temp(GetSharedGroupDataResult response)
        {
            foreach (string member in response.Members)
            {
                if (member == PlayFabHander.myPlayFabId)
                {
                    Debug.Log("Player " + member + " already in SharedGroupData");
                    return;
                }
            }

            AddSharedGroupMembersRequest request = new AddSharedGroupMembersRequest()
            {
                SharedGroupId = SharedGroupId,
                PlayFabIds = new List<string>() { PlayFabHander.myPlayFabId }
            };
            Debug.Log("尝试把玩家" + PlayFabHander.myPlayFabId + "加入SharedGroup");
            PlayFabClientAPI.AddSharedGroupMembers(
                request,
                resultCallback => { Debug.Log("sucessfully add player into sharedgroup"); },
                OnSharedError
            );
        }
    }
}
