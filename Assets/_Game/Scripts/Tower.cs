using System.Collections;
using _Game.Scripts.Gameplay;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Scripts
{
    public class Tower : ITroop
    {
        public int initialHealth = 8;
        public HealthSystemForDummies healthBar;

        public void Start()
        {
            if (team == Team.Player)
            {
                initialHealth = 2 + DataManager.TowerLevel * 4;
            }

            healthBar.MaximumHealth = initialHealth;
            healthBar.CurrentHealth = initialHealth;
        }

        public override void GetHit(uint _dmg)
        {
            if (isDead)
                return;
            if (team == Team.Enemy)
            {
                var goldSpawner = FindObjectOfType<GoldSpawner>();
                if (goldSpawner != null)
                {
                    goldSpawner.SpawnGold(transform.position, (int)_dmg);
                }
            }
            
            healthBar.AddToCurrentHealth(-_dmg);
        }

        public override IEnumerator SetDead(bool val)
        {
            if (team == Team.Player)
            {
                Debug.Log("<color=green>PLAYER WIN</color>");
            }
            else
            {
                Debug.Log("<color=green>ENEMIES WIN</color>");
            }

            isDead = val;

            LevelManager.Instance.Remove(this);
            
            Time.timeScale = 0f;
            
            yield return null;
            
           gameObject.SetActive(false);
           
           Debug.Log($"<color=red>{gameObject.name}</color> Is Dead = {val}");

            DOVirtual.DelayedCall(0.5f, () =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("Gameplay"); 
            });
        }
    }
}