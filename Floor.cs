﻿using System;
﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

  Camera birdEye;
  Camera playersEye;
  public bool start_bird_view;

  ModalDialog dlg;

  GameObject timerText;
  float timer = 0;

  public AudioClip audio_bgm;
  public AudioClip audio_bird;
  public AudioClip audio_navi;
  public AudioClip audio_warning;
  public AudioClip audio_distance;
  public float volume_bgm;
  Dictionary<string, AudioClip> audio_bgms;
  string currentBGM = "";
  AudioSource audio_source_bgm;

  public AudioClip audio_goal;
  public AudioClip audio_break;
  public AudioClip audio_damage;
  public float volume_effects;
  AudioSource audio_source_effects;

  RouteRenderer routeRenderer;

  enum ItemNames
  {
    Bird,
    Navi,
    Distance,
    BreakWall
  }
  Dictionary<ItemNames, Item> items;
  int[] toBreakBlockXZ = new int[] { -1, -1};
  public GameObject itemBreakWall;
  public GameObject textPrefab;
  List<GameObject> distances = new List<GameObject>();

  Coroutine timerColor = null;
  IEnumerator TimerColor( Color c0, Color c1, float time)
  {
    int div = 10;
    for( int cnt = 0; cnt < div; cnt++)
    {
      float r = (float)cnt / (div - 1);
      Color c = c0 * (1 - r) + c1 * r;
      timerText.GetComponent<Text>().color = c;
      yield return new WaitForSeconds( time / div );
    }
    timerColor = null;
  }

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
        scale.y,
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
          ctrl.AutoMovingSpan = 0f;

          ctrl.AddTriggerAction(goalName, () =>
            {
              ctrl.CancelMotions();
              dlg.DoModal( name => {}, timer.ToString("0.0") );
              timer = 0.0f;

              transform.position = blocks.GetBlockPosition( objPositions[startName][0], objPositions[startName][1]);
              transform.localRotation = Quaternion.identity;

              audio_source_effects.PlayOneShot(audio_goal);
            });
          ctrl.AddTriggerAction(enemyName, () =>
            {
              timer += 5.0f;

              if( timerColor != null)
              {
                StopCoroutine(timerColor);
              }
              timerColor = StartCoroutine( TimerColor(Color.red, Color.black, 5f) );

              audio_source_effects.PlayOneShot(audio_damage);
            });

        }else if(item.v == enemyName) {
          ctrl.AutoMovingSpan = 5f;
          ctrl.SetColor( new Color32(165, 35, 86, 255) );

          ctrl.SetLayer( LayerMask.NameToLayer("Ignore Raycast") );
          ctrl.TrackingObject = player.GetComponent<Transform>();

        }
        return true;
      }
    );

    //Camera
    birdEye = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    birdEye.enabled = false;
    playersEye = player.GetComponent<Transform>().Find("Camera").GetComponent<Camera>();
    playersEye.enabled = true;

    SetPlayerActionType();
    if( start_bird_view == true)
    {
      ChangeCamera();
    }

    //Timer
    timerText = GameObject.Find("TimerText");

    //Modal
    dlg = GameObject.Find("Canvas").GetComponent<ModalDialog>();

    //Audio
    audio_source_effects = gameObject.AddComponent<AudioSource>();
    audio_source_effects.volume = volume_effects;
    audio_source_bgm = gameObject.AddComponent<AudioSource>();
    audio_source_bgm.volume = volume_bgm;
    audio_source_bgm.loop = true;

    audio_bgms = new Dictionary<string, AudioClip>()
    {
      {"Default", audio_bgm },
      {"Bird",     audio_bird },
      {"Navi",     audio_navi },
      {"Warning",  audio_warning },
      {"Distance", audio_distance }
    };
    BGM("Default");

    routeRenderer = gameObject.AddComponent<RouteRenderer>();

    //Items
    blocks.All( (x,y) => {
      GameObject o = Instantiate(textPrefab, blocks.GetBlockPosition(x,y), Quaternion.Euler(90, 0, 0));
      o.GetComponent<TextMesh>().text = Mathf.Sqrt(
        Mathf.Pow(objPositions[goalName][0] - x, 2) + Mathf.Pow(objPositions[goalName][1] - y, 2) ).ToString("0.0");
      o.SetActive(false);
      distances.Add(o);
      return true;
    });

    items = new Dictionary<ItemNames, Item>()
    {
      {ItemNames.Bird, new Item(
        5f,
        () => {
          BGM("Bird");
          ChangeCamera();
        },
        () => {
          BGM("Default");
          ChangeCamera();
        }
      )},
      {ItemNames.BreakWall, new Item(
        0f,
        () => {
          if(toBreakBlockXZ[0] != -1)
          {
            audio_source_effects.PlayOneShot(audio_break);
            blocks.RemoveBlock(toBreakBlockXZ[0], toBreakBlockXZ[1], false);
          }
        },
        () => {
        }
      )},
      {ItemNames.Distance, new Item(
        5f,
        () => {
          BGM("Distance");
          distances.ForEach( o => o.SetActive(true) );
        },
        () => {
          BGM("Default");
          distances.ForEach( o => o.SetActive(false) );
        }
      )},
    };

  }

  public void UpdateObjPosition( string name, Vector3 pos, Quaternion rot) {
    int [] index = blocks.GetBlockIndexXZ(pos);

    if( name == playerName)
    {
      int nx = index[0] + Mathf.RoundToInt( (rot * Vector3.forward).x );
      int nz = index[1] + Mathf.RoundToInt( (rot * Vector3.forward).z );
      bool enable = blocks.IsIn( nx, nz ) && blocks.IsWall( nx, nz );
      itemBreakWall.GetComponent<Button>().interactable = enable;
      toBreakBlockXZ[0] = enable ? nx : -1;
      toBreakBlockXZ[1] = enable ? nz : -1;
    }

    objPositions[name] = index;

  }

  void Update()
  {
    if( dlg.Active )
    {
      return;
    }

    List<int> route = blocks.Solve( blocks.xz2i(objPositions[playerName]), blocks.xz2i(objPositions[goalName]) );
    routeRenderer.Render( route, i => blocks.GetBlockPosition(i) );

    if( birdEye.enabled )
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

    timer += Time.deltaTime;
    timerText.GetComponent<Text>().text = timer.ToString("0.0");

    foreach( var item in items)
    {
      if( false == item.Value.Next( Time.deltaTime) )
      {
        item.Value.End();
      }
    }
  }

  void SetPlayerActionType()
  {
    player.GetComponent<PlayerCellController>().ActionType = birdEye.enabled ? 0 : 1;
  }

  void ChangeCamera()
  {
    birdEye.enabled = !birdEye.enabled;
    playersEye.enabled = !playersEye.enabled;
    SetPlayerActionType();
  }

  public void ItemChangeCamera()
  {
    doItem( ItemNames.Bird );
  }
  public void ItemBreakWall()
  {
    doItem( ItemNames.BreakWall );
  }
  public void ItemDistance()
  {
    doItem( ItemNames.Distance );
  }
  void doItem( ItemNames name )
  {
    foreach( var item in items )
    {
      item.Value.End();
    }
    items[name].Start();
  }

  public void BGM(string type)
  {
    if( audio_source_bgm.clip != audio_bgms[type])
    {
      currentBGM = type;
      audio_source_bgm.clip = audio_bgms[type];
      audio_source_bgm.Play();
    }
  }

  public string BGM()
  {
    return currentBGM;
  }

}
