using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class LayerRunner
{
    static LayerRunner instance_main;
    public static LayerRunner Main
    {
        get
        {
            if (instance_main == null)
            {
                instance_main = new LayerRunner();
            }
            return instance_main;
        }
    }

    public UILayer lastLayer;
    public UILayer currentLayer;
    public readonly IDictionary<LayerMark, UILayer> layerDic = new Dictionary<LayerMark, UILayer>();

    public void Clear()
    {
        layerDic.Clear();
    }

    public void AddNewLayer(LayerMark layerMark, UILayer layer)
    {
        if (!layerDic.ContainsKey(layerMark))
            layerDic.Add(layerMark, layer);
        else
            layerDic[layerMark] = layer;

        layer.SetLayerMark(layerMark);
    }

    public async UniTask ChangeProcess(LayerMark layerMark)
    {
        await ChangeProcess<string>(layerMark, null);
    }

    public async UniTask ChangeProcess<T>(LayerMark layerMark, T t)
    {
        if (currentLayer != null)
        {
            await currentLayer.LayerEnd();
        }

        lastLayer = currentLayer;
        layerDic.TryGetValue(layerMark, out currentLayer);
        if (currentLayer != null)
        {
            if (t != null)
            {
                await currentLayer.LayerEnter(t);
            }
            else
            {
                await currentLayer.LayerEnter();
            }
        }
        else
        {
            if (layerDic.ContainsKey(layerMark))
            {
                Debug.Log(layerMark + "in the dic");
                Debug.Log(currentLayer);
            }
            Debug.Log("this layer hasnt been defined：" + layerMark);
        }
    }
}