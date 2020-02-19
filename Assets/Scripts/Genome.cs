﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public class Genome
{
    List<ConnectionGene> connections = new List<ConnectionGene>();
    List<NodeGene> nodes = new List<NodeGene>();
    //List of nodes
    List<List<int>> connectionNodeIDs = new List<List<int>>();

    /* innovation is an ID assigned to each unique connection
     * We keep a list of connections so that a preexisting connection is assigned a preexisting innovation number
     * Once we get the index of the preexisting connection, we match it to this array
     * I am sure there is a better way to do this
    */
    public static int innovation = 0;
    // Adjacency matrix with relevant innovation number at existing connection index
    // [input node][output node] = innovation number
    public static List<List<int?>> adjacency = new List<List<int?>>();

    // Another adjacency matrix for local connections
    List<List<double?>> localAdjacency = new List<List<double?>>();

    public Genome(int inputSize, int outputSize, List<List<int>> connections) {
        // Add input nodes
        for (int i = 0; i < inputSize; i ++) {
            this.nodes.Add(new NodeGene(0, nodes.Count));
        }
        // Add output nodes
        for (int i = 0; i < outputSize; i ++) {
            this.nodes.Add(new NodeGene(2, nodes.Count));
        }
        // Add initial connections
        for (int i = 0; i < connections.Count; i ++) {
            addConnection(connections[i][0], connections[i][1], math.GetRandomDouble(-1, 1));
        }
    }

    // Update adjacency arrays and return innovation number
    private int? updateAdjacencies(int input, int output) {
        return adjacency[input][output] == null ? innovation++ : adjacency[input][output];
    }
    // Check if connection is possible
    private bool checkValid(int input, int output) {
        // If connection or reversed connection exists already: return false
        if (localAdjacency[input][output] != null || localAdjacency[output][input] != null) {
            return false;
        }
        bool[] nodeIsBacktracing = new bool[localAdjacency.Count()];
        localAdjacency[input][output] = 0;
        for (int i = 0; i < localAdjacency.Count(); i ++) {
            return backtrace(i);
        }
    }
    private bool backtrace(int node) {
        for (int i = 0; i < localAdjacency[node].Count(); i ++) {
            if (localAdjacency[node][i] == 0) {
                backtrace(i);
            }
            if (localAdjacency[node][i] == 1) {
                return false;
            }
        }
    }

    //feedforward the network
    List<double> calculate() {
        for (int i = 0; i < nodes.Count(); i ++) {
            if (!nodes[i].done) {
                nodes[i].getOutput();
            }
        }
    }

    // Mutations
    // Add new connection between nodes
    public bool addConnection(int input, int output, double weight) {
        if (!connectionExistsLocal(input, output)) {
            connections.Add(new ConnectionGene(input, output, weight, updateAdjacencies(input, output)));
            connectionNodeIDs.Add(new List<int> {input, output});
            return true;
        }
        return false;
    }
    public void pruneConnection(int input, int output) {

    }

    // Split existing connection, adding a hidden node in the middle
    // Incoming weight is set to 1, outgoing is the same as the previous weight
    private void addNode(ConnectionGene connection) {
        double oldWeight = connection.Split();
        int[] oldConnections = connection.getNodes();
        nodes.Add(new NodeGene(1, nodes.Count));
        addConnection(oldConnections[0], nodes.Count-1, 1);
        addConnection(nodes.Count-1, oldConnections[1], oldWeight);
    }
}