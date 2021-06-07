using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Vector3Int direction = Vector3Int.zero;
    public Vector3Int Direction{ get { return direction; } }
    private Animator animator = null;
    private ParticleSystem dust = null;
    [SerializeField] private WorldManager world = null;

    public Vector3 Step { get { return world.TileSize; } }
    public Vector3 Steps(Vector3Int direction)
    {
        var step = Step;
        step.x *= direction.x;
        step.y *= direction.y;
        step.z *= direction.z;
        return step;
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
        transform.position = Vector3.zero + offset; // TODO: Replace Vector3.zero with spawn position
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
        var current = world.WorldPositionToIndex(transform.position);
        direction.x = (int)(dir.x);
        direction.z = (int)(dir.z);
        if (world.IsBlocked(current + new Vector2Int(direction.x, direction.z))) {
            direction.x = 0;
            direction.z = 0;
        }
        animator.SetBool("Moving", direction.magnitude != 0.0f);
    }

    Vector2Int current_world_index = Vector2Int.zero;
    public void RefreshPlayerInWorld(Vector3 previous, Vector3 current)
    {
        var prev = world.Get(world.WorldPositionToIndex(previous));
        var curr = world.Get(world.WorldPositionToIndex(current));
        current_world_index = world.WorldPositionToIndex(current);
        prev.Player = null;
        curr.Player = this;
    }

    public void PlayDust()
    {
        dust.Play();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) {
            var index = world.WorldPositionToIndex(transform.position);
            var at = world.IndexToWorldPosition(index);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(at, Step);
            at = world.IndexToWorldPosition(current_world_index);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(at, Step);
        }
    }
}
