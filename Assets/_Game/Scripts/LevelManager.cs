using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts
{
    public class LevelManager : Singleton<LevelManager>
    {
        [Header("ITroop")] public List<ITroop> enemies;
        public List<ITroop> players;

        public Troop melee, ranger, elite;
        public Troop eMelee, eRanger, eElite;

        public Tower ally, enemy;

        [Header("SpawnWave")] [SerializeField] SpawnWave[] spawnWaves;
        [SerializeField] SpawnWaveSO dataLevel;

        [Header("Hall")] [SerializeField] BoxCollider2D alliesHall;
        [SerializeField] BoxCollider2D enemiesHall;

        [Header("Buttons")] public Button buyMelee;
        public Button buyRange;
        public Button buyElite;

        private const int MELEE_COST = 3;
        private const int RANGE_COST = 5;
        private const int ELITE_COST = 12;

        public bool levelStart;
        public bool levelEnd;
        private float levelTimer;
        uint waveIndex;
        bool waveEnded = false;

        private void Start()
        {
            spawnWaves = dataLevel.spawnWaves;

            // Initialize troop buying buttons
            buyMelee.onClick.AddListener(() => BuyTroop(MELEE_COST, melee));
            buyRange.onClick.AddListener(() => BuyTroop(RANGE_COST, ranger));
            buyElite.onClick.AddListener(() => BuyTroop(ELITE_COST, elite));
        }

        private void Update()
        {
            if (levelEnd)
                return;

            if (levelStart)
            {
                SpawnEnemy();
            }
        }

        void SpawnEnemy()
        {
            if (levelEnd || waveEnded)
                return;

            levelTimer += Time.deltaTime;
            if (levelTimer >= spawnWaves[waveIndex].spawnTime)
            {
                StartCoroutine(SpawnWave(waveIndex));
                waveIndex++;
                if (waveIndex >= spawnWaves.Length)
                    waveEnded = true;
            }
        }

        IEnumerator SpawnWave(uint _waveIndex)
        {
            if (spawnWaves[_waveIndex].MeleeCount > 0)
            {
                for (int i = 0; i < spawnWaves[_waveIndex].MeleeCount; i++)
                {
                    SpawnEnemyMelee();
                    yield return new WaitForSeconds(.1f);
                }
            }

            if (spawnWaves[_waveIndex].RangeCount > 0)
            {
                for (int i = 0; i < spawnWaves[_waveIndex].RangeCount; i++)
                {
                    SpawnEnemyRange();
                    yield return new WaitForSeconds(.1f);
                }
            }

            if (spawnWaves[_waveIndex].EliteCount > 0)
            {
                for (int i = 0; i < spawnWaves[_waveIndex].EliteCount; i++)
                {
                    SpawnEnemyElite();
                    yield return new WaitForSeconds(.1f);
                }
            }

            yield return null;
        }

        void EndLevel()
        {
            levelEnd = true;
        }

        public void Recruit(ITroop troop)
        {
            if (troop.team == Team.Player)
            {
                players.Add(troop);
            }

            if (troop.team == Team.Enemy)
            {
                enemies.Add(troop);
            }
        }

        public void Remove(ITroop troop)
        {
            if (troop.team == Team.Player)
            {
                players.Remove(troop);
            }

            if (troop.team == Team.Enemy)
            {
                enemies.Remove(troop);
            }
        }

        public void SpawnEnemyMelee()
        {
            var spawned = Instantiate(eMelee, enemiesHall.transform, false);
            spawned.transform.localPosition = new Vector3(0, GetSpawnPos());
            Recruit(spawned);
        }

        public void SpawnEnemyRange()
        {
            var spawned = Instantiate(eRanger, enemiesHall.transform, false);
            spawned.transform.localPosition = new Vector3(0, GetSpawnPos());
            Recruit(spawned);
        }

        public void SpawnEnemyElite()
        {
            var spawned = Instantiate(eElite, enemiesHall.transform, false);
            spawned.transform.localPosition = new Vector3(0, GetSpawnPos());
            Recruit(spawned);
        }

        private float GetSpawnPos()
        {
            var spawnPos = Random.Range(alliesHall.bounds.min.y + .1f, alliesHall.bounds.max.y - .1f) +
                           alliesHall.bounds.min.y - .5f;
            return spawnPos;
        }

        public void Play()
        {
            levelStart = true;

            FoodProductionManager.Instance.StartProduction();

            ally.Start();
        }

        private void BuyTroop(int foodCost, Troop troopPrefab)
        {
            if (FoodProductionManager.Instance.CurrentFood >= foodCost)
            {
                FoodProductionManager.Instance.CurrentFood -= foodCost;

                // Spawn the troop
                var spawned = Instantiate(troopPrefab, alliesHall.transform, false);
                spawned.transform.localPosition = new Vector3(0, GetSpawnPos());
                Recruit(spawned.GetComponent<ITroop>());

                Debug.Log($"Bought {troopPrefab.name} for {foodCost} food.");
            }
        }

        public void UpdateButtonStatus(int val)
        {
            buyMelee.interactable = val >= MELEE_COST;
            buyRange.interactable = val >= RANGE_COST;
            buyElite.interactable = val >= ELITE_COST;
        }
    }
}