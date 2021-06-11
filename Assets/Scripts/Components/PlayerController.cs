using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : TileController
{
    private Vector3Int direction = Vector3Int.zero;
    public override Vector3Int Direction { get { return direction; } }
    

    protected override sealed void OnAwake()
    {
        world.SetPlayer(this);
    }

    void Start()
    {
        transform.position = world.IndexToWorldPosition(new Vector2Int(3, 0)); // TODO: Replace Vector3.zero with spawn position
        RefreshInWorld(transform.position, transform.position);
    }

    public void SetDirection(Vector3 dir)
    {
        direction.x = (int)(dir.x);
        direction.y = 0;
        direction.z = (int)(dir.z);
    }

    public void ResetStatesAndInput()
    {
        direction = Vector3Int.zero;
        SetAttacking(false);
        SetMoving(false);
    }

    private void HandleInput()
    {
        ResetStatesAndInput();
        Vector3 dir = NormalisedInputDirection();
        SetDirection(dir);
    }

    private Vector3 NormalisedInputDirection()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.D)) {
            dir += Vector3.right;
        }
        if (Input.GetKey(KeyCode.A)) {
            dir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.W)) {
            dir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            dir += Vector3.back;
        }
        return dir.normalized;
    }

    private bool NoUserInput()
    {
        return Direction == Vector3Int.zero;
    }

    void ResolveNextPosition(Vector2Int current, Vector2Int dir)
    {
        var target = current + dir;
        if (world.HasEnemy(target)) {
            SetAttacking(true);
            return;
        }
        else if (world.IsBlocked(target)) {
            ResetStatesAndInput();
        }
        else if (world.Get(target).IsReserved) {
            ResetStatesAndInput();
        }
        SetMoving(direction.magnitude != 0.0f);
    }

    void Update() // This should be PlayerIdle state, since we only need input handling in this one state
    {
        HandleInput();
        if (NoUserInput()) {
            return; // Nothing happens
        }
        ResolveNextPosition(TileIndex, TileDirection);
    }

    public override void OnAttack(Vector3 position)
    {
        var index = world.WorldPositionToIndex(position);
        if (world.HasEnemy(index)) {
            var enemy = world.Get(index).Visitor.GetComponent<TileController>();
            enemy.OnDamage(10);
        }
    }

    public override void OnDamage(int damage)
    {
        if (HealthPoints.ReduceAndCheckDeath(damage)) {
            gameObject.SetActive(false);
            // Death
        }
    }

    public override void OnMoveFinish()
    {
        world.Open(world.WorldPositionToIndex(transform.position));
        PlayDust();
    }

    public override void OnAttackFinish()
    {
        
    }
}
