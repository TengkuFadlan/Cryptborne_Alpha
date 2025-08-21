using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Floor", menuName = "Cryptborne/Floor Data")]
public class FloorSO : ScriptableObject
{
  public GameObject map;
  public List<WaveSO> waves;
}