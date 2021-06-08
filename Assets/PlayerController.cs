using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : TileController
{
    [SerializeField] private Health health = null;
    
    private Vector3Int direction = Vector3Int.zero;
    public override Vector3Int Direction{ get { return direction; } }
    private Animator animator = null;
    private ParticleSystem dust = null;
    public int CurrentHealth { get { return health.Current; } }
    public Transform Transform{ get{ return transform; } }

    private void Awake()
    {
        world.SetPlayer(this);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        dust = GetComponent<ParticleSystem>();
        var size = world.GetComponent<Grid>().cellSize;
        var anchor = world.GetComponent<Tilemap>().tileAnchor;
        var offset = new Vector3(size.x * anchor.x,
                                 size.y * anchor.y,
                                 size.z * anchor.z);
        transform.position = world.IndexToWorldPosition(new Vector2Int(3, 0)); // TODO: Replace Vector3.zero with spawn position
        RefreshInWorld(transform.position, transform.position);
    }



    void SetDirection(Vector3 dir)
    {
        direction.x = (int)(dir.x);
        direction.y = 0;
        direction.z = (int)(dir.z);
    }
    void SetDirection(int x, int z)
    {
        direction.x = x;
        direction.y = 0;
        direction.z = z;
    }
    void ResetDirection()
    {
        direction = Vector3Int.zero;
        animator.SetBool("Attacking", false);
        animator.SetBool("Moving", false);
    }

    void Update()
    {
        Vector3 dir = Vector3.zero;
        if(Input.GetKey(KeyCode.D)) {
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
        dir.Normalize();
        ResetDirection();
        SetDirection(dir);
        if (dir.magnitude <= 0.1f) { // Little bit of dead zone; it should be either 1 or 0. Math
            return; // Nothing happens
        }

        var target = world.WorldPositionToIndex(transform.position) + new Vector2Int(direction.x, direction.z);
        if(world.HasEnemy(target)) {
            animator.SetBool("Attacking", true);
            return;
        }
        else if (world.IsBlocked(target)) {
            ResetDirection();
        }
        animator.SetBool("Moving", direction.magnitude != 0.0f);
    }

    public override void Attack(Vector3 position)
    {
        var index = world.WorldPositionToIndex(position);
        if (world.HasEnemy(index)) {
            var enemy = world.Get(index).Visitor.GetComponent<TileController>();
            enemy.OnDamage(10);
        }
    }

    public override void OnDamage(int damage)
    {
        if (health.ReduceAndCheckDeath(damage)) {
            // Death
        }
    }

    public void PlayDust()
    {
        dust.Play();
    }

    public override void OnMoveFinish()
    {
        dust.Play();
    }
}
