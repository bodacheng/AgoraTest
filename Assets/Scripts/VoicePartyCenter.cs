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

        mRtcEngine.SetLogFilter(LOG_FILTER.INFO);
        mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);
    }
}
