using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCellController : MonoBehaviour
{
  Dictionary<string, int[]> nextPosition = new Dictionary<string, int[]>()
  {
    { "up",   new int[] { 0, 1, 0, 1} },
    { "down", new int[] { 0,-1, 0, 1} },
    { "left", new int[] {-1, 0, 0, 1} },
    { "right",new int[] { 1, 0, 0, 1} }
  };
  Dictionary<string, int[]> nextAction = new Dictionary<string, int[]>()
  {
    { "up",   new int[] { 0, 1, 0, 0} },
    { "down", new int[] { 0,-1, 0, 0} },
    { "left", new int[] { 0, 0, -90, 0} },
    { "right",new int[] { 0, 0,  90, 0} }
  };
  Dictionary<string, int[]> actions;

  public int ActionType
  {
    set { actions = value == 0 ? nextPosition : nextAction; }
  }

  Floor floor;
  PlayerMotion pmotion;

  public float AutoMovingSpan { get; set; }
  float autoMovedTime = 0f;
  float autoMovingSpeed = 1.0f;

  void Start()
  {
    ActionTyoe = 0;
    floor = GameObject.Find("Floor").GetComponent<Floor>();
    pmotion = GetComponent<PlayerMotion>();
  }

  void Update()
  {
    if( AutoMovingSpan == 0 )
    {
      foreach( var elem in actions )
      {
        if( Input.GetKeyDown(elem.Key) )
        {
          Move(elem.Value);
        }
      }
    }
    else if( Time.realtimeSinceStartup > autoMovedTime + AutoMovingSpan / autoMovingSpeed )
    {
      autoMovedTime = Time.realtimeSinceStartup;
      pmotion.Unset();

      int[] pos = floor.blocks.GetBlockIndexXZ( GetComponent<Transform>().position );
      List<string> avail = new List<string>();
      foreach( var d in nextPosition )
      {
        if( floor.blocks.IsWall( pos[0] + d.Value[0], pos[1] + d.Value[1] ) == false )
        {
          avail.Add(d.Key);
        }
        if( avail.Count != 0 )
        {
          Move( nextPosition[ avail[ UnityEngine.Random.Range( 0, avail.Count) ]] );
        }
      }
    }
    floor.UpdateObjPosition( gameObject.name, GetComponent<Transform>().position, GetComponent<Transform>().rotation );
  }

  public void SetColor( Color32 col)
  {
    GetComponent<Transform>().Find("Body").GetComponent<Renderer>().material.color = col;
  }
}
