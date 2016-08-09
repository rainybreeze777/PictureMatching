using System;
using System.Collections.Generic;

public interface IBoardModel {
	int numOfRows();
	int numOfColumns();
	int GetTileAt(int row, int column);
	void GenerateBoard();
	bool isRemovable(int r1, int c1, int r2, int c2);
	void remove(int r, int c);
	void remove(int r1, int c1, int r2, int c2);
	void removeColumn(int col);
    void removeRange(int startRow, int endRow, int startCol, int endCol);
	bool isEmpty();
}
