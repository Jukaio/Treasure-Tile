using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : TileController
{
    private Animator animator = null;

    private Vector3Int direction = Vector3Int.zero;
    public override Vector3Int Direction => direction;
    public override void OnMoveFinish()
    {

    }

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", true);
        var size = world.GetComponent<Grid>().cellSize;
        var anchor = world.GetComponent<Tilemap>().tileAnchor;
        var offset = new Vector3(size.x * anchor.x,
                                 size.y * anchor.y,
                                 size.z * anchor.z);
        transform.position = world.IndexToWorldPosition(new Vector2Int(5, 0)); // TODO: Replace Vector3.zero with spawn position
        RefreshInWorld(transform.position, transform.position);
    }

    public override void Attack(Vector3 position)
    {
        var index = world.WorldPositionToIndex(position);
        if (world.HasPlayer(index)) {
            world.Get(index).Visitor.SetActive(false);
        }
    }
    public override void OnDamage(int damage)
    {
        world.Kill(world.WorldPositionToIndex(transform.position));
        gameObject.SetActive(false);
    }

    List<Vector2Int> FindPlayer(Vector2Int from, Vector2Int to) 
    {
        var player = world.Player;
        Vector2Int size = world.Size(); // 120

        float heuristic(Vector2Int a, Vector2Int b)
        {
            return Vector2Int.Distance(a, b);
            //return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        };

        float[,] f = new float[size.x, size.y];
        float[,] g = new float[size.x, size.y];
        Vector2Int[,] trace = new Vector2Int[size.x, size.y];
        bool[,] visited = new bool[size.x, size.y];

        world.ForEach((Vector2Int index, Tile3D content) =>
        {
            f[index.x, index.y] = float.MaxValue;
            g[index.x, index.y] = float.MaxValue;
            trace[index.x, index.y] = new Vector2Int(int.MaxValue, int.MaxValue);
            visited[index.x, index.y] = false;
        });

        f[from.x, from.y] = 0.0f;
        g[from.x, from.y] = heuristic(from, to);

        Stack<Vector2Int> open = new Stack<Vector2Int>();
        open.Push(from);
        while(open.Count > 0) {
            var current = open.Pop();
            if(current == to) {
                break;
            }
 
            visited[current.x, current.y] = true;
            world.ForEachNeumannNeighbour(current, (Vector2Int index, Tile3D content) =>
            {    
                if(visited[index.x, index.y] || content.IsBlocked) {
                    return;
                }

                float cost = g[current.x, current.y] + heuristic(current, index);
                if(cost < g[index.x, index.y]) {
                    g[index.x, index.y] = cost;
                    f[index.x, index.y] = cost * heuristic(index, to);
                    trace[index.x, index.y] = current;
                    open.Push(index);
                }
            });
        }

        var next = to;
        int count = 0;
        List<Vector2Int> path = new List<Vector2Int>();
        while (next != from && count < 10000) {
            path.Add(next);
            next = trace[next.x, next.y];
            count++;
        }
        return path;
    }


    // Update is called once per frame
    void Update()
    {
        var player = world.Player;
    }
    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) {
            return;
        }

        var player = world.Player;
        var path = FindPlayer(world.WorldPositionToIndex(player.transform.position),
                              world.WorldPositionToIndex(transform.position));
        Gizmos.color = Color.magenta;
        foreach(var index in path) {
            var full = world.IndexToWorldPosition(index);
            Gizmos.DrawCube(full, world.TileSize);
        }
    }
}
