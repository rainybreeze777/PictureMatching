using System;
using System.Collections.Generic;

// Custom descending comparer
public class DescComparer<T> : IComparer<T> where T : IComparable {
    public int Compare(T x, T y) {
        return x.CompareTo(y) * -1; // Revert the sign meaning the order is reverted
    }
}
