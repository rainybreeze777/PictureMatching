using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboTree {

    private static ComboTree instance = null;

    private ComboTreeNode rootNode;

    private ComboListFetcher comboListFetcher;

    private ComboTree () {

        rootNode = new ComboTreeNode(-1, true);
        comboListFetcher = ComboListFetcher.GetInstance();

        foreach(KeyValuePair<int, List<int>> aCombo in comboListFetcher.GetList()) {
            
            AddCombo(aCombo.Value, aCombo.Key);
        }
    }

    public static ComboTree GetInstance() {
        if (instance == null) {
            instance = new ComboTree();
        }

        return instance;
    }

    public void AddCombo(List<int> comboSequence, int comboId) {

        if (comboSequence.Count < 2) {
            throw new System.ArgumentException("combo length must be larger than 1");
        }

        ComboTreeNode currentNode = rootNode;

        foreach (int tile in comboSequence) {
            ComboTreeNode checkNode = currentNode.GetChild(tile);
            if (checkNode != null) {
                currentNode = checkNode;
            } else {
                ComboTreeNode newNode = new ComboTreeNode(tile);
                currentNode.AddChildren(newNode);
                currentNode = newNode;
            }
        }

        //At this point, currentNode should point to the last combo
        currentNode.FormCombo(comboId);
    }

    //Will return the id of the combo if it exists
    //-1 if not
    public int GetComboId(List<int> comboSequence) {
        ComboTreeNode currentNode = rootNode;
        foreach (int tile in comboSequence) {
            ComboTreeNode tempNode = currentNode.GetChild(tile);
            if (tempNode == null) {
                return -1;
            }
            currentNode = tempNode;
        }

        if (currentNode.IsACombo) {
            return currentNode.ComboId;
        } else {
            return -1;
        }
    }

    private class ComboTreeNode {

        private List<ComboTreeNode> children = new List<ComboTreeNode>();

        private int tileNumber;
        //Boolean to check whether from root to here
        //forms a combo
        private bool isACombo = false;
        //Associated Combo ID, should be -1 if this node is not a combo
        private int comboId = -1;
        //Use to indicate whether this node is root.
        private bool isRoot;

        public ComboTreeNode(int tileNumber, bool isRoot = false) {
            if (isRoot)
                tileNumber = -1;
            else
                this.tileNumber = tileNumber;
        }

        public int TileNumber { get { return tileNumber; } }
        public bool IsACombo { get { return isACombo; } }
        public int ComboId { get { return comboId; } }

        public void FormCombo (int comboId) {
            isACombo = true;
            this.comboId = comboId;
        }

        public void ClearCombo () {
            isACombo = false;
            comboId = -1;
        }

        public void AddChildren(ComboTreeNode child) {
            children.Add(child);
        }

        public ComboTreeNode GetChild(int tileNum) {
            ComboTreeNode theChild = null;

            foreach (ComboTreeNode child in children) {
                if (child.TileNumber == tileNum) {
                    theChild = child;
                    break;
                }
            }

            return theChild;
        }
    }
}
