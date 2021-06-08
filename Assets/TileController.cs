using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// standard behaviour for all dynamic "tiles"
public abstract class TileController : MonoBehaviour
{
    [SerializeField] protected WorldManager world = null;

    public abstract Vector3Int Direction{ get; }
    public Vector3 Step { get { return world.TileSize; } }

    public Vector3 Steps(Vector3Int direction)
    {
        var step = Step;
        step.x *= direction.x;
        step.y *= direction.y;
        step.z *= direction.z;
        return step;
    }
    public void RefreshInWorld(Vector3 previous, Vector3 current)
    {
        var prev = world.Get(world.WorldPositionToIndex(previous));
        var curr = world.Get(world.WorldPositionToIndex(current));
        prev.Visitor = null;
        curr.Visitor = gameObject;
    }

    public abstract void Attack(Vector3 position);
    public abstract void OnDamage(int damage);

    public abstract void OnMoveFinish();
}

