using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public enum LoaderType
    {
        Dimension3,
        Dimension2
    }
    public LoaderType loaderType = LoaderType.Dimension3;
    public int radius = 2;
    public ChunkSettings chunkSettings;
    [Tooltip("Copy")]
    public GameObject chunkObject;
    [Tooltip("Fallback is self")]
    public Transform parent;
    [Tooltip("Fallback is main camera")]
    public Transform viewer;
    [HideInInspector]
    public Vector3Int viewerCoord;
    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    private List<Chunk> visibleChunks = new List<Chunk>();

    [Tooltip("Editor Only")]
    public bool showChunkBorder = false;
    void Awake()
    {
        if (viewer == null && Camera.main != null)
            viewer = Camera.main.transform;
        if (parent == null)
            parent = transform;
    }
    void Start()
    {
        viewerCoord = chunkSettings.worldPosToCoord(viewer.position);
        radiusUpdate();
    }

    void Update()
    {
        var newViewerCoord = chunkSettings.worldPosToCoord(viewer.position);
        if (!newViewerCoord.Equals(viewerCoord))
        {
            viewerCoord = newViewerCoord;
            visibleUpdate();
            radiusUpdate();
        }
    }
    void radiusUpdate()
    {
        var radius_y = loaderType == LoaderType.Dimension3 ? radius : 0;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius_y; y <= radius_y; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    var coord = new Vector3Int(x, y, z) + viewerCoord;

                    if (chunks.ContainsKey(coord))
                    {
                        if (visibleCheckAndSet(chunks[coord]) && !visibleChunks.Contains(chunks[coord]))
                            visibleChunks.Add(chunks[coord]);
                    }
                    else if (visibleCheck(coord))
                    {
                        Chunk chunk;
                        if (chunkObject == null)
                            chunk = Chunk.Create(parent, coord, chunkSettings.size);
                        else
                            chunk = Chunk.CreateWrapInstantiate(chunkObject, parent, coord, chunkSettings.size);
                        chunks.Add(coord, chunk);
                        visibleChunks.Add(chunk);
                    }
                }
            }
        }
    }
    public bool visibleCheck(Vector3Int coord)
    {
        return (coord - viewerCoord).sqrMagnitude <= radius * radius;
    }
    public bool visibleCheckAndSet(Chunk chunk)
    {
        chunk.Visible = visibleCheck(chunk.Coord);
        return chunk.Visible;
    }
    void visibleUpdate()
    {
        visibleChunks.RemoveAll(chunk => !visibleCheckAndSet(chunk));
    }

    private void OnDrawGizmos()
    {
        if (showChunkBorder)
        {
            foreach (var chunk in visibleChunks)
            {
                Gizmos.DrawWireCube(chunkSettings.worldPosToWorldPosCenter(chunk.transform.position), chunkSettings.size);
            }
        }
    }
}
