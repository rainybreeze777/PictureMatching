using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;

namespace BinaryHeapUnitTests {

    [TestFixture]
    internal class BinaryHeapUnitTests {

        private BinaryHeap<int> bh;

        [SetUp]
        public void Init() {
            bh = new BinaryHeap<int>(Comparer<int>.Default);
        }

        [Test]
        public void InsertTest() {
            List<int> heap = bh.UnitTesting_GetHeap();
            Assert.That(heap.Count == 0);
            bh.Insert(3);
            bh.Insert(1);
            bh.Insert(2);
            Assert.That(heap.Count == 3);
        }

        [Test]
        public void CountTest() {
            Assert.That(bh.Count == 0);
            for (int i = 0; i < 10; ++i) {
                bh.Insert(i);
            }
            Assert.That(bh.Count == 10);
        }

        [Test]
        public void PeekTest() {
            bh.Insert(4);
            bh.Insert(2);
            bh.Insert(5);
            bh.Insert(1);
            bh.Insert(3);
            Assert.That(bh.Peek() == 1);
            Assert.That(bh.Count == 5);
        }

        [Test]
        public void PopTest() {
            bh.Insert(4);
            bh.Insert(2);
            bh.Insert(5);
            bh.Insert(1);
            bh.Insert(3);
            for (int i = 1; i <= 5; ++i) {
                Assert.That(bh.Count == (5 - i + 1));
                Assert.That(bh.Pop() == i);
            }
            Assert.That(bh.Count == 0);
        }

        [Test]
        public void ClearTest() {
            bh.Insert(4);
            bh.Insert(2);
            bh.Insert(5);
            bh.Insert(1);
            bh.Insert(3);
            bh.Clear();
            Assert.That(bh.Count == 0);
        }
    }
}
