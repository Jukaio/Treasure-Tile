using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* Components -> Controller -> Behaviour/States
 * The GameObject hold the data of the components and an Animator
 * The Animator acts as a State Machine, what it naturally is
 * The Components offer "API" that the Controller wraps and manages, i.e. Adapter or Facade
 * The Behaviour/States use the Controller API. The states and behaviours are dedicated updates
 * NOTE: The controller interface is the same, though the implementations differ, that enables 
 * the usage of the SAME animator and only the behaviours differ - cool eh? */
public abstract class TileController : MonoBehaviour
{
    // External components
    [SerializeField] protected WorldManager world = null;
    [SerializeField] private Dust dust = null;

    // Private components
    private Animator animator = null;
    private Health health = null;

    // Public components
    public Health HealthPoints => health;

    // Direction is dependent on the implementation
    public abstract Vector3Int Direction{ get; }
    // Direction to TileDirection or Board Direction
    public Vector2Int TileDirection => Direction.XZ();

    // Helpers
    public Vector3 Size { get { return world.TileSize; } }
    private Vector2Int current_index = Vector2Int.zero;
    public Vector2Int TileIndex => current_index;

    // "API"
    public void SetWorld(WorldManager that)
    {
        world = that;
    }
    public Vector3 Steps(Vector3Int velcoity)
    {
        var step = Size;
        step.x *= velcoity.x;
        step.y *= velcoity.y;
        step.z *= velcoity.z;
        return step;
    }
    public void RefreshInWorld(Vector3 previous, Vector3 current)
    {
        var prev = world.Get(world.WorldPositionToIndex(previous));
        current_index = world.WorldPositionToIndex(current);
        var curr = world.Get(current_index);
        prev.Visitor = null;
        curr.Visitor = gameObject;
    }
    protected virtual void OnAwake() 
    { }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        OnAwake();
    }
    public void RequestMove(Vector3 position)
    {
        var target = world.WorldPositionToIndex(position);
        world.Reserve(target);
    }

    public void PlayDust()
    {
        if(dust != null){
            dust.Play();
        }
    }

    public void SetAttacking(bool to)
    {
        animator.SetBool("Attacking", to);
    }
    public void SetMoving(bool to)
    {
        animator.SetBool("Moving", to);
    }

    // Interface to implement - The bones of the entity
    public abstract void OnAttack(Vector3 position);
    public abstract void OnDamage(int damage);

    public abstract void OnMoveFinish();
    public abstract void OnAttackFinish();
}

