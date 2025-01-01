using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_0", menuName = "SpawnWave", order = 1)]
public class SpawnWaveSO : ScriptableObject
{
    public SpawnWave[] spawnWaves;

}
[Serializable]
public struct SpawnWave
{
    public float spawnTime;
    public uint MeleeCount;
    public uint RangeCount;
    public uint EliteCount;
}