using UnityEngine;
using System.Collections;
using System;
//generic: works with any classes that follow the heap item properties (T) (made bellow), in this case nodes
public class Heap<T> where T : IHeapItem<T> {
	
	//constructor (object)
	/*The where clause in a generic definition specifies constraints on the types that are used as arguments for type parameters in a generic type, 
	method, delegate, or local function. Constraints can specify interfaces, base classes, 
	or require a generic type to be a reference, value, or unmanaged type. They declare capabilities that the type argument must have.
	https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/where-generic-type-constraint
	*/
	
	T[] items;
	int currentItemCount;
	
	public Heap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}
	
	public void Add(T item) {
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;
	}

	public T RemoveFirst() {
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	public void UpdateItem(T item) {
		SortUp(item);
	}

	public int Count {
		get {
			return currentItemCount;
		}
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	void SortDown(T item) {
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					Swap (item,items[swapIndex]);
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
	
	void SortUp(T item) {
		int parentIndex = (item.HeapIndex-1)/2;
		
		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				Swap (item,parentItem);
			}
			else {
				break;
			}

			parentIndex = (item.HeapIndex-1)/2;
		}
	}
	
	void Swap(T itemA, T itemB) {
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

/*
An interface contains definitions for a group of related functionalities that a non-abstract class or a struct must implement. 
An interface may define static methods, which must have an implementation
*/
//interface inherits another interface
public interface IHeapItem<T> : IComparable<T> {//icomparable is a c# generic (IComparable is implemented by types whose values can be ordered or sorted)
	//access and change private fields (get; set; for shorthand return value and change value)
	int HeapIndex {
		get;
		set;
	}
}