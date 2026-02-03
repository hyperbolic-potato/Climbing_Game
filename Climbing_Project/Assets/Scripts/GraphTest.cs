using UnityEngine;
using System.Collections.Generic;

public class GraphTest : MonoBehaviour
{
    void Start()
    {
        GraphSystem<int> graph = new GraphSystem<int>();

        for(int i = 0; i < 7; i++)
        {
            graph.AddNode(i);
        }

        int[,] edges = { { 0, 1 }, { 0, 2 }, { 1, 3 }, { 2, 4 }, { 3, 4 }, { 3, 5 }, { 4, 6 }, { 5, 6 } };

        for (int i = 0; i < edges.GetLength(0); i++)
        {
            graph.ConnectNodes(graph.FindNode(edges[i, 0]), graph.FindNode(edges[i, 1]));
        }

        List<Node<int>> path = graph.BFSearch(graph.FindNode(0), graph.FindNode(6));
        string pathstr = ""; 
        foreach (Node<int> node in path)
        {
            pathstr += node.value.ToString() + " -> ";
        }
        Debug.Log("EXPECTED OUTPUT: 0 -> 2 -> 4 -> 6 -> ");
        Debug.Log("OBSERVED OUTPUT: " + pathstr);
    }

}
