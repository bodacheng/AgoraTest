using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// SingleMode => StartParty => PartySetting => HostingParty
/// SingleMode => RandomJoin => Party
/// </summary>

public class UIDirector : MonoBehaviour
{
    public PartySettingManger partySettingInput;
    public RectTransform BasicT, PartySetting, ConnectionMode_Host;

    public Button HoldAParty;
    public Button JoinAParty;
    public Button StopParty;

    public static UIDirector Instance;

    async void Start()
    {
        Instance = this;

        // Button Feature
        HoldAParty.onClick.AddListener(()=> {
            GotoHoldPartySetting();
        });

        JoinAParty.onClick.AddListener(() => {
            RandomJoinParty();
        });

        StopParty.onClick.AddListener(() => {
            PlayFabHander.Quit();
        });

        LoginLayer loginLayer = new LoginLayer();
        LobbyLayer singleModeLayer = new LobbyLayer();
        PartySettingLayer partySettingLayer = new PartySettingLayer();
        PartyLayer holdingPartyLayer = new PartyLayer();

        LayerRunner.Main.AddNewLayer(LayerMark.Login, loginLayer);
        LayerRunner.Main.AddNewLayer(LayerMark.Lobby, singleModeLayer);
        LayerRunner.Main.AddNewLayer(LayerMark.PartySetting, partySettingLayer);
        LayerRunner.Main.AddNewLayer(LayerMark.Party, holdingPartyLayer);

        // Add basic component for all layers
        foreach (KeyValuePair<LayerMark, UILayer> keyValuePair in LayerRunner.Main.layerDic)
        {
            keyValuePair.Value.uiDirector = this;
        }

        await LayerRunner.Main.ChangeProcess(LayerMark.Login);
    }

    async void GotoHoldPartySetting()
    {
        await LayerRunner.Main.ChangeProcess(LayerMark.PartySetting);
    }

    async void RandomJoinParty()
    {
        await LayerRunner.Main.ChangeProcess(LayerMark.Party);
    }

    public void RefeshUI(LayerMark layerMark)
    {
        switch (layerMark)
        {
            case LayerMark.Lobby:
                BasicT.gameObject.SetActive(true);
                PartySetting.gameObject.SetActive(false);
                ConnectionMode_Host.gameObject.SetActive(false);
                break;
            case LayerMark.PartySetting:
                BasicT.gameObject.SetActive(false);
                PartySetting.gameObject.SetActive(true);
                ConnectionMode_Host.gameObject.SetActive(false);
                break;
            case LayerMark.Party:
                BasicT.gameObject.SetActive(false);
                PartySetting.gameObject.SetActive(false);
                ConnectionMode_Host.gameObject.SetActive(true);
                break;
        }
    }

    public void RefreshPartyState(int state)
    {
        switch (state)
        {
            case -1: // 自身正在举行party 
                HoldAParty.gameObject.SetActive(false);
                JoinAParty.gameObject.SetActive(false);
                StopParty.gameObject.SetActive(true);
                break;
            case 0: // 没有在party中
                HoldAParty.gameObject.SetActive(true);
                JoinAParty.gameObject.SetActive(true);
                StopParty.gameObject.SetActive(false);
                break;
            case 1: // 正在参加别人的party 
                break;
        }
    }
}
