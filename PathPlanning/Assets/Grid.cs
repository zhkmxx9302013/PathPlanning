﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid : MonoBehaviour {
	// public Transform player;
	public bool onlyDisplayPathGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize; //30x30
	public float nodeRadius; // 0.5
	Node[,] grid;
	public List<Node> path;
	public int MaxSize{
		get{return gridSizeX * gridSizeY;}
	}

	float nodeDiameter;
	int gridSizeX, gridSizeY;
	private void Awake() {
		nodeDiameter = 2 * nodeRadius; //1
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter); //30
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter); //30
		CreatGrid();
	}

	void CreatGrid(){
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward*gridWorldSize.y/2;

		for(int x = 0; x < gridSizeX; x++){
			for(int y = 0; y < gridSizeY; y++){
				Vector3 worldPoint = worldBottomLeft+ Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				//Collision
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
				grid[x,y] = new Node(walkable, worldPoint, x, y);
			}
		}
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition){
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid[x,y];
	}

	public List<Node> GetNeighbours(Node node){
		List<Node> neighbours = new List<Node>();

		for(int x = -1; x <= 1; x++){
			for(int y = -1; y <= 1; y++){
				if(x == 0 && y == 0){
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				//判断是否出界
				if(checkX >= 0 && checkX < gridSizeX && checkY >=0 && checkY < gridSizeY){
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	
	private void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if (grid != null && onlyDisplayPathGizmos) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
	// 	Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

	// 	if(onlyDisplayPathGizmos){
	// 		if(path != null){
	// 			foreach(Node n in path){
	// 				Gizmos.color = Color.black;
	// 				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
	// 			}
	// 		}
	// 	}else{
	// 		if(grid != null){
	// 		//Node playerNode = NodeFromWorldPoint(player.position);
	// 			foreach(Node n in grid){
	// 				Gizmos.color = (n.walkable) ? Color.white : Color.red;
	// 				if(path != null)
	// 					if(path.Contains(n)){
	// 						Gizmos.color = Color.black;
	// 					}
	
	// 				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
	// 			}
	// 		}
	// 	}
		
	// }
}
