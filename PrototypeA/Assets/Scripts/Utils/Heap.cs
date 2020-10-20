using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T> {
	public int Count => _currentItemCount;

	T[] _items;
	int _currentItemCount;

	public Heap(int p_maxHeapSize) {
		_items = new T[p_maxHeapSize];
	}

	public void Add(T p_item) {
		p_item.HeapIndex = _currentItemCount;
		_items[_currentItemCount] = p_item;
		SortUp(p_item);
		_currentItemCount++;
	}

	public T RemoveFirst() {
		T __firstItem = _items[0];
		_currentItemCount--;
		_items[0] = _items[_currentItemCount];
		_items[0].HeapIndex = 0;
		SortDown(_items[0]);

		return __firstItem;
	}

	public void UpdateItem(T p_item) {
		SortUp(p_item);
		SortDown(p_item);
	}

	public bool Contains(T p_item) {
		return Equals(_items[p_item.HeapIndex], p_item);
	}

	void SortDown(T p_item) {
		while (true) {
			int __childIndexLeft = p_item.HeapIndex * 2 + 1;
			int __childIndexRight = p_item.HeapIndex * 2 + 2;
			int __swapIndex = 0;

			if (__childIndexLeft < _currentItemCount) {
				__swapIndex = __childIndexLeft;

				if (__childIndexRight < _currentItemCount) {
					if (_items[__childIndexLeft].CompareTo(_items[__childIndexRight]) < 0) {
						__swapIndex = __childIndexRight;
					}
				}

				if (p_item.CompareTo(_items[__swapIndex]) < 0) {
					Swap(p_item, _items[__swapIndex]);
				}
				else {
					return;
				}
			}
			else {
				return;
			}
		}
	}

	void SortUp(T p_item) {
		int __parentIndex = (p_item.HeapIndex - 1) / 2;

		while (true) {
			T __parentItem = _items[__parentIndex];
			if (p_item.CompareTo(__parentItem) > 0) {
				Swap(p_item, __parentItem);
			}
			else {
				break;
			}

			__parentIndex = (p_item.HeapIndex - 1) / 2;
		}
	}

	void Swap(T p_itemA, T p_itemB) {
		_items[p_itemA.HeapIndex] = p_itemB;
		_items[p_itemB.HeapIndex] = p_itemA;

		int __itemAIndex = p_itemA.HeapIndex;
		p_itemA.HeapIndex = p_itemB.HeapIndex;
		p_itemB.HeapIndex = __itemAIndex;
	}
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex { get; set; }
}
