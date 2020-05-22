using System;
﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floor : MonoBehaviour
{
  public GameObject blockPreb;
  public GameObject playerPreb;
  GameObject player;

  public Blocks blocks;
  int dx = 10;
  int dz = 10;
  Transform floor;

  string playerName = "Player";
  string enemyName = "Enemy";
  string goalName = "Goal";
  string startName = "Start";
  Dictionary<string, int[]> objPositions = new Dictionary<string, int[]>();

  void Start() {
    floor = GetComponent<Transform>();

    // Object start position
    objPositions[playerName] = new int[] { 0, 0 };
    objPositions[startName] = new int[]  { 0, 0 };
    objPositions[goalName] = new int[] { dx - 1, dz - 1 };
    objPositions[enemyName] = new int[] { Mathf.RoundToInt(dx / 2), Mathf.RoundToInt(dz / 2) };

    //Blocks
    blockPreb.GetComponent<Transform>().localScale =
      new Vector3(floor.localScale.x / dx, 1f, floor.localScale.z / dz);
    blocks = new Blocks(blockPreb, floor, dx, dz, "map");
    blocks.Init(objPositions);

    //Goal
    GameObject goal = GameObject.Find(goalName);
    goal.name = goalName;
    goal.GetComponent<Transform>().position = blocks.GetBlockPosition( objPositions[goalName][0], objPositions[goalName][1] );

    //Walls
    Vector3 scale = blockPreb.GetComponent<Transform>().localScale;
    for( int angle = 0; angle < 360; angle += 90 ) {
      float x = Mathf.Cos(Mathf.Deg2Rad * angle);
      float z = Mathf.Sin(Mathf.Deg2Rad * angle);

      blockPreb.GetComponent<Transform>().localScale = new Vector3(
        Mathf.RoundToInt(z * 10) == 0 ? 0.01f : floor.localScale.x,
        scale.y / 2f,
        Mathf.RoundToInt(x * 10) == 0 ? 0.01f : floor.localScale.z
        );

      float px = x * floor.localScale.x / 2f;
      float pz = z * floor.localScale.z / 2f;
      float py = floor.localScale.y / 2f + floor.position.y + scale.y / 2f;
      Instantiate( blockPreb, new Vector3(px, py, pz), Quaternion.identity );
    }
    blockPreb.GetComponent<Transform>().localScale = scale;

    //Player & Enemey
    new string[] { playerName, enemyName }.Select( (v, i) => new { v, i }).All(
      item => {
        GameObject obj = Instantiate(playerPreb);
        obj.name = item.v;
        Transform transform = obj.GetComponent<Transform>();
        Vector3 p = blocks.GetBlockPosition( objPositions[item.v][0], objPositions[item.v][1]);
        p.y = floor.localScale.y / 2f + floor.position.y + transform.localScale.y * transform.Find("Body").localScale.y / 2f;
        transform.position = p;

        PlayerCellController ctrl = obj.GetComponent<PlayerCellController>();

        if(item.v == playerName) {
          player = obj;
          // crtl.AutoMovingSpan = 0f;
        }else if(item.v == enemyName) {
          // crtl.AutoMovingSpan = 5f;
          // ctrl.SetColor( new Color32(165, 35, 86, 255) );
        }
        return true;
      }
    );
  }

  public void UpdateObjPosition( string name, Vector3 pos, Quaternion rot) {
    int [] index = blocks.GetBlockIndexXZ(pos);
    objPositions[name] = index;
  }

  void Update()
  {
    int i = Enumerable.Range(1, 2).FirstOrDefault( v => Input.GetMouseButtonDown( v - 1));
    if( i != 0) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      RaycastHit hit = new RaycastHit();
      if( Physics.Raycast( ray.origin, ray.direction, out hit, Mathf.Infinity)){
        Blocks.BlockObj target = blocks.Find(hit.collider.gameObject);
        if( i == 2 && target != null) {
          blocks.RemoveBlock(target);
        }
        else if( i == 1 && gameObject == hit.collider.gameObject) {
          int[] index = blocks.GetBlockIndexXZ( hit.point );
          blocks.CreateBlock( index[0], index[1]);
        }
      }
    }

  }
}
