using Unity.Netcode;
using UnityEngine;

public class MapInit : MonoBehaviour
{
    [SerializeField] Nodedata test;

    public void InitializeMap()
    {
        test.color.Value = Colors.Yellow;
        test.power.Value = 100;
    }
}
