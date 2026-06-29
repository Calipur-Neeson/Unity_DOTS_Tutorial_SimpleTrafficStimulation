using System.Linq;
using UnityEditor;
using UnityEngine;

public static class LaneTopology
{
    [MenuItem("Tools/Traffic/Build Road Network")]
    public static void Build()
    {
        Road[] allRoads = Object.FindObjectsByType<Road>(FindObjectsSortMode.None);
        
        foreach (var road in allRoads)
        {
            road.AdjacentRoads.Clear();
        }
        
        for (int i = 0; i < allRoads.Length; i++)
        {
            Road road = allRoads[i];

            for (int j = i + 1; j < allRoads.Length; j++)
            {
                Road other = allRoads[j];
                
                foreach (var connector in road.Connectors)
                {
                    foreach (var otherConnector in other.Connectors)
                    {
                        if (Vector3.Distance(
                                connector.position,
                                otherConnector.position) < 0.1f)
                        {
                            road.AdjacentRoads.Add(other);
                            other.AdjacentRoads.Add(road);
                            
                            break;
                        }
                    }
                }
            }
            EditorUtility.SetDirty(road);
        }
        AssetDatabase.SaveAssets();

        Debug.Log("Road Network Build Complete.");
    }
    

    [MenuItem("Tools/Traffic/Build Lane Network")]
    public static void BuildLane()
    {
        Road[] roads = Object.FindObjectsByType<Road>(FindObjectsSortMode.None);
        
        foreach (var road in roads)
        {
            foreach (var lane in road.Lanes)
            {
                lane.NextLane = null;
            }
        }
        
        foreach (var road in roads)
        {
            foreach (var otherRoad in road.AdjacentRoads)
            {
                if (otherRoad == null)
                    continue;

                foreach (var lane in road.Lanes)
                {
                    foreach (var nextLane in otherRoad.Lanes)
                    {
                        if (Vector3.Distance(
                                lane.Waypoints.Last().position,
                                nextLane.Waypoints[0].position) < 0.1f)
                        {
                            lane.NextLane = nextLane;
                            break;
                        }
                    }
                    EditorUtility.SetDirty(lane);
                }
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Lane Topology Build Finished.");
    }
}
