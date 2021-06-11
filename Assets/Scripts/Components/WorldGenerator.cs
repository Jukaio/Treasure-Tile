using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldGenerator : UtilityGrid<WorldGenerator.State>
{
    public enum State
    {
        dead,
        alive
    }

    [SerializeField] [Range(0, 256)] private int width = 128;
    [SerializeField] [Range(0, 256)] private int height = 128;
    [SerializeField] [Range(0.0f, 1.0f)] private float fillAmountRatio = 0.5f;
    [SerializeField] [Range(0, 20)] private int stepCount = 6;

    private Vector2Int size;
    private State[,] data;
    private State[,] buffer;

    void Awake()
    {
        size = new Vector2Int(width, height);
        Initialise();
        Generate();
    }
    public void Initialise()
    {
        data = new State[size.x, size.y];
        buffer = new State[size.x, size.y];
        ForEach((Vector2Int index) =>
        {
            buffer[index.x, index.y] = Random.Range(0.0f, 1.0f) < fillAmountRatio ? State.dead : State.alive;
            return buffer[index.x, index.y];
        });
    }
    /* Source: https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm */
    private void CarvePath(Vector2Int from, Vector2Int to, int width) /* TODO: explain width - 
                                                                           * width indicates the "circles" around the origin
                                                                           * width of 1 = line thickness of 3
                                                                           * width of 2 = line thickness of 5 */
    {
        from = Clamp(from);
        to = Clamp(to);

        int dx = Mathf.Abs(to.x - from.x);
        int sx = from.x < to.x ? 1 : -1;
        int dy = -Mathf.Abs(to.y - from.y);
        int sy = from.y < to.y ? 1 : -1;
        int err = dx + dy;  /* error value e_xy */

        while (true) {
            data[from.x, from.y] = State.dead;

            ForEachNeighbourhood(from, Vector2Int.one * width, (Vector2Int neighbour) =>
            {
                return State.dead;
            });

            if (from == to) {
                break;
            }
            int e2 = 2 * err;
            if (e2 >= dy) {
                err += dy;
                from.x += sx;
            }
            if (e2 <= dx) {
                err += dx;
                from.y += sy;
            }
        }
    }

    private void Optimise() // Could make WorldOptimiser
    {
        List<List<Vector2Int>> spaces = IdentifyAllSpaces(State.dead, State.alive);
        for (int i = 0; i < spaces.Count - 1; i++) {
            var from = spaces[i];
            var to = spaces[i + 1];
            CarvePath(from[0], to[0], 1);
        }
        ForEach((Vector2Int index, State state) =>
        {
            bool[,] layout = {
                { false, false, false },
                { false, false, false },
                { false, false, false }
            };
            if (state == State.alive) {
                ForEachNeigbhour(index, (Vector2Int neighbour, State other) =>
                {
                    int x = neighbour.x - index.x + 1;
                    int y = neighbour.y - index.y + 1;
                    layout[x, y] = State.alive == other;
                });

                if(                 layout[1, 0] &&
                 !layout[0, 1] && /*layout[1, 1] && */ !layout[2, 1] &&
                                    layout[1, 2]) {
                    Set(index, State.dead);
                }
                else if (                  !layout[1, 0] &&
                         layout[0, 1] && /*layout[1, 1] && */ layout[2, 1] &&
                                           !layout[1, 2]) {
                    Set(index, State.dead);
                }
                else if (                   layout[1, 0] &&
                         !layout[0, 1] && /*layout[1, 1] && */ !layout[2, 1] &&
                                            !layout[1, 2]) {
                    Set(index, State.dead);
                }
                else if (                   !layout[1, 0] &&
                         !layout[0, 1] && /*layout[1, 1] && */ layout[2, 1] &&
                                            !layout[1, 2]) {
                    Set(index, State.dead);
                }
                else if (                   !layout[1, 0] &&
                         !layout[0, 1] && /*layout[1, 1] && */ !layout[2, 1] &&
                                            layout[1, 2]) {
                    Set(index, State.dead);
                }
                else if (                   !layout[1, 0] &&
                         layout[0, 1] && /*layout[1, 1] && */ !layout[2, 1] &&
                                            !layout[1, 2]) {
                    Set(index, State.dead);
                }
            }
        });
    }
    private void CopyFromBufferToData()
    {
        ForEach((Vector2Int index, State state) =>
        {
            data[index.x, index.y] = buffer[index.x, index.y];
        });
    }
    private void Step()
    {
        ForEach((Vector2Int index, State state) =>
        {
            int count = GetNeighbourCount(index, State.alive);
            if (count > 4) {
                buffer[index.x, index.y] = State.alive;
            }
            else if (count < 4) {
                buffer[index.x, index.y] = State.dead;
            }
        });
        CopyFromBufferToData();
    }
    public void Generate()
    {
        for (int i = 0; i < stepCount; i++) {
            Step();
        }
        Optimise();
    }
    public override State Get(Vector2Int at)
    {
        return data[at.x, at.y];
    }

    public override bool IsSame(State a, State b)
    {
        return a == b;
    }

    public override Vector2Int Origin()
    {
        return Vector2Int.zero;
    }

    public override void Set(Vector2Int at, State that)
    {
        data[at.x, at.y] = that;
    }

    public override Vector2Int Size()
    {
        return size;
    }
}

