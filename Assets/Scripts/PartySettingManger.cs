using UnityEngine;
using UnityEngine.UI;

public class PartySettingManger : MonoBehaviour
{
    public VoicePartyCenter voicePartyCenter;
    public Dropdown maxMemberDropDown;
    public InputField mChannelNameInputField;
    public Button holdPartyBtn;
    public Button joinPartyBtn;
    public Button partyStartBtn;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        holdPartyBtn.onClick.AddListener(StartToEditParty);
        joinPartyBtn.onClick.AddListener(RandomJoinParty);
        partyStartBtn.onClick.AddListener(StartChannel);
    }

    async void StartToEditParty()
    {
        await LayerRunner.Main.ChangeProcess(LayerMark.PartySetting);
    }

    async void StartChannel()
    {
        string channelName = mChannelNameInputField.text.Trim();
        Debug.Log(string.Format("tap joinChannel with channel name {0}", channelName));
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }
        await LayerRunner.Main.ChangeProcess(LayerMark.HoldingParty, channelName);
    }

    /// <summary>
    /// 无法理解如何获取对应app的房间列表，暂时以定值加入
    /// </summary>
    async void RandomJoinParty()
    {
        await LayerRunner.Main.ChangeProcess(LayerMark.HoldingParty);
    }

    public void LeaveChannel()
    {
        voicePartyCenter.GetIRtcEngine().LeaveChannel();
        string channelName = mChannelNameInputField.text.Trim();
        Debug.Log(string.Format("left channel name {0}", channelName));
    }
}
