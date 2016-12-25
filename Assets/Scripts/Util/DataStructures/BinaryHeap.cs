using System;
using System.Collections.Generic;

public class BinaryHeap<T> where T : IComparable<T> {

    private List<T> m_heap = new List<T>();
    private IComparer<T> m_comparer = null;

    public void Insert(T item) {
        m_heap.Add(item);
        BubbleUp(m_heap.Count - 1);
    }

    public T Pop() {
        if (m_heap.Count == 0) { return default(T); }

        T ret = m_heap[0];
        m_heap[0] = m_heap[m_heap.Count - 1];
        m_heap.RemoveAt(m_heap.Count - 1);
        BubbleDown(0);
        return ret;
    }

    public T Peek() {
        if (m_heap.Count == 0) { return default(T); }

        return m_heap[0];
    }

    public int Count { get { return m_heap.Count; } }

    public void Clear() {
        m_heap.Clear();
    }

    public BinaryHeap(IComparer<T> comparer) {
        m_comparer = comparer;
    }

    private void BubbleUp(int checkingIndex) {
        if (checkingIndex == 0) { return; }

        bool isLeftChild = (checkingIndex % 2) == 1;
        int parentIndex = isLeftChild ? (checkingIndex - 1) / 2 : checkingIndex / 2 - 1;

        if (m_comparer.Compare(m_heap[checkingIndex], m_heap[parentIndex]) < 0) {
            Swap(checkingIndex, parentIndex);
            BubbleUp(parentIndex);
        }
    }

    private void BubbleDown(int checkingIndex) {
        int leftChildIndex = checkingIndex * 2 + 1;
        int rightChildIndex = (checkingIndex + 1) * 2;

        if (leftChildIndex >= m_heap.Count && rightChildIndex >= m_heap.Count) { 
            return;
        } else if (rightChildIndex >= m_heap.Count) {
            if (m_comparer.Compare(m_heap[leftChildIndex], m_heap[checkingIndex]) < 0) {
                Swap(leftChildIndex, checkingIndex);
            }
        } else {
            int indexToBeSwapChecked = m_comparer.Compare(m_heap[leftChildIndex], m_heap[rightChildIndex]) < 0 ? leftChildIndex : rightChildIndex;
            if (m_comparer.Compare(m_heap[indexToBeSwapChecked], m_heap[checkingIndex]) < 0) {
                Swap(indexToBeSwapChecked, checkingIndex);
                BubbleDown(indexToBeSwapChecked);
            }
        }
    }

    private void Swap(int index1, int index2) {
        T temp = m_heap[index1];
        m_heap[index1] = m_heap[index2];
        m_heap[index2] = temp;
    }
}
