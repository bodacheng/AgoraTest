using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using System;
//using EntityKey = PlayFab.DataModels.EntityKey;
using PlayFab.GroupsModels;

public static partial class PlayFabHander
{
    static string entityId;
    static string entityType;

    public static event Action<LoginResult> CustomOnPlayFabLogin = delegate { };

    /// <summary>
    /// Enter a channel, call this whenever you enter a planet 
    /// </summary>
    /// <param name="groupid"></param>
    public static void EnterAChannel(string groupid)
    {
        // 1. 加入到PlayFab的shared group里
        AddSharedGroupMember(groupid);
        // 2. 更新玩家在playfab的数据 （ onplanet ）
        PlayFabClientAPI.UpdateUserData
        (
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>()
                {
                    { "OnPlanet", groupid },
                }
            },
            result => {
                Debug.Log("Successfully updated user data");
                // 3. 进去agora语音频道
                VoicePartyCenter.Instance.GetIRtcEngine().JoinChannel(groupid, "extra", 0);
            },
            error => {
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }

    /// <summary>
    /// 退出频道
    /// </summary>
    public static void Quit()
    {
        // 点击退出的话首先一定会从agora音声频道退出
        // 是否删除掉playfab上对应的group靠的应该是判断声网对应频道内还有没有玩家
        // 没有的话，直接就把group删除。因为group的存在本来就是用来辅助我们记录到底有什么频道正在通话
        // 对应的处理其实是在LeaveChannel所触发的行为里，在VoicePartyCenter那边。
        VoicePartyCenter.Instance.LeaveChannel();

        // 然后，应该抹除掉在playfab上关于「在哪个星球」的信息
        PlayFabClientAPI.UpdateUserData
        (
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>()
                {
                    { "OnPlanet", null },
                }
            },
            result => {
                Debug.Log("已经把「在哪个星球」这个信息删除");
            },
            error => {
                Debug.Log(error.GenerateErrorReport());
            }
        );
        // 
    }

    /// <summary>
    /// Login into playfab. this is the start of the whole travel process
    /// </summary>
    public static void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };

        void SucessProcess(LoginResult loginResult)
        {
            OnLoginRegular(loginResult);
            CustomOnPlayFabLogin.Invoke(loginResult);
        }
        PlayFabLogin.CustomIDLogin(request, SucessProcess, null);
    }

    // PlayFab Login Regular Process
    static void OnLoginRegular(LoginResult result)
    {
        entityId = result.EntityToken.Entity.Id;
        // The expected entity type is title_player_account.
        entityType = result.EntityToken.Entity.Type;
    }

    // 目前测试中的进入语音频道与退出语音频道时触发的playfab系列处理
    public static void VoiceRoomsCheck(LoginResult loginResult)
    {
        CurrentVoiceChannlesCheck(null);
    }

    private static void OnSharedError(PlayFab.PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public static void OnAcceptApplication(PlayFab.GroupsModels.EmptyResponse response)
    {
        var prevRequest = (AcceptGroupApplicationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
    }
}
