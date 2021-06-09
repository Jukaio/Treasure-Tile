using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : TileController
{
    private Vector3Int direction = Vector3Int.zero;
    public override Vector3Int Direction => direction;
    private List<Vector2Int> path = null;

    public void SetPath(List<Vector2Int> that) {
        path = that;
        if(path?.Count <= 0) {
            direction = Vector3Int.zero;
            return;
        }
        direction = path[path.Count - 1].XZ();
        var dir = direction.XZ();
        if (world.Get(dir).IsReserved) { // On research means player is in a movement animation 
            direction = Vector3Int.zero; // Just delay this entity's move by a little
            return;
        }
        else if(world.HasPlayer(dir)) { 
            SetAttacking(true);
        }
        else {
            world.Reserve(dir);
            SetMoving(true);
        }
        direction -= world.WorldPositionToIndex(transform.position).XZ();
    }


    public override void OnMoveFinish()
    {
        world.Get(world.WorldPositionToIndex(transform.position)).Open();
        PlayDust();
        SetMoving(false);
    }
    public override void OnAttackFinish()
    {
        SetAttacking(false);
    }

    void Start()
    {
        transform.position = world.IndexToWorldPosition(new Vector2Int(5, 0)); // TODO: Replace Vector3.zero with spawn position
        RefreshInWorld(transform.position, transform.position);
    }

    public override void OnAttack(Vector3 position)
    {
        var index = world.WorldPositionToIndex(position);
        if (world.HasPlayer(index)) {
            var enemy = world.Get(index).Visitor.GetComponent<TileController>();
            enemy.OnDamage(10);
        }
    }
    public override void OnDamage(int damage)
    {
        if(HealthPoints.ReduceAndCheckDeath(damage)) {
            world.Kill(world.WorldPositionToIndex(transform.position));
            gameObject.SetActive(false);
        }
    }

    // TODO: Pathfinding component - system
    List<Vector2Int> FindShortestPath(Vector2Int from, Vector2Int to) 
    { 
        var player = world.Player;
        Vector2Int size = world.Size(); // 120

        float heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
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

        Queue<Vector2Int> open = new Queue<Vector2Int>();
        open.Enqueue(from);
        while(open.Count > 0) {  
            var current = open.Dequeue();
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
                    f[index.x, index.y] = cost + heuristic(index, to);
                    trace[index.x, index.y] = current;
                    open.Enqueue(index);
                }
            });
        }

        var next = to;
        if (next == new Vector2Int(int.MaxValue, int.MaxValue)) {
            return null;
        }

        int count = 0;
        List<Vector2Int> path = new List<Vector2Int>();
        while (next != from // Run while not full path collected
               && count < world.Size().magnitude // ... while not inifnite
               && next != new Vector2Int(int.MaxValue, int.MaxValue)) { // ... while not invalid
            path.Add(next);
            next = trace[next.x, next.y];
            count++;
        }
        return path;
    }


    public bool FoundShortestPathToPlayer(out List<Vector2Int> path)
    {
        var player = world.Player;
        path = FindShortestPath(world.WorldPositionToIndex(transform.position),
                                world.WorldPositionToIndex(player.transform.position));
        return path != null;
    }

}
