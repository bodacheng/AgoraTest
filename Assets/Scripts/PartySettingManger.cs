using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 这个模块你可以理解成，它就是针对那个开催party时候的设置页面的。
/// </summary>
public class PartySettingManger : MonoBehaviour
{
    public InputField mChannelNameInputField;
    public Button partyStartBtn;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        partyStartBtn.onClick.AddListener(StartNewChannel);
    }

    async void StartNewChannel()
    {
        string channelName = mChannelNameInputField.text.Trim();
        Debug.Log(string.Format("tap joinChannel with channel name {0}", channelName));
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        // channelName 是我们输入的昵称
        await LayerRunner.Main.ChangeProcess(LayerMark.Party, channelName);
    }
}
