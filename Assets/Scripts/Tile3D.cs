using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile3D : TileBase
{
    public Tile3D()
    {

    }
    public Tile3D(Tile3D other)
    {
        this.gameObject = other.gameObject;
        this.blocked = other.blocked;
        this.orientation = other.orientation;
        this.prefab = other.prefab;
        this.visitor = other.visitor;
    }

    public enum Direction
    {
        North,
        East,
        South,
        West,
    }
    //public float rotation;
    [SerializeField] private Direction orientation = Direction.North;
    [SerializeField] private bool blocked = false;

    public GameObject gameObject{ get { return prefab; } set { prefab = value; } }
    public Matrix4x4 transform { get { return transformation; } set { transformation = value; } }
    public bool IsBlocked { get { return blocked; } }

    private int damage;
    public int Damage => damage;
    public void SetDamage(int value)
    {
        damage = Mathf.Clamp(value, 0, int.MaxValue);
    }


    private GameObject visitor = null;
    public GameObject Visitor { get { return visitor; } set { visitor = value; } }

    public bool TryGetPlayer(out PlayerController player)
    {
        if (visitor == null) {
            player = null;
            return false;
        }
        return visitor.TryGetComponent<PlayerController>(out player);
    }
    public bool TryGetController(out TileController tile)
    {
        if(visitor == null) {
            tile = null;
            return false;
        }
        return visitor.TryGetComponent<TileController>(out tile);
    }

    public bool HasPlayer 
    { 
        get 
        {
            if(visitor == null) {
                return false;
            }
            return visitor.GetComponent<PlayerController>() != null; 
        } 
    }
    public bool HasEnemy
    {
        get
        {
            if (visitor == null) {
                return false;
            }
            return visitor.GetComponent<EnemyController>() != null;
        }
    }


    private float DirectionToAngle(Direction dir)
    {
        switch (dir) {
            case Direction.North:
                return 0.0f;
            case Direction.East:
                return 90.0f;
            case Direction.South:
                return 180.0f;
            case Direction.West:
                return 270.0f;
        }
        return 0.0f;
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {

        go.transform.Rotate(Quaternion.Euler(0.0f, DirectionToAngle(orientation), 0.0f).eulerAngles);
        return base.StartUp(position, tilemap, go);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = null;
        tileData.color = Color.white;
        tileData.gameObject = gameObject;
        tileData.transform = transformation;// * rotate_matrix;
        tileData.flags = TileFlags.InstantiateGameObjectRuntimeOnly;
        tileData.colliderType = Tile.ColliderType.None; // No colliders allowed

        base.GetTileData(position, tilemap, ref tileData);
    }

    private Matrix4x4 transformation = Matrix4x4.identity;
    [SerializeField] private GameObject prefab;
}
