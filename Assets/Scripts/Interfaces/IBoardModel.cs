using System;
using System.Collections.Generic;

public interface IBoardModel {
	int numOfRows();
	int numOfColumns();
	int GetTileAt(int row, int column);
	void GenerateBoard();
	bool isRemovable(int r1, int c1, int r2, int c2);
	bool remove(int r1, int c1, int r2, int c2);
	bool isEmpty();
}
