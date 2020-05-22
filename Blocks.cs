using System;
﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blocks {

  public class BlockObj {

    public BlockObj(int x, int z, GameObject b) {
      this.X = x;
      this.Z = z;
      this.Block = b;
    }
    public int X { get; private set; }
    public int Z { get; private set; }
    public GameObject Block { get; set; }
  }

  GameObject prefab;
  Transform floor;
  int width;
  int height;
  BlockObj[] blocks;
  int[] map;
  bool remap;
  Vector3 blockSize;
  string prefsName;

  public Blocks(GameObject prefab, Transform floor, int dx, int dz, string prefsName){
    this.prefab = prefab;
    this.floor = floor;
    this.width = dx;
    this.height = dz;
    this.prefsName = prefsName;
    this.blockSize = prefab.GetComponent<Transform>().localScale;

    blocks = new BlockObj[ width * height ];
    map = new int[ blocks.Length ];
    foreach( var item in blocks.Select( (v, i) => new { v, i } )){
      blocks[item.i] = new BlockObj( i2x(item.i), i2z(item.i), null);
    }
  }

  public void Init( Dictionary<string, int[]> objPositions) {

  }

  public int i2x( int i) {
    return i % width;
  }
  public int i2z( int i) {
    return i / width;
  }
  public int[] i2xz( int i) {
    return new int[] { i2x(i), i2z(i) };
  }
  public int xz2i( int[] xz) {
    return xz2i( xz[0], xz[1] );
  }
  public int xz2i( int x, int z) {
    return x + z * width;
  }

  public BlockObj Find( GameObject obj) {
    return Array.Find<BlockObj>( blocks, x => x.Block == obj );
  }

  public int[] GetBlockIndexXZ( Vector3 pos) {
    int[] index = new int[] {
      Mathf.FloorToInt( ( pos.x - ( floor.position.x - floor.localScale.x / 2f) * width / floor.localScale.x )),
      Mathf.FloorToInt( ( pos.z - ( floor.position.z - floor.localScale.z / 2f) * height / floor.localScale.z ))
    };
    return index;
  }
  public int GetBlockIndex( Vector3 pos) {
    return xz2i( GetBlockIndexXZ(pos) );
  }

  public void CreateBlock(int x, int z, bool save = true) {
    blocks[ xz2i(x, z) ].Block =
      UnityEngine.Object.Instantiate(prefab, GetBlockPosition(x, z), Quaternion.identity);
    remap = true;
    if( save ) {
      // SavePrefs();
    }
  }

  public Vector3 GetBlockPosition( int index) {
    return GetBlockPosition( i2x(index), i2z(index) );
  }
  public Vector3 GetBlockPosition( int x, int z ) {
    return new Vector3(
      ( blockSize.x / 2f ) + ( floor.position.x - floor.localScale.x / 2f ) + ( x * floor.localScale.x / width ),
      ( blockSize.y / 2f ) + floor.position.y + floor.localScale.y / 2f,
      ( blockSize.z / 2f ) + ( floor.position.z - floor.localScale.z / 2f ) + ( z * floor.localScale.z / height )
      );
  }

  public void RemoveBlock( int x, int z, bool save = true ) {
    RemoveBlock( blocks[ xz2i(x, z) ], save );
  }
  public void RemoveBlock( BlockObj obj, bool save = true ) {
    UnityEngine.Object.Destroy( obj.Block );
    obj.Block = null;
    remap = true;
    if( save ) {
      // SavePrefs();
    }
  }
}
