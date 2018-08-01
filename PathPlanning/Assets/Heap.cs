using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Heap<T> where T :IHeapItem<T> {

	T[] items;
	int currentItemCount;

	public Heap(int maxHeapSize){
		items = new T[maxHeapSize];
	}

	public void Add(T item){
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount ++;
	}

	public T RemoveFirst(){
		T firstItem = items[0];
		currentItemCount --;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	public int Count(){
		return currentItemCount;
	}

	public void UpdateItem(T item){
		SortUp(item);
	}
	public bool Contains(T item){
		return Equals(items[item.HeapIndex], item);
	}

	void SortUp(T item){
		int parentIndex = (item.HeapIndex - 1) / 2;
		while(true){
			T parentItem = items[parentIndex];
			if(item.CompareTo(parentItem) > 0)
			{
				Swap(item, parentItem);
			}else{
				break;
			}
			//更新父节点继续比较
			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	void SortDown(T item){
		int parentIndex = (item.HeapIndex - 1) / 2;
		while(true){
			int childLeftIndex = 2 * item.HeapIndex + 1;
			int childRightIndex = 2 * item.HeapIndex + 2;
			int swapIndex = 0;
			if(childLeftIndex < currentItemCount){
				swapIndex = childLeftIndex;

				if(childRightIndex < currentItemCount){
					if(items[childLeftIndex].CompareTo(items[childRightIndex]) < 0){
						swapIndex = childRightIndex;
					}
				}

				if(item.CompareTo(items[swapIndex]) < 0){
					Swap(item, items[swapIndex]);
				}else{
					return;
				}

			}else{
				return;
			}
		}
	}


	void Swap(T itemA, T itemB){
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;

		int indexA = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = indexA;
	}
}


public interface IHeapItem<T> : IComparable<T>{
	int HeapIndex{
		set;
		get;
	}
}
