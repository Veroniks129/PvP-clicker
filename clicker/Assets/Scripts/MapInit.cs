using Unity.Netcode;
using UnityEngine;

public class MapInit : MonoBehaviour
{
    [SerializeField] public GameObject map;

    public void InitializeMap()
    {
        for (int i = 0; i < map.transform.childCount; i++)
        {
            Transform node_parent = map.transform.GetChild(i);
            for (int j = 0; j < node_parent.transform.childCount; j++)
            {
                if (node_parent.name == "WinScore")
                {
                    continue;
                }

                Transform child = node_parent.transform.GetChild(j);
                if (child.name == "node") {
                    Nodedata nodedata = child.GetComponent<Nodedata>();
                    nodedata.color.Value = Colors.Neutral;
                    nodedata.power.Value = Random.Range(3, 11);
                    if (nodedata.isLarge)
                    {
                        nodedata.power.Value = Random.Range(25, 51);
                    }
                    if (node_parent.name == "Moscow")
                    {
                        nodedata.power.Value = 100;
                    }

                    if (node_parent.name == "Vyborg")
                    {
                        nodedata.color.Value = Colors.Blue;
                        nodedata.power.Value = 25;
                    }
                    if (node_parent.name == "Nizhniy-Novgorod")
                    {
                        nodedata.color.Value = Colors.Red;
                        nodedata.power.Value = 25;
                    }
                    if (node_parent.name == "Riga")
                    {
                        nodedata.color.Value = Colors.Yellow;
                        nodedata.power.Value = 25;
                    }
                    if (node_parent.name == "Kursk")
                    {
                        nodedata.color.Value = Colors.Green;
                        nodedata.power.Value = 25;
                    }
                }
            }
        }
    }
}
