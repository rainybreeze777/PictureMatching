using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboTree {

	private static ComboTree instance = null;

	private ComboNode rootNode;

	private ComboTree () {
		rootNode = new ComboNode(-1, true);
	}

	public ComboTree GetInstance() {
		if (instance == null) {
			instance = new ComboTree();
		}

		return instance;
	}

	public void AddCombo(List<int> comboSequence, string name) {

		if (comboSequence.Count < 2) {
			throw new System.ArgumentException("combo length must be larger than 1");
		}

		ComboNode currentNode = rootNode;

		foreach (int tile in comboSequence) {
			ComboNode checkNode = currentNode.GetChild(tile);
			if (checkNode != null) {
				currentNode = checkNode;
			} else {
				ComboNode newNode = new ComboNode(tile);
				currentNode.AddChildren(newNode);
				currentNode = newNode;
			}
		}

		//At this point, currentNode should point to the last combo
		currentNode.FormCombo(name);
	}

	//Will return the name of the combo if it exists
	//Empty String if not
	public string GetCombo(List<int> comboSequence) {
		ComboNode currentNode = rootNode;
		foreach (int tile in comboSequence) {
			ComboNode tempNode = currentNode.GetChild(tile);
			if (tempNode == null) {
				return "";
			}
			currentNode = tempNode;
		}

		if (currentNode.IsACombo) {
			return currentNode.ComboName;
		} else {
			return "";
		}
	}

	private class ComboNode {

		private List<ComboNode> children = new List<ComboNode>();

		private int tileNumber;
		//Boolean to check whether from root to here
		//forms a combo
		private bool isACombo = false;
		//comboName should be empty if this is not a combo
		private string comboName = "";
		//Use to indicate whether this node is root.
		private bool isRoot;

		public ComboNode(int tileNumber, bool isRoot = false) {
			if (isRoot)
				tileNumber = -1;
			else
				this.tileNumber = tileNumber;
		}

		public int TileNumber {
			get { return tileNumber; }
		}

		public bool IsACombo {
			get { return isACombo; }
		}

		public string ComboName {
			get { return comboName; }
		}

		public void FormCombo (string name) {
			isACombo = true;
			comboName = name;
		}

		public void ClearCombo () {
			isACombo = false;
			comboName = "";
		}

		public void AddChildren(ComboNode child) {
			children.Add(child);
		}

		public ComboNode GetChild(int tileNum) {
			ComboNode theChild = null;

			foreach (ComboNode child in children) {
				if (child.TileNumber == tileNum) {
					theChild = child;
					break;
				}
			}

			return theChild;
		}
	}
}
