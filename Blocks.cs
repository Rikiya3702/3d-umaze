using System.Collections;
using System.Collections.Generic;
using System Linq;
using UnityEngine;

public class Blocks {

  public class BlockObj {

    public class BlockObj(int x, int z, GameObject b) {
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

    blocks = new Blockobj[ width * height ];
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

}
