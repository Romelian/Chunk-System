using UnityEngine;

/// <summary>
/// Settings that don't change chunk to chunk
/// </summary>
[System.Serializable]
public class ChunkSettings
{
    public Vector3 size = new Vector3(100, 100, 100);

    public Vector3Int worldPosToCoord(Vector3 pos)
    {
        return new Vector3Int(Mathf.FloorToInt(pos.x / size.x), Mathf.FloorToInt(pos.y / size.y), Mathf.FloorToInt(pos.z / size.z));
    }
    public Vector3 coordToWorldPos(Vector3Int pos)
    {
        return new Vector3(pos.x * size.x, pos.y * size.x, pos.z * size.z);
    }
    public Vector3 worldPosToWorldPosCenter(Vector3 pos)
    {
        return pos + size / 2;
    }
    public Vector3 coordToWorldPosCenter(Vector3Int pos)
    {
        return worldPosToWorldPosCenter(coordToWorldPos(pos));
    }
}