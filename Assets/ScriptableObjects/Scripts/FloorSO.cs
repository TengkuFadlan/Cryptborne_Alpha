using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Floor", menuName = "Cryptborne/Floor Data")]
public class FloorSO : ScriptableObject
{
  public GameObject map;
  public List<WaveSO> waves;
  public String floorTip;
  public Sprite floorImageTip;
}