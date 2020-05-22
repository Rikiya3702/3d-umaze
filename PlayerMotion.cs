﻿using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
  public class Animation
  {
    public float Duration { get; set; }
    public Action Complete { get; set; }
    Action<float> animate;

    public Animation( Action<float> a, float d, Action c = null )
    {
      this.animate = a;
      this.Duration = d;
      this.Complete = c;
    }

    public void Animate(float p)
    {
      animate(p);
      if( p >= 1.0f && Complete != null)
      {
        Complete();
      }
    }
  }

  List<Animation> animations = new List<Animation>();
  float started_time = 0;

  public void Add( Action<float> animate, float duration, Action complete = null)
  {
    Add( new Animation(animate, duration, complete) );
  }
  public void Add(Animation[] anis)
  {
    foreach( Animation ani in anis )
    {
      Add(ani);
    }
  }
  public void Add(Animation ani)
  {
    this.animations.Add(ani);
  }
  public void Unset()
  {
    animations.ForEach(a => a.Animate(1f));
    animations.Clear();
    started_time = 0f;
  }
  public void Cancel()
  {
    animations.Clear();
    started_time = 0f;
  }
  public void Set( Action<float> animate, float duration, Action complete = null)
  {
    Unset();
    Add( new Animation(animate, duration, complete) );
  }

  void Start()
  {
  }

  void Update()
  {
    if( animations.Count > 0)
    {
      if( started_time == 0f)
      {
        started_time = Time.realtimeSinceStartup;
      }
      float progress = (Time.realtimeSinceStartup - started_time) / animations[0].Duration;
      animations[0].Animate( Mathf.Min(1f, progress) );

      if( progress >= 1.0f )
      {
        animations.RemoveAt(0);
        started_time = 0f;
      }
    }
  }
}
