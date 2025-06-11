using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniMax
{
    public StateAI Root { get; set; }
    public StateAI Result { get; set; }
    public int MaxDepth { get; set; }

    public MiniMax(StateAI root, int maxDepth)
    {
        Root = root;
        MaxDepth = maxDepth;
    }

    public StateAI Search()
    {
        MiniMaxNode rootNode = new MiniMaxNode(0, Root);
        int alpha = int.MaxValue;
        int beta = int.MinValue;
        Result = MiniMaxSearch(rootNode, alpha, beta).state;
        return Result;
    }

    private MiniMaxNode MiniMaxSearch(MiniMaxNode node, int alpha, int beta)
    {
        if (node.state.IsTerminal() || node.depth == MaxDepth)
        {
            return node;
        }

        if (node.children.Count == 0)
        {
            List<List<MoveMemento>> childStates = node.state.unitManagerMemento.getChildStates(!node.state.enemyTurn);

            foreach (List<MoveMemento> childState in childStates)
            {
                var child = new StateAI(
                    node.state.unitManagerMemento,
                    node.state.gridManagerMemento,
                    childState,
                    !node.state.enemyTurn,
                    node.state.stateScore
                );

                node.children.Add(new MiniMaxNode(node.depth + 1, child));
            }
        }

        MiniMaxNode bestNode = node.children.ElementAt(0);
        int bestScore = node.children.ElementAt(0).state.stateScore;

        foreach (MiniMaxNode child in node.children)
        {
            MiniMaxNode childResult = MiniMaxSearch(child, alpha, beta);
            int childScore = childResult.state.stateScore;

            if (node.maximizingPlayer)
            {
                if (childScore > bestScore)
                {
                    bestScore = childScore;
                    bestNode = child;
                }
                alpha = Math.Max(alpha, bestScore);
            }
            else
            {
                if (childScore < bestScore)
                {
                    bestScore = childScore;
                    bestNode = child;
                }
                beta = Math.Min(beta, bestScore);
            }

            if(beta <= alpha)
            {
                break;
            }
        }

        return bestNode;
    }
}

public class MiniMaxNode
{
    public int depth;
    public StateAI state;
    public bool maximizingPlayer;
    public List<MiniMaxNode> children = new List<MiniMaxNode>();

    public MiniMaxNode(int depth, StateAI state)
    {
        this.depth = depth;
        this.state = state;
        this.maximizingPlayer = !state.enemyTurn;
    }
}
