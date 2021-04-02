using UnityEngine;
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
    private IRtcEngine mRtcEngine = null;

    public IRtcEngine GetIRtcEngine()
    {
        return mRtcEngine;
    }

    // Start is called before the first frame update
    void Start()
    {

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
        };

        mRtcEngine.OnLeaveChannel += (RtcStats stats) =>
        {
            string leaveChannelMessage = string.Format("onLeaveChannel callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}", stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate);
            Debug.Log(leaveChannelMessage);
        };

        mRtcEngine.OnUserJoined += (uint uid, int elapsed) =>
        {
            string userJoinedMessage = string.Format("onUserJoined callback uid {0} {1}", uid, elapsed);
            Debug.Log(userJoinedMessage);
        };

        mRtcEngine.OnUserOffline += (uint uid, USER_OFFLINE_REASON reason) =>
        {
            string userOfflineMessage = string.Format("onUserOffline callback uid {0} {1}", uid, reason);
            Debug.Log(userOfflineMessage);
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

    void OnApplicationQuit()
    {
        if (mRtcEngine != null)
        {
            // 这个比想象中的要重要。没有这个会导致很多诡异问题。
            IRtcEngine.Destroy();
        }
    }
}
