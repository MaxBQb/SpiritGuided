using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DefaultNamespace
{
    public interface Creature
    {
        [CanBeNull] GameObject spirit { get; set; }
        Vector3 offset { get; }
        DateTime lastOccupied { get; }
        void Move(Vector2 direction);
        void Look(Quaternion lookAngle);
        void MoveVertically(float direction);
        void Attack();
    }

    public static class MobExtensions
    {
        public static bool IsCreature(this GameObject other) => other.tag == "Creature";
        public static bool HasSpirit(this GameObject other) => other.GetControls()?.spirit != null;
        public static bool IsAvailable(this GameObject other) 
            => other.IsCreature() 
               && !other.HasSpirit() 
               && DateTime.Now.CompareTo(other.GetControls().lastOccupied + TimeSpan.FromSeconds(10.0)) >= 0;
        public static Creature GetControls(this GameObject other) => other.GetComponent<Creature>();
    }
}