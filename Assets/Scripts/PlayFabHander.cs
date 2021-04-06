using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using System;
using PlayFab.AuthenticationModels;
using PlayFab.DataModels;
//using EntityKey = PlayFab.DataModels.EntityKey;
using PlayFab.GroupsModels;

public static class PlayFabHander
{
    static string entityId;
    static string entityType;
    static PlayFab.ClientModels.EntityKey myAccEntityKey;

    static string group_entityId;
    static string group_entityType;
    static PlayFab.GroupsModels.EntityKey currentGroupEntityKey;

    public static event Action<LoginResult> OnPlayFabLogin = delegate { };
    public static event Action<ListMembershipResponse> ProcessAfterGetGroups = delegate { };

    /// <summary>
    /// PlayFab的login，其中extraSucessProcess是外部带入的，希望在login成功瞬间做的事情
    /// 而PlayFabHander.OnLogin 是PlayFab login必须执行的事情，也就是获得这个玩家的entityId和entityType
    /// </summary>
    /// <param name="extraSucessProcess"></param>
    public static void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };

        // 固定执行PlayFabHander.OnLogin，同时还执行extraSucessProcess
        void SucessProcess(LoginResult loginResult)
        {
            OnLoginRegular(loginResult);
            OnPlayFabLogin.Invoke(loginResult);
        }
        PlayFabLogin.CustomIDLogin(request, SucessProcess, null);
    }

    // PlayFab Login Regular Process
    static void OnLoginRegular(LoginResult result)
    {
        entityId = result.EntityToken.Entity.Id;
        // The expected entity type is title_player_account.
        entityType = result.EntityToken.Entity.Type;
        myAccEntityKey = result.EntityToken.Entity;
    }

    // 目前测试中的进入语音频道与退出语音频道时触发的playfab系列处理
    public static void VoiceRoomsCheck(LoginResult loginResult)
    {
        CheckCurrentGroups(null);
    }

    // Shared Group 相关代码 // 
    // Group系统貌似能够辅助我们建立语音房间。目前确定OnListGroups可以获得group列表
    // 注意这个系列的函数经常有EntityKey的引数，往往这种情况下这个引数用null带进去，也能正常跑

    //public static readonly HashSet<KeyValuePair<string, string>> EntityGroupPairs = new HashSet<KeyValuePair<string, string>>();
    public static readonly Dictionary<string, string> GroupNameById = new Dictionary<string, string>();

    public static PlayFab.GroupsModels.EntityKey EntityKeyMaker(string entityId)
    {
        return new PlayFab.GroupsModels.EntityKey { Id = entityId };
    }

    static void CreateGroup(string groupName)
    {
        // A player-controlled entity creates a new group
        var request = new CreateGroupRequest { GroupName = groupName };
        PlayFabGroupsAPI.CreateGroup(request, OnCreateGroup, OnSharedError);
    }

    public static void DeleteGroup()
    {
        PlayFab.GroupsModels.EntityKey entityKey = new PlayFab.GroupsModels.EntityKey
        {
            Id = group_entityId,
            Type = group_entityType
        };
        Debug.Log("尝试删除playfab group:" + VoicePartyCenter.targetVoiceRoom);
        var request = new DeleteGroupRequest{ Group = entityKey };
        PlayFabGroupsAPI.DeleteGroup(request, OnDeleteGroup, OnSharedError);
    }

    private static void OnSharedError(PlayFab.PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    static void CheckCurrentGroups(PlayFab.GroupsModels.EntityKey entityKey)
    {
        var request = new ListMembershipRequest { Entity = entityKey };
        PlayFabGroupsAPI.ListMembership(request, ProcessAfterGetGroups, OnSharedError);
    }

    public static void CheckGroupsBeforeCreate(ListMembershipResponse response)
    {
        var prevRequest = (ListMembershipRequest)response.Request;
        foreach (var pair in response.Groups)
        {
            GroupNameById[pair.Group.Id] = pair.GroupName;
            Debug.Log("group id: " + GroupNameById[pair.Group.Id]);
            if (pair.GroupName == VoicePartyCenter.targetVoiceRoom)
            {
                // 加入
                Debug.Log("试图建立的房间已经存在？加入：" + pair.GroupName);
                VoicePartyCenter.Instance.GetIRtcEngine().JoinChannel(pair.GroupName, "extra", 0);
                return;
            }
            //EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));
        }
        CreateGroup(VoicePartyCenter.targetVoiceRoom);
    }

    public static void CheckGroupsBeforeJoin(ListMembershipResponse response)
    {
        var prevRequest = (ListMembershipRequest)response.Request;
        foreach (var pair in response.Groups)
        {
            GroupNameById[pair.Group.Id] = pair.GroupName;
            Debug.Log("group id: " + GroupNameById[pair.Group.Id]);
            VoicePartyCenter.Instance.GetIRtcEngine().JoinChannel(pair.GroupName, "extra", 0);
            return;
            //EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));
        }
    }

    private static void OnCreateGroup(CreateGroupResponse response)
    {
        Debug.Log("Group Created: " + response.GroupName + " - " + response.Group.Id);
        var prevRequest = (CreateGroupRequest)response.Request;
        currentGroupEntityKey = prevRequest.Entity;
        group_entityId = response.Group.Id;
        group_entityType = response.Group.Type;
        if (currentGroupEntityKey != null)
        {
            // 根本没跑这里面
            Debug.Log("已经获取了currentGroupEntityKey。ID ： "+ currentGroupEntityKey.Id + " Type:" + currentGroupEntityKey.Type);
        }
        //EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, response.Group.Id));
        GroupNameById[response.Group.Id] = response.GroupName;
        // Create Playfab Group 同时加入agora的语音房间
        VoicePartyCenter.Instance.GetIRtcEngine().JoinChannel(response.GroupName, "extra", 0);
    }

    private static void OnDeleteGroup(PlayFab.GroupsModels.EmptyResponse response)
    {
        Debug.Log("成功删除playfab的group");
    }

    public static void ApplyToGroup(string groupId, PlayFab.GroupsModels.EntityKey entityKey)
    {
        // A player-controlled entity applies to join an existing group (of which they are not already a member)
        var request = new ApplyToGroupRequest { Group = EntityKeyMaker(groupId), Entity = entityKey };
        PlayFabGroupsAPI.ApplyToGroup(request, OnApply, OnSharedError);
    }

    public static void OnApply(ApplyToGroupResponse response)
    {
        var prevRequest = (ApplyToGroupRequest)response.Request;

        // Presumably, this would be part of a separate process where the recipient reviews and accepts the request
        var request = new AcceptGroupApplicationRequest { Group = prevRequest.Group, Entity = prevRequest.Entity };
        PlayFabGroupsAPI.AcceptGroupApplication(request, OnAcceptApplication, OnSharedError);
    }

    public static void OnAcceptApplication(PlayFab.GroupsModels.EmptyResponse response)
    {
        var prevRequest = (AcceptGroupApplicationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
    }

    // PublisherData 相关代码. PublisherData到底什么时候用都不理解
    // 但playfab的其他playerdata的set与get大体也是相同的感觉
    public static void SetPublisherData(LoginResult loginResult)
    {
        PlayFabClientAPI.UpdateUserPublisherData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                     { "currentVoiceRoom", VoicePartyCenter.targetVoiceRoom }
                }
        },
            result => Debug.Log("Successfully updated user data:" + VoicePartyCenter.targetVoiceRoom),
            error =>
            {
                Debug.Log("Got error setting user data Ancestor to Arthur");
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }

    public static void GetPublisherData(LoginResult loginResult)
    {
        PlayFabClientAPI.GetUserPublisherData(
            new GetUserDataRequest()
            {
                PlayFabId = loginResult.PlayFabId
            },
            result => {
                Debug.Log("Got user data:");
                if (result.Data == null || !result.Data.ContainsKey("currentVoiceRoom"))
                {
                    Debug.Log("No currentVoiceRoom");
                }
                else
                {
                    VoicePartyCenter.targetVoiceRoom = result.Data["currentVoiceRoom"].Value;
                    VoicePartyCenter.Instance.GetIRtcEngine().JoinChannel(VoicePartyCenter.targetVoiceRoom, "extra", 0);
                    Debug.Log("helloCurrentrooom :" + VoicePartyCenter.targetVoiceRoom);
                }
            },
            (error) => {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }

    // PlayFab  实体（Entity）相关代码 //
    // 从内容来看应该是用来更新和获取每一个玩家的私有属性
    // 但实体这个概念在playfab里貌似范围非常广，到底这个实体是玩家数据，还是游戏数据，由entityType的值决定
    // 这个entityType貌似有若干几个固定值
    // 而且官方大体说了这样一个意思：更新玩家数据建议使用实体系统
    // 其他有待继续研究
    static void GetEntityTokenExample()
    {
        PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(),
        (entityResult) =>
        {
            var entityId = entityResult.Entity.Id;
            var entityType = entityResult.Entity.Type;
        }, null); // Define your own OnPlayFabError function to report errors
    }

    static void SetEntityObjectsExample()
    {
        var data = new Dictionary<string, object>()
        {
            {"Health", 100},
            {"Mana", 10000}
        };

        var dataList = new List<SetObject>()
        {
            new SetObject()
            {
                ObjectName = "PlayerData",
                DataObject = data
            },
            // A free-tier customer may store up to 3 objects on each entity
        };

        PlayFabDataAPI.SetObjects(new SetObjectsRequest()
        {
            Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType }, // Saved from GetEntityToken, or a specified key created from a titlePlayerId, CharacterId, etc
            Objects = dataList,
        }, (setResult) => {
            Debug.Log(setResult.ProfileVersion);
        }, null);
    }

    static void GetObjectsRequestExample()
    {
        var getRequest = new GetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType } };
        PlayFabDataAPI.GetObjects(getRequest,
            result => { var objs = result.Objects; },
            null
        );
    }
}
