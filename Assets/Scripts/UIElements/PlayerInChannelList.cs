using System.Collections.Generic;
using UnityEngine;

public class PlayerInChannelList : MonoBehaviour
{
    public PlayerInfoUI playIconPrefab;

    IDictionary<string, PlayerInfoUI> Dic = new Dictionary<string, PlayerInfoUI>();

    public void NewUserEnter(string userID)
    {
        PlayerInfoUI newIcon = GameObject.Instantiate(playIconPrefab);
        newIcon.userID.text = userID;
        newIcon.transform.SetParent(transform);
        Dic.Add(userID, newIcon);
    }

    public void UserQuit(string userID)
    {
        PlayerInfoUI targetIcon = Dic[userID];
        Dic.Remove(userID);
        if (targetIcon.gameObject != null)
            GameObject.Destroy(targetIcon.gameObject);
    }
}
