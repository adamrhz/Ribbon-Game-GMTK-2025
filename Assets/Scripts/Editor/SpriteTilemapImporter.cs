using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SpriteTilemapAutoSlice : EditorWindow
{
    private List<Texture2D> collisionTextures = new List<Texture2D>();
    private List<Texture2D> backgroundTextures = new List<Texture2D>();
    private List<string> groupNames = new List<string>();
    private int cellSize = 32;

    [MenuItem("Tools/Import Multiple Sprite Groups to Tilemaps")]
    public static void ShowWindow()
    {
        GetWindow<SpriteTilemapAutoSlice>("Sprite Groups to Tilemaps");
    }

    private void OnGUI()
    {
        GUILayout.Label("Multiple Groups (Lap1, Lap2, etc.)", EditorStyles.boldLabel);
        cellSize = EditorGUILayout.IntField("Cell Size", cellSize);

        int groupCount = EditorGUILayout.IntField("Number of Groups", groupNames.Count == 0 ? 1 : groupNames.Count);
        EnsureGroupCount(groupCount);

        for (int i = 0; i < groupNames.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            groupNames[i] = EditorGUILayout.TextField("Group Name", groupNames[i]);
            collisionTextures[i] = (Texture2D)EditorGUILayout.ObjectField("Collision Texture", collisionTextures[i], typeof(Texture2D), false);
            backgroundTextures[i] = (Texture2D)EditorGUILayout.ObjectField("Background Texture (optional)", backgroundTextures[i], typeof(Texture2D), false);
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Process All Groups"))
        {
            for (int i = 0; i < groupNames.Count; i++)
            {
                if (collisionTextures[i] != null)
                {
                    ProcessGroup(groupNames[i], collisionTextures[i], backgroundTextures[i]);
                }
                else
                {
                    Debug.LogWarning($"Group {groupNames[i]} skipped (no collision texture).");
                }
            }
        }
    }

    private void EnsureGroupCount(int count)
    {
        while (groupNames.Count < count)
        {
            groupNames.Add($"Lap{groupNames.Count + 1}");
            collisionTextures.Add(null);
            backgroundTextures.Add(null);
        }

        while (groupNames.Count > count)
        {
            int last = groupNames.Count - 1;
            groupNames.RemoveAt(last);
            collisionTextures.RemoveAt(last);
            backgroundTextures.RemoveAt(last);
        }
    }

    private void ProcessGroup(string groupName, Texture2D collisionTex, Texture2D backgroundTex)
    {
        GameObject gridObj = new GameObject(groupName, typeof(Grid));
        Grid grid = gridObj.GetComponent<Grid>();
        grid.cellSize = new Vector3(1, 1, 0);

        GameObject collisionGO = new GameObject("Collision", typeof(Tilemap), typeof(TilemapRenderer), typeof(TilemapCollider2D), typeof(Rigidbody2D), typeof(CompositeCollider2D));
        collisionGO.transform.SetParent(gridObj.transform);
        Tilemap collisionTilemap = collisionGO.GetComponent<Tilemap>();

        Rigidbody2D rb = collisionGO.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        TilemapCollider2D tilemapCollider = collisionGO.GetComponent<TilemapCollider2D>();
        tilemapCollider.usedByComposite = true;

        CompositeCollider2D composite = collisionGO.GetComponent<CompositeCollider2D>();
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;

        collisionGO.GetComponent<TilemapRenderer>().sortingOrder = 0;

        Tilemap backgroundTilemap = null;
        if (backgroundTex != null)
        {
            GameObject backgroundGO = new GameObject("Background", typeof(Tilemap), typeof(TilemapRenderer));
            backgroundGO.transform.SetParent(gridObj.transform);
            backgroundTilemap = backgroundGO.GetComponent<Tilemap>();
            backgroundGO.GetComponent<TilemapRenderer>().sortingOrder = -1;
        }

        Dictionary<string, Sprite> collisionSprites = SliceTexture(collisionTex);
        FillTilemap(collisionTilemap, collisionSprites);

        if (backgroundTex != null)
        {
            Dictionary<string, Sprite> backgroundSprites = SliceTexture(backgroundTex);
            FillTilemap(backgroundTilemap, backgroundSprites);
        }

        Debug.Log($"Processed group: {groupName}");
    }

    private Dictionary<string, Sprite> SliceTexture(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            Debug.LogError("Not a valid texture importer.");
            return new Dictionary<string, Sprite>();
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.maxTextureSize = 8192;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.spritePixelsPerUnit = cellSize;
        importer.isReadable = true;

        List<SpriteMetaData> metas = new List<SpriteMetaData>();
        Color32[] pixels = texture.GetPixels32();
        int texWidth = texture.width;
        int texHeight = texture.height;

        for (int y = 0; y < texHeight; y += cellSize)
        {
            for (int x = 0; x < texWidth; x += cellSize)
            {
                if (!IsEmptyTile(pixels, texWidth, x, y, cellSize))
                {
                    SpriteMetaData meta = new SpriteMetaData
                    {
                        rect = new Rect(x, y, cellSize, cellSize),
                        name = $"tile_{x}_{y}",
                        pivot = new Vector2(0.5f, 0.5f),
                        alignment = (int)SpriteAlignment.Center
                    };
                    metas.Add(meta);
                }
            }
        }

        importer.spritesheet = metas.ToArray();
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Object[] slicedSprites = AssetDatabase.LoadAllAssetsAtPath(path);
        Dictionary<string, Sprite> spriteMap = new Dictionary<string, Sprite>();

        foreach (var obj in slicedSprites)
        {
            if (obj is Sprite s)
            {
                spriteMap[s.name] = s;
            }
        }

        return spriteMap;
    }

    private bool IsEmptyTile(Color32[] pixels, int texWidth, int startX, int startY, int size)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int px = startX + x;
                int py = startY + y;

                if (px >= texWidth || py >= pixels.Length / texWidth)
                    continue;

                Color32 c = pixels[py * texWidth + px];
                if (c.a > 0) return false;
            }
        }
        return true;
    }

    private void FillTilemap(Tilemap tilemap, Dictionary<string, Sprite> spriteMap)
    {
        foreach (var kvp in spriteMap)
        {
            string[] parts = kvp.Key.Split('_');
            int x = int.Parse(parts[1]);
            int y = int.Parse(parts[2]);

            Vector3Int pos = new Vector3Int(x / cellSize, y / cellSize, 0);

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = kvp.Value;
            tilemap.SetTile(pos, tile);
        }
    }
}
