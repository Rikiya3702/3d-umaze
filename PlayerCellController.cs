using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCellController : MonoBehaviour {
  Dictionary<string, int[]> nextPosition = new Dictionary<string, int[]>() {
    { "up",   new int[] { 0, 1, 0, 1} },
    { "down", new int[] { 0,-1, 0, 1} },
    { "left", new int[] {-1, 0, 0, 1} },
    { "right",new int[] { 1, 0, 0, 1} }
  };
  Dictionary<string, int[]> nextAction = new Dictionary<string, int[]>() {
    { "up",   new int[] { 0, 1, 0, 0} },
    { "down", new int[] { 0,-1, 0, 0} },
    { "left", new int[] { 0, 0, -90, 0} },
    { "right",new int[] { 0, 0,  90, 0} }
  };
  Dictionary<string, int[]> actions;

  public int ActionType {
    set { actions = value == 0 ? nextPosition : nextAction; }
  }

    void Start()
    {

    }

    void Update()
    {

    }
}
