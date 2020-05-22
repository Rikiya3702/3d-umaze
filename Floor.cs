﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
  public GameObject blockPreb;
  public Blocks blocks;
  int dx = 10;
  int dz = 10;
  Transform floor;

  string playerName = "Player";
  string enemyName = "Enemy";
  string goalName = "Goal";
  string startName = "Start";
  Dictionary<string, int[]> objPositions = new Dictionary<string, int[]>();
    void Start()
    {
      floor = GetComponent<Transform>();

      // Object start position
      objPositions[playerName] = new int[] [ 0, 0 ];
      objPositions[startName] = new int[]  [0, 0 ];
      objPositions[goalName] = new int[]  [dx - 1, dz - 1 ];
      objPositions[enemyName] = new int[]  [Mathf.RoundToInt(dx / 2), Mathf.RoundToInt(dz / 2) ];

      //Blocks
      blockPreb.GetComponent<Transform>().localScale =
        newVector3(floor.localScale.x / dx, 1f, floor.localScale.z / dz);
      blocks = new Blocks(blockPreb, floor, dx, dz, "map");
      blocks.Init(objPositions);
    }

    void Update()
    {

    }
}
