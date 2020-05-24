using System;
﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouteRenderer : MonoBehaviour
{

  LineRenderer line;
  public Material line_material;

  void Start()
  {
    line = ( new GameObject("RouteRenderer") ).AddComponent<LineRenderer>();
    line.startWidth = 0.1f;
    line.endWidth = 0.1f;
    line.material = line_material;
  }

  public void Render( List<int> route, Func<int, Vector3> i2p, Color color )
  {
    line.startColor = color;
    line.endColor = line.startColor;
    line.positionCount = route.Count;
    foreach( var item in route.Select( (v, i) => new { v, i} ))
    {
      line.SetPosition( item.i, i2p(item.v) );
    }
  }
  public void Render( List<int> route, Func<int, Vector3> i2p )
  {
    Render( route, i2p, new Color32(248, 248, 127, 200) );
  }
  public void Clear()
  {
    line.positionCount = 0;
  }

  void Update()
  {

  }
}
