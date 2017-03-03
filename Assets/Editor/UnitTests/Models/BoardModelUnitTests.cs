using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;

namespace BoardModelUnitTests {

    [TestFixture]
    internal class BoardModelUnitTests {

        private BoardModel boardModel = null;

        private int[,] stage1GameBoard = new int[,]{
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 5, 5, 5, 5, 5, 5, 0},
            {0, 5, 1, 1, 4, 5, 5, 0},
            {0, 5, 2, 3, 0, 3, 5, 0},
            {0, 5, 2, 0, 4, 5, 5, 0},
            {0, 5, 5, 5, 5, 5, 5, 0},
            {0, 0, 0, 0, 0, 0, 0, 0}};
        private int[,] stage2GameBoardType1 = new int[,]{
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 5, 5, 5, 5, 5, 5, 0},
            {0, 5, 1, 5, 0, 2, 5, 0},
            {0, 5, 0, 1, 2, 3, 5, 0},
            {0, 5, 5, 3, 0, 0, 4, 0},
            {0, 5, 5, 5, 4, 5, 5, 0},
            {0, 0, 0, 0, 0, 0, 0, 0}};
        private int[,] stage2GameBoardType2 = new int[,]{
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 5, 5, 5, 5, 5, 5, 0},
            {0, 5, 1, 0, 5, 2, 5, 0},
            {0, 5, 3, 1, 2, 0, 5, 0},
            {0, 4, 0, 0, 3, 5, 5, 0},
            {0, 5, 5, 4, 5, 5, 5, 0},
            {0, 0, 0, 0, 0, 0, 0, 0}};
        private int[,] stage3GameBoardType1 = new int[,]{
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 5, 5, 5, 5, 5, 5, 0},
            {0, 5, 1, 2, 3, 0, 4, 0},
            {0, 5, 0, 0, 4, 0, 3, 0},
            {0, 5, 2, 1, 5, 5, 5, 0},
            {0, 5, 5, 5, 5, 5, 5, 0},
            {0, 0, 0, 0, 0, 0, 0, 0}};
        private int[,] stage3GameBoardType2 = new int[,]{
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 5, 1, 5, 1, 5, 5, 0},
            {0, 4, 5, 5, 5, 5, 2, 0},
            {0, 5, 5, 5, 5, 5, 5, 0},
            {0, 4, 5, 5, 5, 5, 2, 0},
            {0, 5, 3, 5, 3, 5, 5, 0},
            {0, 0, 0, 0, 0, 0, 0, 0}};
        private int[,] filledGameBoard = new int[,]{
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 5, 1, 5, 4, 3, 5, 0},
            {0, 1, 4, 1, 2, 5, 3, 0},
            {0, 3, 1, 2, 5, 2, 5, 0},
            {0, 5, 5, 4, 2, 5, 5, 0},
            {0, 5, 3, 5, 5, 4, 5, 0},
            {0, 0, 0, 0, 0, 0, 0, 0}};

        [SetUp]
        public void Init() {
            boardModel = new BoardModel();
            var mockBattleSignal = Substitute.For<BattleResultUpdatedSignal>();
            boardModel.battleResultUpdatedSignal = mockBattleSignal;
            var mockNewBoardSignal = Substitute.For<NewBoardConstructedSignal>();
            boardModel.newBoardConstructedSignal = mockNewBoardSignal;
            var mockTileDestroyedSignal = Substitute.For<TileDestroyedSignal>();
            boardModel.tileDestroyedSignal = mockTileDestroyedSignal;
            var mockTileRangeDestroyedSignal = Substitute.For<TileRangeDestroyedSignal>();
            boardModel.tileRangeDestroyedSignal = mockTileRangeDestroyedSignal;
            var mockPlayerStatus = Substitute.For<IPlayerStatus>();
            mockPlayerStatus.GetEquippedWeapons().Returns(new List<Weapon>());
            boardModel.playerStatus = mockPlayerStatus;
            boardModel.PostConstruct();
        }

        // Make sure Row count is 7
        [Test]
        public void BoardRowCountTest() {
            Assert.That(boardModel.numOfRows() == 7);
        }

        // Make sure Column count is 8
        [Test]
        public void BoardColumnCountTest() {
            Assert.That(boardModel.numOfColumns() == 8);
        }

        // isRemovable() Tests
        [Test]
        public void Stage1IsRemovableTest() {
            boardModel.UnitTesting_SetBoard(stage1GameBoard, 14);
            // Horizontal neighbour
            Assert.That(boardModel.isRemovable(2, 2, 2, 3));
            Assert.That(boardModel.isRemovable(2, 3, 2, 2));
            // Vertical neightbour
            Assert.That(boardModel.isRemovable(3, 2, 4, 2));
            Assert.That(boardModel.isRemovable(4, 2, 3, 2));
            // Horizontal gapped
            Assert.That(boardModel.isRemovable(3, 3, 3, 5));
            Assert.That(boardModel.isRemovable(3, 5, 3, 3));
            // Vertical gapped
            Assert.That(boardModel.isRemovable(2, 4, 4, 4));
            Assert.That(boardModel.isRemovable(4, 4, 2, 4));
        }

        [Test]
        public void Stage2IsRemovableTest() {
            boardModel.UnitTesting_SetBoard(stage2GameBoardType1, 13);
            // |_
            Assert.That(boardModel.isRemovable(2, 2, 3, 3));
            Assert.That(boardModel.isRemovable(3, 3, 2, 2));
            // |-
            Assert.That(boardModel.isRemovable(3, 4, 2, 5));
            Assert.That(boardModel.isRemovable(2, 5, 3, 4));
            // __|
            Assert.That(boardModel.isRemovable(4, 3, 3, 5));
            Assert.That(boardModel.isRemovable(3, 5, 4, 3));
            // --|
            Assert.That(boardModel.isRemovable(5, 4, 4, 6));
            Assert.That(boardModel.isRemovable(4, 6, 5, 4));

            boardModel.UnitTesting_SetBoard(stage2GameBoardType2, 13);
            // -|
            Assert.That(boardModel.isRemovable(2, 2, 3, 3));
            Assert.That(boardModel.isRemovable(3, 3, 2, 2));
            // _|
            Assert.That(boardModel.isRemovable(3, 4, 2, 5));
            Assert.That(boardModel.isRemovable(2, 5, 3, 4));
            // |__
            Assert.That(boardModel.isRemovable(3, 2, 4, 4));
            Assert.That(boardModel.isRemovable(4, 4, 3, 2));
            // |--
            Assert.That(boardModel.isRemovable(4, 1, 5, 3));
            Assert.That(boardModel.isRemovable(5, 3, 4, 1));
        }

        [Test]
        public void Stage3IsRemovableTest() {
            boardModel.UnitTesting_SetBoard(stage3GameBoardType1, 13);
            //  |
            //  --
            //   |
            Assert.That(boardModel.isRemovable(2, 2, 4, 3));
            Assert.That(boardModel.isRemovable(4, 3, 2, 2));
            //    |
            //  --
            // |
            Assert.That(boardModel.isRemovable(4, 2, 2, 3));
            Assert.That(boardModel.isRemovable(2, 3, 4, 2));
            // -|
            //  |-
            Assert.That(boardModel.isRemovable(2, 4, 3, 6));
            Assert.That(boardModel.isRemovable(3, 6, 2, 4));
            //  |-
            // -|
            Assert.That(boardModel.isRemovable(3, 4, 2, 6));
            Assert.That(boardModel.isRemovable(2, 6, 3, 4));

            boardModel.UnitTesting_SetBoard(stage3GameBoardType2, 15);
            // ---
            // | |
            Assert.That(boardModel.isRemovable(1, 2, 1, 4));
            Assert.That(boardModel.isRemovable(1, 4, 1, 2));
            // -|
            // -|
            Assert.That(boardModel.isRemovable(2, 6, 4, 6));
            Assert.That(boardModel.isRemovable(4, 6, 2, 6));
            // | |
            // ---
            Assert.That(boardModel.isRemovable(5, 2, 5, 4));
            Assert.That(boardModel.isRemovable(5, 4, 5, 2));
            // |-
            // |-
            Assert.That(boardModel.isRemovable(2, 1, 4, 1));
            Assert.That(boardModel.isRemovable(4, 1, 2, 1));
        }

        [Test]
        public void UnremovableTest() {
            boardModel.UnitTesting_SetBoard(filledGameBoard, 15);

            Assert.That(!boardModel.isRemovable(2, 1, 2, 3));
            Assert.That(!boardModel.isRemovable(2, 3, 2, 1));
            Assert.That(!boardModel.isRemovable(1, 2, 3, 2));
            Assert.That(!boardModel.isRemovable(3, 2, 1, 2));

            Assert.That(!boardModel.isRemovable(3, 3, 2, 4));
            Assert.That(!boardModel.isRemovable(2, 4, 3, 3));
            Assert.That(!boardModel.isRemovable(2, 4, 3, 5));
            Assert.That(!boardModel.isRemovable(3, 5, 2, 4));
            Assert.That(!boardModel.isRemovable(3, 5, 4, 4));
            Assert.That(!boardModel.isRemovable(4, 4, 3, 5));
            Assert.That(!boardModel.isRemovable(4, 4, 3, 3));
            Assert.That(!boardModel.isRemovable(3, 3, 4, 4));

            Assert.That(!boardModel.isRemovable(1, 5, 2, 6));
            Assert.That(!boardModel.isRemovable(2, 6, 1, 5));
            Assert.That(!boardModel.isRemovable(2, 6, 5, 2));
            Assert.That(!boardModel.isRemovable(5, 2, 2, 6));
            Assert.That(!boardModel.isRemovable(5, 2, 3, 1));
            Assert.That(!boardModel.isRemovable(3, 1, 5, 2));
            Assert.That(!boardModel.isRemovable(3, 1, 1, 5));
            Assert.That(!boardModel.isRemovable(1, 5, 3, 1));

            Assert.That(!boardModel.isRemovable(2, 2, 1, 4));
            Assert.That(!boardModel.isRemovable(1, 4, 2, 2));
            Assert.That(!boardModel.isRemovable(4, 3, 5, 5));
            Assert.That(!boardModel.isRemovable(5, 5, 4, 3));
            Assert.That(!boardModel.isRemovable(2, 2, 4, 3));
            Assert.That(!boardModel.isRemovable(4, 3, 2, 2));
            Assert.That(!boardModel.isRemovable(4, 3, 1, 4));
            Assert.That(!boardModel.isRemovable(1, 4, 4, 3));
        }

        // GetTileAt() tests
        [Test]
        public void GetTileAtTest() {
            boardModel.UnitTesting_SetBoard(filledGameBoard, 15);
            for (int r = 1; r < boardModel.numOfRows() - 1; ++r) {
                for (int c = 1; c < boardModel.numOfColumns() - 1; ++c) {
                    Assert.That(boardModel.GetTileAt(r, c) == filledGameBoard[r, c]);
                }
            }
        }

        // GenerateBoard() tests
        [Test]
        public void GenerateBoardTest() {
            boardModel.GenerateBoard();
            int[] tilesCount = new int[6];
            int[,] generatedBoard = boardModel.UnitTesting_GetBoard();
            for (int r = 1; r < boardModel.numOfRows() - 1; ++r) {
                for (int c = 1; c < boardModel.numOfColumns() - 1; ++c) {
                    tilesCount[generatedBoard[r,c]]++;
                }
            }
            int tilesCountSum = 0;
            for (int i = 1; i < tilesCount.Length; ++i) {
                tilesCountSum += tilesCount[i];
            }
            Assert.That(tilesCountSum
                        == (boardModel.numOfRows() - 2)
                        * (boardModel.numOfColumns() - 2));
            foreach(int tileCount in tilesCount) {
                Assert.That(tileCount % 2 == 0); // make sure its even tiles
            }
        }

        // remove() tests
        [Test]
        public void Remove1TileTest(){
            for (int r = 1; r < boardModel.numOfRows() - 1; ++r) {
                for (int c = 1; c < boardModel.numOfColumns() - 1; ++c) {
                    boardModel.UnitTesting_SetBoard((int[,]) filledGameBoard.Clone(), 15);
                    boardModel.remove(r, c);
                    int[,] gameBoard = boardModel.UnitTesting_GetBoard();
                    for (int checkingR = 1; checkingR < boardModel.numOfRows() - 1; ++checkingR) {
                        for (int checkingC = 1; checkingC < boardModel.numOfColumns() - 1; ++checkingC) {
                            if (checkingR == r && checkingC == c) {
                                Assert.That(gameBoard[checkingR, checkingC] == 0);
                            } else {
                                Assert.That(gameBoard[checkingR, checkingC] == filledGameBoard[checkingR, checkingC]);
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void Remove2TilesTest() {
            boardModel.UnitTesting_SetBoard((int[,]) filledGameBoard.Clone(), 15);
            int r1 = 2;
            int c1 = 2;
            int r2 = 3;
            int c2 = 3;
            boardModel.remove(r1, c1, r2, c2);
            int[,] gameBoard = boardModel.UnitTesting_GetBoard();
            for (int checkingR = 1; checkingR < boardModel.numOfRows() - 1; ++checkingR) {
                for (int checkingC = 1; checkingC < boardModel.numOfColumns() - 1; ++checkingC) {
                    if ((checkingR == r1 && checkingC == c1) ||
                        (checkingR == r2 && checkingC == c2)) {
                        Assert.That(gameBoard[checkingR, checkingC] == 0);
                    } else {
                        Assert.That(gameBoard[checkingR, checkingC] == filledGameBoard[checkingR, checkingC]);
                    }
                }
            }
        }

        [Test]
        public void RemoveColumnTest() {
            for (int c = 1; c < boardModel.numOfColumns() - 1; ++c) {
                boardModel.UnitTesting_SetBoard((int[,]) filledGameBoard.Clone(), 15);
                List<int> removedCount = boardModel.removeColumn(c);
                int[,] gameBoard = boardModel.UnitTesting_GetBoard();
                for (int checkingR = 1; checkingR < boardModel.numOfRows() - 1; ++checkingR) {
                    for (int checkingC = 1; checkingC < boardModel.numOfColumns() - 1; ++checkingC) {
                        if (checkingC == c) {
                            Assert.That(gameBoard[checkingR, checkingC] == 0);
                        } else {
                            Assert.That(gameBoard[checkingR, checkingC] == filledGameBoard[checkingR, checkingC]);
                        }
                    }
                }

                int[] elemCount = new int[5];
                for (int r = 1; r < boardModel.numOfRows() - 1; ++r) {
                    elemCount[filledGameBoard[r, c] - 1]++;
                }

                for (int i = 0; i < removedCount.Count; ++i) {
                    Assert.That(removedCount[i] == elemCount[i]);
                }
            }
        }

        [Test]
        public void RemoveRangeTest() {
            int startRow = 2;
            int startCol = 2;
            int endRow = 4;
            int endCol = 5;
            boardModel.UnitTesting_SetBoard((int[,]) filledGameBoard.Clone(), 15);
            List<int> removedCount = boardModel.removeRange(startRow, endRow, startCol, endCol);
            for (int r = startRow; r <= endRow; ++r ) {
                for (int c = startCol; c <= endCol; ++c ) {
                    Assert.That(boardModel.GetTileAt(r, c) == 0);
                }
            }
            int[] expectedCount = new int[]{ 2, 4, 0, 2, 4 };
            for (int i = 0; i < removedCount.Count; ++i) {
                Assert.That(removedCount[i] == expectedCount[i]);
            }
        }

        [Test]
        public void IsEmptyTest() {
            boardModel.UnitTesting_SetBoard(new int[7,8], 0);
            Assert.That(boardModel.isEmpty());
        }
    }
}
