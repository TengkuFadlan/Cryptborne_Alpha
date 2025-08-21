using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Wave", menuName = "Cryptborne/Wave Data")]
public class WaveSO : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject enemyPrefab;
        public Vector2 spawnPosition;
    }

    public List<EnemySpawn> enemiesToSpawn;
}