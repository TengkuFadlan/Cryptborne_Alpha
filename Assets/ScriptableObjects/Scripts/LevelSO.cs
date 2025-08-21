using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level", menuName = "Cryptborne/Level")]
public class LevelSO : ScriptableObject
{
  public GameObject playerCharacter;
  public List<FloorSO> floors;
}