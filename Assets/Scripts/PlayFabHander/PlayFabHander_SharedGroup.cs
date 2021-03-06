using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public static partial class PlayFabHander
{
    /// <summary>
    /// 
    /// </summary>
    static void CreateSharedGroup(string groupNickName, string SharedGroupId, Action<CreateSharedGroupResult> onCreateAction, Action onSharedGroupExistAction)
    {
        PlayFabClientAPI.GetSharedGroupData(
            new GetSharedGroupDataRequest()
            {
                SharedGroupId = SharedGroupId
            },
            Temp,
            OnSharedError
        );
        void Temp(GetSharedGroupDataResult response)
        {
            foreach (var kv in response.Data)
            {
                UpdateSharedGroupDataRequest request = new UpdateSharedGroupDataRequest
                {
                    SharedGroupId = SharedGroupId,
                    Data = new Dictionary<string, string>
                    {
                        { "name", groupNickName }
                    }
                };
                PlayFabClientAPI.UpdateSharedGroupData(request, OnUpdateSharedGroupData, OnSharedError, request.SharedGroupId);
                onSharedGroupExistAction.Invoke();//更新sharedgroup的名字
                return;
            }
            Debug.Log("房间不存在或不正常，尝试创建.sharedGroupid:" + SharedGroupId);
            PlayFabClientAPI.CreateSharedGroup(
                new CreateSharedGroupRequest()
                {
                    SharedGroupId = SharedGroupId
                },
                onCreateAction,
                OnSharedError,
                groupNickName
            );
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

    static void AddSharedGroupMember(string SharedGroupId, string playFabId)
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
                PlayFabIds = new List<string>() { playFabId }
            };
            Debug.Log("尝试把玩家" + playFabId + "加入SharedGroup");
            PlayFabClientAPI.AddSharedGroupMembers(
                request,
                resultCallback => { Debug.Log("sucessfully add player into sharedgroup"); },
                OnSharedError
            );
        }
    }

    static void RemoveSharedGroupMember(string SharedGroupId, string playFabId)
    {
        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
        {
            GetMembers = true,
            SharedGroupId = SharedGroupId
        }, Temp, null);

        void Temp(GetSharedGroupDataResult response)
        {
            foreach (string member in response.Members)
            {
                if (member == playFabId)
                {
                    RemoveSharedGroupMembersRequest request = new RemoveSharedGroupMembersRequest()
                    {
                        SharedGroupId = SharedGroupId,
                        PlayFabIds = new List<string>() { playFabId }
                    };
                    PlayFabClientAPI.RemoveSharedGroupMembers(
                        request,
                        resultCallback => { Debug.Log("sucessfully remove player from sharedgroup"); },
                        OnSharedError);
                    return;
                }
            }
        }
    }
}
