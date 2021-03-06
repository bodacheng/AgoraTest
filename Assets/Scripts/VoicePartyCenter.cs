using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab.GroupsModels;
using PlayFab;
using UnityEngine.UI;
#if(UNITY_2018_3_OR_NEWER)
using UnityEngine.Android;
#endif
using agora_gaming_rtc;

// Core methods of voice channel join

public class VoicePartyCenter : MonoBehaviour
{
    // PLEASE KEEP THIS App ID IN SAFE PLACE
    // Get your own App ID at https://dashboard.agora.io/
    // After you entered the App ID, remove ## outside of Your App ID
    public string appId = "9c20adeaf06641ddb7248182316b7039";
    public PlayerInChannelList playerInChannelList;

    private IRtcEngine mRtcEngine = null;
    
    static uint my_uid;
    static List<uint> remoteStreams = new List<uint>();

    public static VoicePartyCenter Instance;

    List<AUDIO_EFFECT_PRESET> AllVoiceEffects = new List<AUDIO_EFFECT_PRESET>()
    {
        AUDIO_EFFECT_PRESET.AUDIO_EFFECT_OFF,
        AUDIO_EFFECT_PRESET.VOICE_CHANGER_EFFECT_BOY,
        AUDIO_EFFECT_PRESET.VOICE_CHANGER_EFFECT_GIRL,
        AUDIO_EFFECT_PRESET.VOICE_CHANGER_EFFECT_HULK,
        AUDIO_EFFECT_PRESET.VOICE_CHANGER_EFFECT_OLDMAN,
        AUDIO_EFFECT_PRESET.VOICE_CHANGER_EFFECT_SISTER,
        AUDIO_EFFECT_PRESET.VOICE_CHANGER_EFFECT_UNCLE,
        AUDIO_EFFECT_PRESET.ROOM_ACOUSTICS_KTV,
        AUDIO_EFFECT_PRESET.ROOM_ACOUSTICS_PHONOGRAPH,
        AUDIO_EFFECT_PRESET.ROOM_ACOUSTICS_SPACIAL,
        AUDIO_EFFECT_PRESET.ROOM_ACOUSTICS_STUDIO,
        AUDIO_EFFECT_PRESET.ROOM_ACOUSTICS_VIRTUAL_STEREO,
        AUDIO_EFFECT_PRESET.ROOM_ACOUSTICS_VOCAL_CONCERT,
        AUDIO_EFFECT_PRESET.STYLE_TRANSFORMATION_POPULAR,
        AUDIO_EFFECT_PRESET.STYLE_TRANSFORMATION_RNB,
        AUDIO_EFFECT_PRESET.VOICE_CHANGER_EFFECT_PIGKING
    };

    List<VOICE_BEAUTIFIER_PRESET> AllBEAUTIFIEREffects = new List<VOICE_BEAUTIFIER_PRESET>()
    {
         VOICE_BEAUTIFIER_PRESET.VOICE_BEAUTIFIER_OFF,
          VOICE_BEAUTIFIER_PRESET.CHAT_BEAUTIFIER_FRESH,
           VOICE_BEAUTIFIER_PRESET.CHAT_BEAUTIFIER_MAGNETIC,
            VOICE_BEAUTIFIER_PRESET.CHAT_BEAUTIFIER_VITALITY,
             VOICE_BEAUTIFIER_PRESET.TIMBRE_TRANSFORMATION_CLEAR,
              VOICE_BEAUTIFIER_PRESET.TIMBRE_TRANSFORMATION_DEEP,
               VOICE_BEAUTIFIER_PRESET.TIMBRE_TRANSFORMATION_FALSETTO,
                VOICE_BEAUTIFIER_PRESET.TIMBRE_TRANSFORMATION_FULL,
                 VOICE_BEAUTIFIER_PRESET.TIMBRE_TRANSFORMATION_MELLOW,
                  VOICE_BEAUTIFIER_PRESET.TIMBRE_TRANSFORMATION_RESOUNDING,
                   VOICE_BEAUTIFIER_PRESET.TIMBRE_TRANSFORMATION_RINGING,
                    VOICE_BEAUTIFIER_PRESET.TIMBRE_TRANSFORMATION_VIGOROUS,
    };

    public Text effecttext; 
    int i = 0, y = 0;
    public void SwitchVoice()
    {
        if (i < AllVoiceEffects.Count - 1)
            i++;
        else
            i = 0;
        effecttext.text = "effect " + Enum.GetName(typeof(AUDIO_EFFECT_PRESET), AllVoiceEffects[i]); 
        mRtcEngine.SetAudioEffectPreset(AllVoiceEffects[i]);
    }

    public Text beautifultext;
    public void SwitchAudioEffectPreset()
    {
        if (y < AllBEAUTIFIEREffects.Count - 1)
            y++;
        else
            y = 0;

        beautifultext.text = "beautiful " + Enum.GetName(typeof(VOICE_BEAUTIFIER_PRESET), AllBEAUTIFIEREffects[y]);
        mRtcEngine.SetVoiceBeautifierPreset(AllBEAUTIFIEREffects[y]);
    }

    public void ToAudience()
    {
        mRtcEngine.SetClientRole( CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE );
    }

    public void ToBroadCaster()
    {
        mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    }

    public IRtcEngine GetIRtcEngine()
    {
        return mRtcEngine;
    }

    void AddRemoteStreans(uint uid)
    {
        if (!remoteStreams.Contains(uid))
        {
            remoteStreams.Add(uid);
            Debug.Log("New player added uid:" + uid + ". Now there are " + remoteStreams.Count + "players in the room.");
            playerInChannelList.NewUserEnter(uid.ToString());
        }
        else
        {
            Debug.Log("Logic error.Try add a player that already exist :" + uid);
        }
    }

    void RemoveRemoteStreams(uint uid)
    {
        if (remoteStreams.Contains(uid))
        {
            remoteStreams.Remove(uid);
            Debug.Log("Now there are " + remoteStreams.Count + "players in the room.");
            playerInChannelList.UserQuit(uid.ToString());
        }
        else
        {
            Debug.Log("Logic error.Try remove a player that not exist in list:" + uid);
        }

        // remoteStreams 到底有没有正确反应房间的人数需要进一步研究
        if (remoteStreams.Count == 0)
        {
            //PlayFabHander.DeleteGroup(PlayFabHander.targetPlayfabGroupName);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

#if (UNITY_2018_3_OR_NEWER)
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {

        }
        else
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif

        mRtcEngine = IRtcEngine.GetEngine(appId);
        mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) =>
        {
            string joinSuccessMessage = string.Format("joinChannel callback uid: {0}, channel: {1}, version: {2}", uid, channelName, null);
            Debug.Log(joinSuccessMessage);
            my_uid = uid;
            AddRemoteStreans(my_uid); // add remote stream id to list of users
            UIDirector.Instance.RefeshUI(LayerMark.Party);
        };

        mRtcEngine.OnLeaveChannel += (RtcStats stats) =>
        {
            string leaveChannelMessage = string.Format("onLeaveChannel callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}", stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate);
            Debug.Log(leaveChannelMessage);
            RemoveRemoteStreams(my_uid); // add remote stream id to list of users
            playerInChannelList.Clear();
        };

        mRtcEngine.OnUserJoined += (uint uid, int elapsed) =>
        {
            AddRemoteStreans(uid); // add remote stream id to list of users
            //string userJoinedMessage = string.Format("onUserJoined callback uid {0} {1}", uid, elapsed);
            //Debug.Log(userJoinedMessage);
        };

        mRtcEngine.OnUserOffline += (uint uid, USER_OFFLINE_REASON reason) =>
        {
            string userOfflineMessage = string.Format("onUserOffline callback uid {0} {1}", uid, reason);
            Debug.Log(userOfflineMessage);
            RemoveRemoteStreams(uid);
        };

        mRtcEngine.OnVolumeIndication += (AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume) =>
        {
            if (speakerNumber == 0 || speakers == null)
            {
                Debug.Log(string.Format("onVolumeIndication only local {0}", totalVolume));
            }

            for (int idx = 0; idx < speakerNumber; idx++)
            {
                string volumeIndicationMessage = string.Format("{0} onVolumeIndication {1} {2}", speakerNumber, speakers[idx].uid, speakers[idx].volume);
                Debug.Log(volumeIndicationMessage);
            }
        };

        mRtcEngine.OnUserMutedAudio += (uint uid, bool muted) =>
        {
            string userMutedMessage = string.Format("onUserMuted callback uid {0} {1}", uid, muted);
            Debug.Log(userMutedMessage);
        };

        mRtcEngine.OnWarning += (int warn, string msg) =>
        {
            string description = IRtcEngine.GetErrorDescription(warn);
            string warningMessage = string.Format("onWarning callback {0} {1} {2}", warn, msg, description);
            Debug.Log(warningMessage);
        };

        mRtcEngine.OnError += (int error, string msg) =>
        {
            string description = IRtcEngine.GetErrorDescription(error);
            string errorMessage = string.Format("onError callback {0} {1} {2}", error, msg, description);
            Debug.Log(errorMessage);
        };

        mRtcEngine.OnRtcStats += (RtcStats stats) =>
        {
            string rtcStatsMessage = string.Format("onRtcStats callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}, tx(a) kbps: {5}, rx(a) kbps: {6} users {7}",
                stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate, stats.txAudioKBitRate, stats.rxAudioKBitRate, stats.userCount);
            Debug.Log(rtcStatsMessage);

            int lengthOfMixingFile = mRtcEngine.GetAudioMixingDuration();
            int currentTs = mRtcEngine.GetAudioMixingCurrentPosition();

            string mixingMessage = string.Format("Mixing File Meta {0}, {1}", lengthOfMixingFile, currentTs);
            Debug.Log(mixingMessage);
        };

        mRtcEngine.OnAudioRouteChanged += (AUDIO_ROUTE route) =>
        {
            string routeMessage = string.Format("onAudioRouteChanged {0}", route);
            Debug.Log(routeMessage);
        };

        mRtcEngine.OnRequestToken += () =>
        {
            string requestKeyMessage = string.Format("OnRequestToken");
            Debug.Log(requestKeyMessage);
        };

        mRtcEngine.OnConnectionInterrupted += () =>
        {
            string interruptedMessage = string.Format("OnConnectionInterrupted");
            Debug.Log(interruptedMessage);
        };

        mRtcEngine.OnConnectionLost += () =>
        {
            string lostMessage = string.Format("OnConnectionLost");
            Debug.Log(lostMessage);
        };

        mRtcEngine.SetLogFilter(LOG_FILTER.INFO);
        mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);
    }

    // 退出一个频道的逻辑是：先从playfab退，再从agora的频道退
    public void LeaveChannel()
    {
        mRtcEngine.LeaveChannel();
    }

    void OnApplicationQuit()
    {
        if (mRtcEngine != null)
        {
            // 这个比想象中的要重要。没有这个会导致很多诡异问题。
            IRtcEngine.Destroy();
        }
    }
}
