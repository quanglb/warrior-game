using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Gameplay
{
    public abstract class ITroop : MonoBehaviour
    {
        public Team team;
        public bool isDead;
        public abstract void GetHit(uint _dmg);
        public abstract IEnumerator SetDead(bool val);
    }
}