using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

public class GridDrawer : MonoBehaviour
{
    [BoxGroup("Grid Settings"), SerializeField]
    private Material gridMaterial;

    [BoxGroup("Grid Settings"), SerializeField]
    private int gridSize = 10;

    [BoxGroup("Grid Settings"), SerializeField] 
    private int gridTypes = 4;
    
    private Texture2D runtimeTypeMapTex;

    private void Awake()
    {
        if (originTypeMapTex == null)
        {
            originTypeMapTex = new Texture2D(gridSize, gridSize, TextureFormat.RFloat, false);
            originTypeMapTex.wrapMode = TextureWrapMode.Clamp;
            originTypeMapTex.filterMode = FilterMode.Point;
        }
    
        // 修改为使用构造函数创建新纹理并复制像素数据
        runtimeTypeMapTex = new Texture2D(originTypeMapTex.width, originTypeMapTex.height, originTypeMapTex.format, originTypeMapTex.mipmapCount > 0);
        runtimeTypeMapTex.wrapMode = originTypeMapTex.wrapMode;
        runtimeTypeMapTex.filterMode = originTypeMapTex.filterMode;
        Graphics.CopyTexture(originTypeMapTex, runtimeTypeMapTex);
        
        Test();

        UpdateGrid();
    }

    private void OnDestroy()
    {
        if (runtimeTypeMapTex != null)
        {
            Destroy(runtimeTypeMapTex);
        }
        gridMaterial.SetTexture("_TypeMap", originTypeMapTex);
    }

    // pos 必须不为null
    // 调用完成后需要调用 ApplyTypeChanges()
    public void UpdateGridPosType(GridPosition2D pos, EGridType gridType)
    {
        Texture2D tex = originTypeMapTex;
        if (Application.isPlaying)
        {
            tex = runtimeTypeMapTex;
        }
        // 确保值在0-1范围内
        float r = Mathf.Clamp01((float)gridType / (float)(gridTypes - 1));
        tex.SetPixel(pos.X, pos.Z, new Color(r, 0f, 0f, 1f));
    }

    public void ApplyTypeChanges()
    {
        Texture2D tex = originTypeMapTex;
        if (Application.isPlaying)
        {
            tex = runtimeTypeMapTex;
        }
        tex.Apply();
    }
    
#region EditorButtons
    [Button]
    private void UpdateGrid()
    {
        Texture2D tex = originTypeMapTex;
        if (Application.isPlaying)
        {
            tex = runtimeTypeMapTex;
        }
        gridMaterial.SetTexture("_TypeMap", tex);
        gridMaterial.SetInt("_GridSize", gridSize);
    }
    
    // ----------------手动设置格子类型------------------
#if UNITY_EDITOR
    [BoxGroup("SetGridPosType"), SerializeField]
    private int x;

    [BoxGroup("SetGridPosType"),SerializeField]
    private int z;

    [BoxGroup("SetGridPosType"),SerializeField]
    private EGridType type;
#endif

    [BoxGroup("SetGridPosType"),Button]
    private void UpdateGridPosType()
    {
        GridPosition2D pos = new GridPosition2D(x, z);
        UpdateGridPosType(pos, type);
        ApplyTypeChanges();
    }
    
    // ------------生成type map---------------
    
    [FormerlySerializedAs("typeMapTex")]
    [BoxGroup("GridType"), SerializeField]
    private Texture2D originTypeMapTex;
    
    [BoxGroup("GridType"), Button("Create Type Map Texture")]
    private void CreateTypeMapTexture()
    {
#if UNITY_EDITOR
        originTypeMapTex = new Texture2D(gridSize, gridSize, TextureFormat.RFloat, false);
        originTypeMapTex.wrapMode = TextureWrapMode.Clamp;
        originTypeMapTex.filterMode = FilterMode.Point;
        
        // 保存为资产
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Type Map Texture",
            "TypeMapTexture",
            "asset",
            "Select save location");
            
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(originTypeMapTex, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
    
    // ----------------测试用------------------
#if UNITY_EDITOR
    [Button]
    private void Test() 
    {
        // 遍历所有格子，把类型写进 R 通道
        for(int _z = 0; _z < gridSize; _z++)
        {
            for(int _x = 0; _x < gridSize; _x++)
            {
                // 构建EGridType类型的枚举变量
                int t = (_x + _z) % gridTypes;
                UpdateGridPosType(new GridPosition2D(_x, _z), (EGridType)t);
                
                /*if (Application.isPlaying)
                {
                    ___Singletons.Log.Debug("x:" + x + " y:" + y + " r:" + r);
                }*/
                
            }
        }

        ApplyTypeChanges();
    }
#endif
#endregion
}
