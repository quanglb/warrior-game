using System.ComponentModel;
using Spine.Unity;
using UnityEngine;

namespace _Game.Scripts
{
    public class Unit : MonoBehaviour
    {
        public Team team;
        public TroopClass troopClass;

        public int initialHealth = 8;
        public HealthSystemForDummies healthBar;
        public Unit target;
        [Header("Animation")] public SkeletonAnimation skeletonAnimation;

        [SerializeField] [SpineAnimation(dataField = "skeletonAnimation")]
        protected string runAnims;

        [SerializeField] [SpineAnimation(dataField = "skeletonAnimation")]
        protected string attackAnim;

        [SerializeField] [SpineAnimation(dataField = "skeletonAnimation")]
        protected string deadAnim;

        [Header("Stats")] [Tooltip("How many hits per second")]
        public float attackInterval = 1.5f;

        protected float attackAnimSpeed = 1;
        protected float attackTimer;
        public float attackRange;
        public float aoeRadius;
        public uint AOEDmg;
        public float startingY;
        public bool isDead;
        public float movementSpeed = .5f;
        public LayerMask castMask;

        [Header("Target Search")] [SerializeField]
        protected Vector2 boxSize = new Vector2(.5f, 8f);

        [SerializeField] protected float boxCastRange;

        private Rigidbody2D rigidbody2D;

        protected Vector3 invesrseX = new Vector3(-1, 1, 1);
        protected Vector3 sortPos;

        [Header("Status")] protected float freezeTimer;

        private void Start()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
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
            if (skeletonAnimation.AnimationState.GetCurrent(0) ==
                null /* || skeletonAnimation.AnimationState.GetCurrent(0).Animation.Name == idleAnims[0]*/)
                skeletonAnimation.AnimationState.SetAnimation(0, runAnims, true);
            else
                skeletonAnimation.AnimationState.AddAnimation(0, runAnims, true, 0f);
            //rigidbody2D.AddForce(transform.right * movementSpeed * 10f);
            rigidbody2D.velocity = transform.right * movementSpeed;
            if (Mathf.Abs(transform.position.y - startingY) > .1f)
            {
                //rigidbody2D.AddForce(transform.up * Mathf.Sign(startingY - transform.position.y) * movementSpeed * 10f);
                rigidbody2D.velocity = transform.up * Mathf.Sign(startingY - transform.position.y) * movementSpeed;
            }
        }

        protected void FindClosestTarget()
        {
            var _tempRange = Mathf.Infinity;
            RaycastHit2D[] raycastHit2DArray = new RaycastHit2D[5];
            if (transform == null) return;
            Physics2D.BoxCastNonAlloc(transform.position - transform.right * 2, boxSize, 0, transform.right,
                raycastHit2DArray, boxCastRange, castMask);
            if (raycastHit2DArray[0].transform == null)
            {
                target = null;
                return;
            }

            foreach (RaycastHit2D raycastHit in raycastHit2DArray)
            {
                if (raycastHit.transform == null)
                    break;
                if (Vector2.Distance(transform.position, raycastHit.transform.position) +
                    (Vector2.up * Mathf.Abs(transform.position.y - raycastHit.transform.position.y)).magnitude <
                    _tempRange)
                {
                    _tempRange = Vector2.Distance(transform.position, raycastHit.transform.position);
                    target = raycastHit.transform.GetComponent<Unit>();
                }
            }
        }

        protected virtual void MoveToTarget()
        {
            if (skeletonAnimation.AnimationState.GetCurrent(0) ==
                null /*|| skeletonAnimation.AnimationState.GetCurrent(0).Animation.Name == idleAnims[0]*/)
                skeletonAnimation.AnimationState.SetAnimation(0, runAnims, true);
            else
                skeletonAnimation.AnimationState.AddAnimation(0, runAnims, true, 0f);
            Vector3 direction;
            direction = (target.transform.position - transform.position + Vector3.up * Random.Range(-.1f, .1f))
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
                    skeletonAnimation.AnimationState.SetAnimation(0, attackAnim, false).TimeScale = attackAnimSpeed;
                else
                    skeletonAnimation.AnimationState.SetAnimation(0, attackAnim, false);
            }
        }
    }
}