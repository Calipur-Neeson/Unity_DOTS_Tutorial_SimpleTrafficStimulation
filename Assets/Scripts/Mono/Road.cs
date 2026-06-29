using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    [Header("Connectors")]
    public Transform[] Connectors;
    
    [Header("Adjacent Roads")]
    public List<Road> AdjacentRoads;
    
    [Header("Lanes on this road")]
    public Lane[] Lanes;
    
    private void OnValidate()
    {
        Lanes = GetComponentsInChildren<Lane>();
    }
}
