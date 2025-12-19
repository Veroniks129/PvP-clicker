using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Nodedata : NetworkBehaviour
{
    public List<GameObject> neighbors;
    public NetworkVariable<int> power = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Colors> color = new NetworkVariable<Colors>(Colors.Neutral,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public int minRandomValue = 5;
    public int maxRandomValue = 10;
    public bool isLarge = false;

    //private void Awake()
    //{
    //    int initWeight = Random.Range(minRandomValue, maxRandomValue + 1);
    //    power.Value = initWeight;
    //}

    public bool IsAnyNeighMy(Colors color)
    {
        foreach (GameObject neighbour in neighbors)
        {
            Nodedata nodedata = neighbour.GetComponent<Nodedata>();
            if (nodedata.color.Value == color) return true;
        }
        return false;
    }

    public string GetName()
    {
        GameObject nodeNameObj = transform.parent.Find("nodename").gameObject;
        TextMeshProUGUI namingMesh = nodeNameObj.GetComponent<TextMeshProUGUI>();
        return namingMesh.text;
    }
}
