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

    public static UIDirector Instance;

    async void Start()
    {
        Instance = this;

        // Button Feature
        HoldAParty.onClick.AddListener(()=> {
            GotoHoldPartySetting();
        });

        SingleModeLayer singleModeLayer = new SingleModeLayer();
        PartySettingLayer partySettingLayer = new PartySettingLayer();
        HoldingPartyLayer holdingPartyLayer = new HoldingPartyLayer();
        LayerRunner.Main.AddNewLayer(LayerMark.Single, singleModeLayer);
        LayerRunner.Main.AddNewLayer(LayerMark.PartySetting, partySettingLayer);
        LayerRunner.Main.AddNewLayer(LayerMark.HoldingParty, holdingPartyLayer);

        // Add basic component for all layers
        foreach (KeyValuePair<LayerMark, UILayer> keyValuePair in LayerRunner.Main.layerDic)
        {
            keyValuePair.Value.uiDirector = this;
        }

        await LayerRunner.Main.ChangeProcess(LayerMark.Single);
    }

    async void GotoHoldPartySetting()
    {
        await LayerRunner.Main.ChangeProcess(LayerMark.PartySetting);
    }

    public void RefeshUI(LayerMark layerMark)
    {
        switch (layerMark)
        {
            case LayerMark.Single:
                BasicT.gameObject.SetActive(true);
                PartySetting.gameObject.SetActive(false);
                ConnectionMode_Host.gameObject.SetActive(false);
                break;
            case LayerMark.PartySetting:
                BasicT.gameObject.SetActive(false);
                PartySetting.gameObject.SetActive(true);
                ConnectionMode_Host.gameObject.SetActive(false);
                break;
            case LayerMark.HoldingParty:
                BasicT.gameObject.SetActive(false);
                PartySetting.gameObject.SetActive(false);
                ConnectionMode_Host.gameObject.SetActive(true);
                break;
        }
    }
}
