using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathFinding : MonoBehaviour {

	Grid grid;
	public Transform seeker, target;
	PathRequestManager requestManager;

	List<Node> gen_path;

	void Awake() {
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}

	
	// private void Update() {
	// 	if(Input.GetButtonDown("Jump")){
	// 		if(FindPath(seeker.position, target.position)){
	// 		// seeker.transform.LookAt(obj[i % obj.Length]);
	// 		}else{

	// 		}
	// 	}
	// }

	public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
		StartCoroutine(FindPath(startPos,targetPos));
	}
	
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		
		
		if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count() > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		yield return null;
		if (pathSuccess) {
			waypoints = RetracePath(startNode,targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints,pathSuccess);
		
	}
	// bool FindPath(Vector3 startPos, Vector3 targetPos)
	// {
	// 	Node startNode = grid.NodeFromWorldPoint(startPos);
	// 	Node targedNode = grid.NodeFromWorldPoint(targetPos);

	// 	// List<Node> openSet = new List<Node>();
	// 	Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
	// 	HashSet<Node> closedSet = new HashSet<Node>();

	// 	//将起始点加入Open
	// 	openSet.Add(startNode);

	// 	while(openSet.Count() > 0)
	// 	{
	// 		//默认取出open的第一个元素作为遍历首节点。
	// 		//Node currentNode = openSet[0];
	// 		Node currentNode = openSet.RemoveFirst();
	// 		// for(int i = 0; i < 	openSet.Count; i++){
	// 		// 	//在Open表中找出最小的fCost节点
	// 		// 	if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost){
	// 		// 		currentNode = openSet[i];
	// 		// 	}
	// 		// }

	// 		//从Open表中移除代价最小的节点
	// 		// openSet.Remove(currentNode);

	// 		//在Close表中加入代价最小的节点
	// 		closedSet.Add(currentNode);

	// 		//是否到达目标点
	// 		if(currentNode == targedNode){
	// 			RetracePath(startNode, targedNode);
	// 			return true;
	// 		}

	// 		//遍历邻居节点
	// 		foreach(Node neighbour in grid.GetNeighbours(currentNode)){
	// 			if(!neighbour.walkable || closedSet.Contains(neighbour)) continue;

	// 			int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
	// 			if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)){
	// 				neighbour.gCost = newMovementCostToNeighbour;
	// 				neighbour.hCost = GetDistance(neighbour, targedNode);
	// 				neighbour.parent = currentNode;

	// 				if(!openSet.Contains(neighbour)){
	// 					openSet.Add(neighbour);
	// 				}
	// 			}
	// 		}

	// 	}
	// 	return false;
	// }

	public Vector3[] RetracePath(Node startNode, Node targetNode){
		List<Node> path = new List<Node>();
		Node currentNode = targetNode;
		while(currentNode != startNode){
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		// path.Reverse();
		// gen_path = path;
		// grid.path = path;
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < path.Count; i ++) {
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}


	public int GetDistance(Node nodeA, Node nodeB){
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		/*
			* 对角线代价为14， 平移代价为10， 纵向为y， 横向为x， 
			* A到B的距离为 14*y + 10*(x-y) 或 14*x + 10*(y-x) 
			
				o o ↗ → → → B
				o ↗ o o o o o
				A o o o o o o
				
		 */
		if(dstX > dstY){
			return 14 * dstY + 10 * (dstX - dstY);
		}else{
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}
}
