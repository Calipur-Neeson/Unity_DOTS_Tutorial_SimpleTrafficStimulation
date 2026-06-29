using UnityEngine;

public class Lane : MonoBehaviour
{
    [Header("Way points")]
    public Transform[] Waypoints;
    
    [Header("Next Lane")]
    public Lane NextLane;

    
    private Road[] _roads;
    private void OnValidate()
    {
        Waypoints = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            Waypoints[i] = transform.GetChild(i);
        }
    }

}
