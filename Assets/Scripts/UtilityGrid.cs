using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UtilityGrid<Resource> : MonoBehaviour
{
    public delegate Resource Assigner(Vector2Int index);
    public delegate void Receiver(Vector2Int index, Resource state);

    // Implement those in derived class to enjoy the full Interface
    public abstract Vector2Int Origin();
    public abstract Vector2Int Size();
    public abstract Resource Get(Vector2Int at);
    public abstract void Set(Vector2Int at, Resource that);
    public abstract bool IsSame(Resource a, Resource b);

    public bool IsInBounds(Vector2Int at) 
    {
        return at.x >= Origin().x && at.x < Size().x &&
               at.y >= Origin().y && at.y < Size().y;
    }

    public Vector2Int Clamp(Vector2Int of)
    {
        of.x = Mathf.Clamp(of.x, Origin().x, Size().x - 1);
        of.y = Mathf.Clamp(of.y, Origin().y, Size().y - 1);
        return of;
    }
    public int GetNeighbourCount(Vector2Int of, Resource compare)
    {
        int count = 0;
        ForEachNeigbhour(of, (Vector2Int index, Resource state) =>
        {
            count += IsSame(Get(index), compare) ? 1 : 0;
        });
        return count;
    }

    public void GetNeighbourhoodBounds(Vector2Int center, Vector2Int dimensions, out Vector2Int min, out Vector2Int max)
    {
        min = new Vector2Int(center.x - dimensions.x > Origin().x ? center.x - dimensions.x : Origin().x,
                             center.y - dimensions.y > Origin().y ? center.y - dimensions.y : Origin().y);

        max = new Vector2Int(center.x + dimensions.x < Size().x - 1 ? center.x + dimensions.x : Size().x - 1,
                             center.y + dimensions.y < Size().y - 1 ? center.y + dimensions.y : Size().y - 1);
    }
    public void GetNeighbourBounds(Vector2Int of, out Vector2Int min, out Vector2Int max)
    {
        GetNeighbourhoodBounds(of, Vector2Int.one, out min, out max);
    }

    public void ForEachNeighbourhood(Vector2Int index, Vector2Int dimension, Receiver receive)
    {
        GetNeighbourhoodBounds(index, dimension, out Vector2Int min, out Vector2Int max);
        for (int y = min.y; y <= max.y; y++) {
            for (int x = min.x; x <= max.x; x++) {
                if (x == index.x && y == index.y) {
                    continue; // Skip origin
                }
                receive(new Vector2Int(x, y), Get(new Vector2Int(x, y)));
            }
        }
    }
    public void ForEachNeighbourhood(Vector2Int index, Vector2Int dimension, Assigner assign)
    {
        GetNeighbourhoodBounds(index, dimension, out Vector2Int min, out Vector2Int max);
        for (int y = min.y; y <= max.y; y++) {
            for (int x = min.x; x <= max.x; x++) {
                if (x == index.x && y == index.y) {
                    continue; // Skip origin
                }
                Set(new Vector2Int(x, y), assign(new Vector2Int(x, y)));
            }
        }
    }

    public void ForEachNeigbhour(Vector2Int index, Receiver receive)
    {
        GetNeighbourhoodBounds(index, Vector2Int.one, out Vector2Int min, out Vector2Int max);
        for (int y = min.y; y <= max.y; y++) {
            for (int x = min.x; x <= max.x; x++) {
                if (x == index.x && y == index.y) {
                    continue; // Skip origin
                }
                receive(new Vector2Int(x, y), Get(new Vector2Int(x, y)));
            }
        }
    }
    public void ForEachNeigbhour(Vector2Int index, Assigner assign)
    {
        GetNeighbourhoodBounds(index, Vector2Int.one, out Vector2Int min, out Vector2Int max);
        for (int y = min.y; y <= max.y; y++) {
            for (int x = min.x; x <= max.x; x++) {
                if (x == index.x && y == index.y) {
                    continue; // Skip origin
                }
                Set(new Vector2Int(x, y), assign(new Vector2Int(x, y)));
            }
        }
    }
    public void ForEachOnEdge(Assigner assign)
    {
        int index = Origin().y;
        for (int x = 0; x < Size().x - 1; x++) {
            Set(new Vector2Int(x, index), assign(new Vector2Int(x, index)));
        }
        index = Size().x - 1;
        for (int y = 0; y < Size().y - 1; y++) {
            Set(new Vector2Int(index, y), assign(new Vector2Int(index, y)));
        }
        index = Size().y - 1;
        for (int x = Size().x - 1; x >= Origin().x; x--) {
            Set(new Vector2Int(x, index), assign(new Vector2Int(x, index)));
        }
        index = Origin().x;
        for (int y = Size().y - 1; y >= Origin().y; y--) {
            Set(new Vector2Int(index, y), assign(new Vector2Int(index, y)));
        }
    }

    public void ForEach(Assigner assign)
    {
        for (int y = 0; y < Size().y; y++) {
            for (int x = 0; x < Size().x; x++) {
                Set(new Vector2Int(x, y), assign(new Vector2Int(x, y)));
            }
        }
    }
    public void ForEach(Receiver receive)
    {
        for (int y = 0; y < Size().y; y++) {
            for (int x = 0; x < Size().x; x++) {
                receive(new Vector2Int(x, y), Get(new Vector2Int(x, y)));
            }
        }
    }

    private static bool[,] CreateEmptyBoolMatrix(bool value, Vector2Int size)
    {
        bool[,] matrix = new bool[size.x, size.y];

        for (int y = 0; y < size.y; y++) {
            for (int x = 0; x < size.x; x++) {
                matrix[x, y] = value;
            }
        }
        return matrix;
    }

    public List<Vector2Int> IdentifySpace(Vector2Int index, ref bool[,] visited, Resource compare)
    {
        List<Vector2Int> space = new List<Vector2Int>();

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(index);
        while (stack.Count > 0) {
            Vector2Int current = stack.Pop();
            if (visited[current.x, current.y]) {
                continue;
            }
            visited[current.x, current.y] = true;

            if (IsSame(Get(current), compare)) {
                space.Add(current);

                ForEachNeigbhour(current,
                (Vector2Int neighbour, Resource resource) =>
                {
                    stack.Push(neighbour);
                });
            }
        }
        return space;
    }

    public List<List<Vector2Int>> IdentifyAllSpaces(Resource to_continue, Resource to_break)
    {
        List<List<Vector2Int>> spaces = new List<List<Vector2Int>>();
        bool[,] visited = CreateEmptyBoolMatrix(false, Size());

        ForEach((Vector2Int index, Resource state) =>
        {
            if (IsSame(Get(index), to_break)) {
                return; // We only care about dead space
            }

            if (visited[index.x, index.y]) {
                return; // break out of lambda
            }

            spaces.Add(IdentifySpace(index, ref visited, to_continue));
        });
        return spaces;
    }

}

