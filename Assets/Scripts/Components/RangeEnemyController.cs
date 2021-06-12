using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RangeEnemyController : EnemyController // This kind of inheritance hierarchy is certainly... not good
{                                                   // But in the scope of this little project it is efficient :) 
    [SerializeField] int range = 3;

    public Vector3 Target { get; private set; }

    public override void SetPath(List<Vector2Int> that) {
        path = that;
        if(path?.Count <= 0) {
            direction = Vector3Int.zero;
            return;
        }
        direction = path[path.Count - 1].XZ();
        var dir = direction.XZ();

        var player_tile_index = path[0];
        Target = world.IndexToWorldPosition(player_tile_index);
        var distance = TileIndex.Manhattan(player_tile_index);
        if(distance <= range) {
            if (world.HasPlayer(player_tile_index)) {
                SetAttacking(true);
            }
        }
        else if (world.Get(dir).IsReserved) { 
            direction = Vector3Int.zero; 
            return;
        }
        else {
            world.Reserve(dir);
            SetMoving(true);
        }
        direction -= world.WorldPositionToIndex(transform.position).XZ();
    }


}
