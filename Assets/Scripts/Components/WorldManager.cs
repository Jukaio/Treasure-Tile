using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class WorldManager : UtilityGrid<Tile3D>
{
    private Grid grid;

    private Tilemap tilemap;
    private WorldGenerator generator;

    [SerializeField] private UIManager ui = null;
    [SerializeField] private GameObject enemy = null;

    // We could do this smarter - Let's do it like that first
    [SerializeField] private Tile3D ground = null;
    [SerializeField] private Tile3D wall = null;
    [SerializeField] private Tile3D north_east_corner_wall = null;
    [SerializeField] private Tile3D north_west_corner_wall = null;
    [SerializeField] private Tile3D south_east_corner_wall = null;
    [SerializeField] private Tile3D south_west_corner_wall = null;
    [SerializeField] private Tile3D north_east_inner_corner_wall = null;
    [SerializeField] private Tile3D north_west_inner_corner_wall = null;
    [SerializeField] private Tile3D south_east_inner_corner_wall = null;
    [SerializeField] private Tile3D south_west_inner_corner_wall = null;
    [SerializeField] private Tile3D north_half_wall = null;
    [SerializeField] private Tile3D south_half_wall = null;
    [SerializeField] private Tile3D east_half_wall = null;
    [SerializeField] private Tile3D west_half_wall = null;
    [SerializeField] private Tile3D stone = null;

    public PlayerController Player { get; private set; }
    // To make it VERY explicit - SetPlayer
    public void SetPlayer(PlayerController that) 
    {
        Player = that;
    }

    public Vector3 TileSize{ get { return grid.cellSize; } }

    public Vector2Int WorldPositionToIndex(Vector3 at)
    {
        var size = grid.cellSize;
        var anchor = tilemap.tileAnchor;
        var offset = new Vector3(size.x * anchor.x,
                                 size.y * anchor.y,
                                 size.z * anchor.z);
        at -= offset;
        return new Vector2Int((int)(at.x / size.x), (int)(at.z / size.z));
    }

    public Vector3 IndexToWorldPosition(Vector2Int index)
    {
        var size = grid.cellSize;
        var anchor = tilemap.tileAnchor;
        var offset = new Vector3(size.x * anchor.x,
                                 size.y * anchor.y,
                                 size.z * anchor.z);
        var at = new Vector3(size.x * index.x, 0, size.z * index.y);
        return at + offset;
    }

    public void KillEnemy(Vector2Int at) 
    {
        if (IsInBounds(at)) {
            var that = Get(at);
            ui.Return(that.Visitor.GetComponent<EnemyController>());
            that.Visitor = null;
            that.Open();
        }
    }
    public void Open(Vector2Int at)
    {
        if (IsInBounds(at)) {
            var that = Get(at);
            that.Open();
        }
    }
    public void Reserve(Vector2Int at)
    {
        if (IsInBounds(at)) {
            var that = Get(at);
            that.Reserve();
        }
    }
    public bool IsBlocked(Vector2Int at) 
    {
        if(IsInBounds(at)) {
            var that = Get(at);
            return that.IsBlocked;
        }
        return true;
    }
    public bool HasEnemy(Vector2Int at)
    {
        if (IsInBounds(at)) {
            var that = Get(at);
            return that.HasEnemy;
        }
        return false;
    }
    public bool HasPlayer(Vector2Int at)
    {
        if (IsInBounds(at)) {
            var that = Get(at);
            return that.HasPlayer;
        }
        return false;
    }

    public override Tile3D Get(Vector2Int at) 
    {
        return tilemap.GetTile<Tile3D>((Vector3Int)at);
    }

    public override bool IsSame(Tile3D a, Tile3D b)
    {
        return a == b;
    }

    public override Vector2Int Origin()
    {
        return (Vector2Int)tilemap.origin;
    }

    public override void Set(Vector2Int at, Tile3D that)
    {
        tilemap.SetTile((Vector3Int)at, that);
    }

    public override Vector2Int Size()
    {
        return (Vector2Int)tilemap.size;
    }

    Tile3D BoolLayoutToTile(bool[,] layout)
    {
        if (                layout[1, 0] && !layout[2, 0] &&
            layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                            layout[1, 2]) {
            return south_east_inner_corner_wall;
        }
        else if (!layout[0, 0] && layout[1, 0] &&
                 layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                 layout[1, 2]) {
            return south_west_inner_corner_wall;
        }
        else if (                layout[1, 0] &&
                 layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                 !layout[0, 2] && layout[1, 2]) {
            return north_west_inner_corner_wall;
        }
        else if (               layout[1, 0] &&
                layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                                layout[1, 2] && !layout[2, 2]) {
            return north_east_inner_corner_wall;
        } // wall
        else if (                layout[1, 0] &&
                 layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                                 layout[1, 2]) {
            return wall;
        } // outer corners
        else if (                !layout[1, 0] &&
                 layout[0, 1] && layout[1, 1] && !layout[2, 1] &&
                                  layout[1, 2]) {
            return south_east_corner_wall;
        }
        else if (!layout[1, 0] &&
                 !layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                                 layout[1, 2]) {
            return south_west_corner_wall;
        }
        else if (layout[1, 0] &&
                 layout[0, 1] && layout[1, 1] && !layout[2, 1] &&
                                  !layout[1, 2]) {
            return north_east_corner_wall;
        }
        else if (layout[1, 0] &&
                 !layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                                 !layout[1, 2]) {
            return north_west_corner_wall;
        } // Walls
        else if (!layout[1, 0] &&
                 layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                                 layout[1, 2]) {
            return south_half_wall;
        }
        else if (layout[1, 0] &&
                 layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                                 !layout[1, 2]) {
            return north_half_wall;
        }
        else if (layout[1, 0] &&
                 layout[0, 1] && layout[1, 1] && !layout[2, 1] &&
                                 layout[1, 2]) {
            return east_half_wall;
        }
        else if (layout[1, 0] &&
                 !layout[0, 1] && layout[1, 1] && layout[2, 1] &&
                                  layout[1, 2]) {
            return west_half_wall;
        }
        else if (!layout[0, 0] && !layout[1, 0] && !layout[2, 0] &&
                 !layout[0, 1] && layout[1, 1] && !layout[2, 1] &&
                 !layout[0, 2] && !layout[1, 2] && !layout[2, 2]) {
            return stone;
        }
        else if (layout[0, 0] && !layout[1, 0] && !layout[2, 0] &&
                 !layout[0, 1] && layout[1, 1] && !layout[2, 1] &&
                 !layout[0, 2] && !layout[1, 2] && layout[2, 2]) {
            return stone;
        }
        else if (!layout[0, 0] && !layout[1, 0] && layout[2, 0] &&
                 !layout[0, 1] && layout[1, 1] && !layout[2, 1] &&
                 layout[0, 2] && !layout[1, 2] && !layout[2, 2]) {
            return stone;
        }
        if (layout[1, 1]) {
            return stone;
        }
        else if (!layout[1, 1]) {
            return ground;
        }
        return null;
    }

    Tile3D EvaluateTileConsideringNeighbours(Vector2Int index, WorldGenerator.State state)
    {
        bool[,] layout = {
                { false, false, false },
                { false, false, false },
                { false, false, false }
            };
        layout[1, 1] = WorldGenerator.State.alive == state;

        generator.ForEachNeigbhour(index,
        (Vector2Int neighbour, WorldGenerator.State n_state) =>
        {
            int x = neighbour.x - index.x + 1;
            int y = neighbour.y - index.y + 1;
            layout[x, y] = WorldGenerator.State.alive == n_state;
        });
        return BoolLayoutToTile(layout);
    }

    void CreateBordersAroundLevel()
    {
        ForEachOnEdge((Vector2Int index) => 
        {
            return wall; //EvaluateTileConsideringNeighbours(index, generator.Get(index));
        });

    }

    void LoadGeneratedLevel()
    {
        generator.ForEach((Vector2Int index, WorldGenerator.State state) =>
        {
            Set(index, Tile3D.Copy(EvaluateTileConsideringNeighbours(index, state)));
        });
    }

    private void SpawnEnemy(Vector2Int at)
    {
        enemy = Instantiate(enemy);
        var controller = enemy.GetComponent<EnemyController>();
        controller.SetWorld(this);
        controller.Spawn(IndexToWorldPosition(at));
        var element = ui.Request(controller);
        element.GetComponent<EnemyHealthUI>().Register(controller.HealthPoints);
        element.SetActive(true);
        element.GetComponent<EnemyHealthUI>().SetCanvas(ui.gameObject);
    }

    // Start is called before the first frame update
    void Awake()
    {
        grid = GetComponent<Grid>();
        tilemap = GetComponent<Tilemap>();
        generator = GetComponent<WorldGenerator>();

        LoadGeneratedLevel();
    }

    private void Start()
    {
        SpawnEnemy(new Vector2Int(5, 0));
 
    }


    [SerializeField] private bool debug_generator = false;
    private void OnDrawGizmosSelected()
    {
        if(Application.isPlaying) {
            if(debug_generator) {
                ForEach((Vector2Int index, Tile3D tile) =>
                {
                    if(tile.Visitor != null) {
                        if (tile.HasPlayer) {
                            Gizmos.color = Color.cyan;
                        } 
                        else {
                            Gizmos.color = Color.yellow;
                        }
                    }
                    else if (tile.IsReserved) {
                        Gizmos.color = Color.magenta;
                    }
                    else {
                        Gizmos.color = tile.IsBlocked ? Color.green : Color.red;
                    }
                    var offset = new Vector3(grid.cellSize.x * tilemap.tileAnchor.x, 
                                             grid.cellSize.y * tilemap.tileAnchor.y, 
                                             grid.cellSize.z * tilemap.tileAnchor.z);
                    Gizmos.DrawWireCube(tilemap.CellToWorld((Vector3Int)index) + offset, grid.cellSize);
                });
                return;
            }
        }
    }
}
