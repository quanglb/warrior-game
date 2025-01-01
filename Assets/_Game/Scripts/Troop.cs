using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Gameplay;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace _Game.Scripts
{
    public class Troop : ITroop
    {
        public TroopClass troopClass;

        public int initialHealth = 8;
        public HealthSystemForDummies healthBar;
        public ITroop target;
        [Header("Animation")] public SkeletonAnimation skeletonAnimation;

        [SerializeField] [SpineAnimation(dataField = "skeletonAnimation")]
        protected string runAnims;

        [SerializeField] [SpineAnimation(dataField = "skeletonAnimation")]
        protected string attackAnim;

        [SerializeField] [SpineAnimation(dataField = "skeletonAnimation")]
        protected string deadAnim;

        [SerializeField] [SpineAnimation(dataField = "skeletonAnimation")]
        protected string idleAnim;

        [Header("Stats")] [Tooltip("How many hits per second")]
        public float attackInterval = 1.5f;

        protected float attackAnimSpeed = 1;
        protected float attackTimer;
        public float attackRange;
        public uint dmg;
        public float aoeRadius;
        public uint AOEDmg;
        public float startingY;

        public float movementSpeed = .5f;
        public LayerMask castMask;

        [Header("Target Search")] [SerializeField]
        protected Vector2 boxSize = new Vector2(.5f, 8f);

        [SerializeField] protected float boxCastRange;

        private Rigidbody2D rigidbody2D;

        protected Vector3 invesrseX = new Vector3(-1, 1, 1);
        protected Vector3 sortPos;

        [Header("Status")] protected float freezeTimer;

        protected virtual void Start()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();

            Setup();
        }

        private void Setup()
        {
            sortPos = transform.position;
            sortPos.z = sortPos.y;
            transform.position = sortPos;

            healthBar.MaximumHealth = initialHealth;
            healthBar.CurrentHealth = initialHealth;

            isDead = false;
            attackTimer = attackInterval / 2;
            startingY = transform.position.y;
            skeletonAnimation.Initialize(false);
            attackAnimSpeed = skeletonAnimation.Skeleton.Data.FindAnimation(attackAnim).Duration / attackInterval;
        }

        protected virtual void FixedUpdate()
        {
            sortPos = transform.position;
            sortPos.z = sortPos.y;
            transform.position = sortPos;

            if (freezeTimer > 0)
            {
                freezeTimer -= Time.deltaTime;
                return;
            }

            attackTimer += Time.deltaTime;
            if (!isDead /* && !CheckLevelEnd*/)
            {
                FindClosestTarget();
                if (target == null)
                    MoveForward();
                else
                {
                    if (target.GetComponent<Tower>() == null)
                    {
                        if (Vector2.Distance(transform.position, target.transform.position) >
                            (attackRange + target.GetComponentInChildren<Renderer>().bounds.size.x / 3))
                            MoveToTarget();
                        else
                            Attack();
                    }
                    else
                    {
                        if (Mathf.Abs(transform.position.x - (Team.Player == team
                                ? target.GetComponent<Collider2D>().bounds.min.x
                                : target.GetComponent<Collider2D>().bounds.max.x)) >
                            attackRange + Random.Range(-.05f, .05f))
                            MoveToTarget();
                        else
                            Attack();
                    }
                }

                rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, movementSpeed);
            }
#if UNITY_EDITOR
            Debug.DrawLine(transform.position - transform.up * boxSize.y / 2,
                transform.position + transform.right * boxCastRange - transform.up * boxSize.y / 2 +
                transform.right * boxSize.x / 2, Color.red);
            Debug.DrawLine(transform.position + transform.up * boxSize.y / 2,
                transform.position + transform.right * boxCastRange + transform.up * boxSize.y / 2 +
                transform.right * boxSize.x / 2, Color.red);
#endif
        }

        protected virtual void MoveForward()
        {
            if (skeletonAnimation.AnimationState.GetCurrent(0) ==  null)
                skeletonAnimation.AnimationState.SetAnimation(0, runAnims, true);
            else
                skeletonAnimation.AnimationState.AddAnimation(0, runAnims, true, 0f);
            
            rigidbody2D.velocity = transform.right * movementSpeed;
            
            if (Mathf.Abs(transform.position.y - startingY) > .1f)
            {
                rigidbody2D.velocity = transform.up * Mathf.Sign(startingY - transform.position.y) * movementSpeed;
            }
        }

        protected void FindClosestTarget()
        {
            List<ITroop> potentialTargets = team == Team.Player
                ? LevelManager.Instance.enemies
                : LevelManager.Instance.players;

            potentialTargets = potentialTargets.Where(t => t != null && t.gameObject.activeInHierarchy).ToList();

            ITroop closestTarget = null;
            float closestDistanceSqr = float.MaxValue;

            Vector3 currentPosition = transform.position;

            foreach (var potentialTarget in potentialTargets)
            {
                float distanceSqr = (potentialTarget.transform.position - currentPosition).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestTarget = potentialTarget;
                }
            }

            target = closestTarget;
        }

        protected virtual void MoveToTarget()
        {
            if (skeletonAnimation.AnimationState.GetCurrent(0) == null)
                skeletonAnimation.AnimationState.SetAnimation(0, runAnims, true);
            else
                skeletonAnimation.AnimationState.AddAnimation(0, runAnims, true, 0f);
            
            Vector3 direction = (target.transform.position - transform.position + Vector3.up * Random.Range(-.1f, .1f))
                .normalized;
            //rigidbody2D.AddForce(direction * movementSpeed * 10f);
            rigidbody2D.velocity = direction * movementSpeed;
        }

        protected virtual void Attack()
        {
            rigidbody2D.velocity = Vector2.zero;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0;
                if (attackInterval <= skeletonAnimation.Skeleton.Data.FindAnimation(attackAnim).Duration)
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, attackAnim, false).TimeScale = attackAnimSpeed;

                    skeletonAnimation.AnimationState.AddAnimation(0, idleAnim, true, 0f);
                }
                else
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, attackAnim, false);
                    skeletonAnimation.AnimationState.AddAnimation(0, idleAnim, true, 0f);
                }

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    if (target != null)
                    {
                        target.GetHit(dmg);

                        var spawner = FindObjectOfType<DamageTextSpawner>();
                        if (spawner != null)
                        {
                            spawner.SpawnDamageText(target.transform.position, dmg);
                        }
                    }
                });
            }
        }

        public override void GetHit(uint _dmg)
        {
            if (isDead)
                return;

            healthBar.AddToCurrentHealth(-_dmg);
            // if (_dmg > HP)
            // {
            //     _dmg = HP;
            // }
            // if (!isPlayer)
            // {
            //     //Config.GOLD_COLLECTED += _dmg;
            //     BigDouble goldCollectValue = goldValue / maxHP * _dmg;
            //     CoinDropPool.Instance.Spawn(transform.position, (uint)goldCollectValue);
            //     Config.HIDDEN_GOLD_COLLECTED += goldCollectValue;
            // }
            // else
            // {
            //     LevelManager.Instance.PlayCameraShake(AssetsManager.Instance.damageHallShakePreset);
            // }
            // HP -= (uint)_dmg;
            // HPFill.fillAmount = (float)HP / (float)maxHP;
            // HPTxt.text = HP.ToString();
            //
            // if (HP <= 0)
            // {
            //     LevelManager.Instance.PlayCameraShake(AssetsManager.Instance.endGameShakePreset);
            //     isDead = true;
            //     var deadFX = Instantiate(AssetsManager.Instance.baseDead, transform.position, Quaternion.identity);
            //     deadFX.GetComponent<ParticleSystem>().Play();
            //     AudioManager.Instance.PlaySound2D(Sounds.BaseDead);
            //     EventDispatcher.Instance.PostEvent(EventID.EndLevel, !isPlayer);
            //     Destroy(gameObject);
            // }
        }

        public override IEnumerator SetDead(bool val)
        {
            isDead = val;
            if (team == Team.Enemy)
            {
                var goldSpawner = FindObjectOfType<GoldSpawner>();
                if (goldSpawner != null)
                {
                    int goldAmount = Random.Range(1, 10);
                    goldSpawner.SpawnGold(transform.position, goldAmount);
                }
            }

            LevelManager.Instance.Remove(this);

            var entry = skeletonAnimation.AnimationState.SetAnimation(0, deadAnim, false);

            while (entry != null && !entry.IsComplete)
            {
                yield return null;
            }

            Destroy(gameObject);

            Debug.Log($"<color=red>{gameObject.name}</color> Is Dead = {val}");
        }
    }
}