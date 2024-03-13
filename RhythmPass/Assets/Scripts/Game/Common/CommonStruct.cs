using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public struct RayResult
    {
        public WorldCamera WorldCamera;
        public Vector3 HitPosition;
        public Vector2Int HitIndex;
        public TileObject HitTile;
        public RayResult(WorldCamera camera, TileObject tile, Vector3 hitPos, Vector2Int hitIndex)
        {
            WorldCamera = camera;
            HitTile = tile;
            HitPosition = hitPos;
            HitIndex = hitIndex;
        }
    }
    public readonly struct DamageInfo
    {
        public readonly WorldObject Attacker;
        public readonly WorldCharacter Victim;
        public readonly int Damage;
        public DamageInfo(WorldObject attacker, WorldCharacter victim, int damage)
        {
            Attacker = attacker;
            Victim = victim;
            Damage = damage;
        }
        public bool IsValid()
        {
            if (null == Attacker || null == Victim)
                return false;
            return true;
        }
        public static DamageInfo Create(WorldObject attacker, WorldCharacter victim, int damage)
        {
            return new DamageInfo(attacker, victim, damage);
        }
    }
}