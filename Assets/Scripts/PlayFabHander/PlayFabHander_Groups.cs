using UnityEngine;
using PlayFab;
using PlayFab.GroupsModels;
using System;

public static partial class PlayFabHander
{
    static string tempGroupName = "fjiewjfoiwejoifjwoiefjewif"; 

    public static string targetGroupName;// group自定义昵称
    public static string targetPlayfabGroupName;//机器码

    //public static readonly HashSet<KeyValuePair<string, string>> EntityGroupPairs = new HashSet<KeyValuePair<string, string>>();
    public static event Action<ListMembershipResponse> ProcessAfterGetGroups = delegate { };

    /// <summary>
    /// 检查目前在playfab上的group列表
    /// 检查完了后怎么做，根据情况不同而定（创建房间还是随机加入房间）
    /// </summary>
    /// <param name="entityKey"></param>
    static void CurrentVoiceChannlesCheck(EntityKey entityKey)
    {
        var request = new ListMembershipRequest { Entity = entityKey };
        PlayFabGroupsAPI.ListMembership(request, ProcessAfterGetGroups, OnSharedError);
    }

    // 下面两个函数在实际运行时候是紧随上面

    /// <summary>
    /// 试图创建新房间，但先检查下同名房间是否已经存在，存在的话直接加入。
    /// </summary>
    /// <param name="response"></param>
    public static void CheckGroupsBeforeCreate(ListMembershipResponse response)
    {
        foreach (var pair in response.Groups)
        {
            //Debug.Log("存在以下 group name:" + pair.GroupName);
            //Debug.Log("roles count:" + pair.Roles.Count);
            //Debug.Log("group id:" + pair.Group.Id);

            if (pair.GroupName == tempGroupName)
            {
                // 加入
                Debug.Log("试图建立的房间已经存在？加入：" + pair.GroupName);
                EnterAChannel(tempGroupName);
                return;
            }
            //EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));
        }
        // 确定没有重名房间已经存在的话，就以自身机器码为名称建立房间
        // 而昵称其实是记在了PlayFabHander.targetGroupName里，随后的成功处理中会以之命名sharedgroupdata
        CreateGroup(tempGroupName);
    }

    /// <summary>
    /// 随机加入语聊
    /// </summary>
    /// <param name="response"></param>
    public static void CheckGroupsBeforeJoin(ListMembershipResponse response)
    {
        foreach (var pair in response.Groups)
        {
            //EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));
            // 不加入自己的星球
            if (pair.GroupName != SystemInfo.deviceUniqueIdentifier)
            {
                EnterAChannel(pair.GroupName);
                return;
            }
        }
        Debug.Log(" 没有找到正在语聊中的房间。 ");
        UIDirector.Instance.RefeshUI(LayerMark.Lobby);
    }

    static void RemoveMember(string memberid)
    {
        PlayFabGroupsAPI.RemoveMembers(
            new RemoveMembersRequest() {
                Group = new EntityKey
                {
                    Id = targetPlayfabGroupName,
                    Type = "group"
                },
                Members = new System.Collections.Generic.List<EntityKey>
                {
                    new EntityKey
                    {
                        Id = memberid,
                        Type = "title_player_account"
                    }
                }
            },
            on => {
            },
            OnSharedError);
    }

    /// <summary>
    /// Group 的建立为的是其他玩家能够找到这个房间，因为playfab提供group列表
    /// 而shared group data是为了保存某个房间的数据。因为目前的调查来看group那边貌似保存不了数据，但shareddatagroup却可以
    /// </summary>
    /// <param name="groupName"> group 的昵称</param>
    static void CreateGroup(string groupName)
    {
        PlayFabGroupsAPI.CreateGroup(
            new CreateGroupRequest()
            {
                GroupName = groupName
            },
            resultCallback => {
                Debug.Log("成功创建了group" + groupName);
                // Group创建成功后，继而创建SharedGroup
                EnterAChannel(tempGroupName);
                //CreateSharedGroup(
                //    targetGroupName,
                //    tempGroupName,
                //    PlayFabHander.OnCreateSharedGroup,
                //    () => EnterAChannel(tempGroupName));
            },
            resultCallback => {
                EnterAChannel(tempGroupName);
                //CreateSharedGroup(
                //    targetGroupName,
                //    tempGroupName,
                //    PlayFabHander.OnCreateSharedGroup,
                //    () => EnterAChannel(tempGroupName));
            }
        );
    }

    // 当房间人数为0的时候，由客户端主动删除房间.必须同时删除所有成员
    public static void DeleteGroup(string groupName)
    {
        PlayFabGroupsAPI.GetGroup(
            new GetGroupRequest() {
                GroupName = groupName
            }, Temp, OnSharedError);

        void Temp(GetGroupResponse response)
        {
            Debug.Log("尝试将已经没有通话的语音房间删除：" + groupName);

            PlayFabGroupsAPI.RemoveMembers(
                new RemoveMembersRequest(),
                    on => {
                    PlayFabGroupsAPI.DeleteGroup(
                        new DeleteGroupRequest()
                        {
                            Group = new EntityKey
                            {
                                Id = response.Group.Id,
                                Type = "group"
                            }
                        },
                        resultCallback => {
                            Debug.Log("group已经删除" + groupName);
                            RemoveSharedGroupMember(groupName, PlayFabHander.myPlayFabId);
                        },
                        OnSharedError
                    );
                },
            OnSharedError);
        }
    }
}
