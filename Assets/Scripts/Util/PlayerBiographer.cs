using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class is a Tree, and is used to record the player's progress in the game
// Currently records places player have visited, and to people player have talked
// if the player have done so, the id of the scene/nested scene and characterId
// will be present under the appropriate parent node
[Serializable]
public class PlayerBiographer : IBiographer {

    // Root should corresponds to player being in the map, and should have an id of 0
    // On the first level, all PointNode ids must be > 0, and they corresponds
    // to gameScene ids as listed in the stage texts json files
    // Nested levels follow a different rule as specified in PointNode
    [SerializeField] private Stack<PointNode> playerTrail;

    public void Visit(int destId) {
        playerTrail.Push(playerTrail.Peek().AddChildPoint(destId));
    }

    public bool AlreadyVisitedFromCurrentPoint(int destId) {
        return playerTrail.Peek().HasChildPoint(destId);
    }

    public void Leave() {
        // Do NOT pop the root node! or all data gets lost!
        if (playerTrail.Count > 1) {
            playerTrail.Pop();
        }
    }

    public void InitFromGameSave(GameSave save) {
        PlayerBiographer saveBiographer = save.PlayerBio;
        playerTrail = saveBiographer.playerTrail;
    }

    public bool IsAtMap() {
        return playerTrail.Count == 1 && playerTrail.Peek().Id == 0;
    }

    [PostConstruct]
    public void PostConstruct() {
        if (playerTrail == null) {
            playerTrail = new Stack<PointNode>();
            playerTrail.Push(new PointNode(0));
        }
    }

    [Serializable]
    private class PointNode {
        // The first level follows a different rule, as specified in PlayerBiography
        // In nested PointNode, a < 0 id indicates a nested scene id, and > 0
        // id corresponds to a characterId in the game
        [SerializeField] private int id; 
        public int Id { get { return id; } }
        [SerializeField] private SortedList<int, PointNode> nestedPoints = null;

        public PointNode(int id) {
            this.id = id;
        }

        public bool HasChildPoint(int id) {
            if (nestedPoints == null || nestedPoints.Count == 0) { return false; }

            return nestedPoints.ContainsKey(id);
        }

        public PointNode GetChild(int id) {
            return nestedPoints == null ? null : nestedPoints[id];
        }

        public PointNode AddChildPoint(int id) {
            if (nestedPoints == null) {
                nestedPoints = new SortedList<int, PointNode>();
            }

            if (!nestedPoints.ContainsKey(id)) {
                PointNode newPointNode = new PointNode(id); 
                nestedPoints.Add(id, newPointNode);
                return newPointNode;
            } 

            // Has id as child already, return existing child node
            return nestedPoints[id];
        }
    }
}
