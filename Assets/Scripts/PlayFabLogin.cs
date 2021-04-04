using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class PlayFabLogin
{
    public static void CustomIDLogin(LoginWithCustomIDRequest request, Action<LoginResult> sucess, Action<PlayFabError> fail)
    {
        PlayFabClientAPI.LoginWithCustomID(
            request,
            sucess,
            fail
        );
    }

    public static void IOSDeviceIDLogin()
    {
        PlayFabClientAPI.LoginWithIOSDeviceID (
            new LoginWithIOSDeviceIDRequest
            {
                CreateAccount = true
            },
            result =>
            {
                Debug.Log("playfab login successed?");
            },
            error =>
            {
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }
}
