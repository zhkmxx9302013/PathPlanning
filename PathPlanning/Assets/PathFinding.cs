using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour {

	Grid grid;
	public Transform seeker, target;

	List<Node> gen_path;

	private void Awake() {
		grid = GetComponent<Grid>();	
	}

	
    Vector3 velocity;
	float speed = 1.0f;
	private void Update() {
		if(FindPath(seeker.position, target.position)){
			// seeker.transform.LookAt(obj[i % obj.Length]);
		}else{

		}
	}
	bool FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targedNode = grid.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();

		//将起始点加入Open
		openSet.Add(startNode);

		while(openSet.Count > 0)
		{
			//默认取出open的第一个元素作为遍历首节点。
			Node currentNode = openSet[0];
			for(int i = 0; i < 	openSet.Count; i++){
				//在Open表中找出最小的fCost节点
				if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost){
					currentNode = openSet[i];
				}
			}

			//从Open表中移除代价最小的节点
			openSet.Remove(currentNode);

			//在Close表中加入代价最小的节点
			closedSet.Add(currentNode);

			//是否到达目标点
			if(currentNode == targedNode){
				RetracePath(startNode, targedNode);
				return true;
			}

			//遍历邻居节点
			foreach(Node neighbour in grid.GetNeighbours(currentNode)){
				if(!neighbour.walkable || closedSet.Contains(neighbour)) continue;

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)){
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targedNode);
					neighbour.parent = currentNode;

					if(!openSet.Contains(neighbour)){
						openSet.Add(neighbour);
					}
				}
			}

		}
		return false;
	}

	public void RetracePath(Node startNode, Node targetNode){
		List<Node> path = new List<Node>();
		Node currentNode = targetNode;
		while(currentNode != startNode){
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
		gen_path = path;
		grid.path = path;
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
