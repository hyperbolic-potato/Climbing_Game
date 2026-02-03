using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphSystem<T>
{
    //every node in the graph. note that not all nodes are necessarily connected to one another
    public List<Node<T>> graph;


    public GraphSystem()
    {
        graph = new List<Node<T>>();
    }

    //basic operations, adding and removing nodes to the graph

    public void AddNode(Node<T> node)
    {
        graph.Add(node);
        //literally as simple as it gets
    }

    public void AddNode( T value) //node agnostic overload
    {
        graph.Add(new Node<T>(value));
    }

    public Node<T> FindNode(T value)
    {
        foreach (Node<T> n in graph)
        {
            if (n.value.Equals(value)) return n;
        }
        return null;
    }

    public void RemoveNode(Node<T> node)
    {
        graph.Remove(node);

        foreach (Node<T> n in graph)
        {
            n.connections.Remove(node);
        }
        
    }

    public void RemoveNode(T value) //node agnostic overload
    {
        Node<T> node = this.FindNode(value);

        RemoveNode(node);
    }

    //slightly more complex operation, connecting two nodes with an edge. supports uni- or bi- directionality.
    //Also technically supports degenerate (self referencing) edges but... why?

    public void ConnectNodes(Node<T> from, Node<T> to, bool isDirected = false)
    {
        from.connections.Add(to);

        if(!isDirected) to.connections.Add(from);
    }

    public void ConnectNodes(T from, T to, bool isDirected = false) //overload
    {
        Node<T> fromNode = this.FindNode(from);
        Node<T> toNode = this.FindNode(to);

        ConnectNodes(fromNode, toNode, isDirected);
        
    }



    public void DisconnectNodes(Node<T> from, Node<T> to, bool isDirected = false)
    {
        from.connections.Remove(to);

        if (!isDirected) to.connections.Remove(from);
    }

    public void DisconnectNodes(T from, T to, bool isDirected = false)
    {
        Node<T> fromNode = this.FindNode(from);
        Node<T> toNode = this.FindNode(to);

        DisconnectNodes(fromNode, toNode, isDirected);
    }

    //searches. This is where the graphing gets really graphy, so get your programmer thigh highs nice and tight were goin on a ride.


    public List<Node<T>> BFSearch(Node<T> origin, Node<T> target)
    {

        //breadth first search, e. g. search all sibling nodes before moving on to the next child nodes.
        //this one inherently returns the shortest path on an UNWEIGHTED GRAPH.

        Queue<Node<T>> q = new Queue<Node<T>>(); //the queue of nodes to be searched

        List<Node<T>> searched = new List<Node<T>>(); //a blacklist of nodes that keeps them from being searched more than once

        List<Node<T>> parents = new List<Node<T>>(); //a parallel array of searched for use in finding a path from the origin to the target

        List<Node<T>> path = new List<Node<T>>(); //the path to be returned

        //prime the search with the origin node
        searched.Add(origin);
        parents.Add(origin);
        q.Enqueue(origin);

        while( q.Count > 0) //while there's still nodes in the queue...
        {
            Node<T> head = q.Dequeue(); //get the next in line

            if(head == target)
            {
                //if we've found the target, trace a path from it back to the origin
                while (!path.Contains(origin))
                {
                    path.Add(head);
                    head = parents[searched.IndexOf(head)];
                }

                path.Reverse();

                return path;
            }

            foreach(Node<T> edge in head.connections)
            {
                //for every node in the that has an edge with the head node, add it to the queue and save a reference to its parent for later
                if (!searched.Contains(edge))
                {
                    searched.Add(edge);
                    parents.Add(head);
                    q.Enqueue(edge);
                }
            }
        }
        //If you've burned through the entire graph and no path exists from origin to target
        return null; // better luck next time :(
    }

    public List<Node<T>> BFSearch(T origin, T target)
    {
        Node<T> originNode = this.FindNode(origin);
        Node<T> targetNode = this.FindNode(origin);

        return BFSearch(originNode, targetNode);
    }

}

public class Node<T>
{
    //each node of the graph has a list of connections and an associated value of type <T>.
    public T value;
    public List<Node<T>> connections;

    public Node(T value)
    {
        this.value = value;
        this.connections = new List<Node<T>>();
    }

    public override string ToString()
    {
        return $"n[{this.value}]";
    }
}
