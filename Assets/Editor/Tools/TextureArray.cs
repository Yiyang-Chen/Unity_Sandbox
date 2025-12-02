using UnityEngine;
using UnityEditor;
using System.IO; // 用于 Path 等操作

public class Texture2DArrayCreator : EditorWindow
{
    [SerializeField] private Texture2D[] sourceTextures;
    [SerializeField] private bool generateMipmaps = false;

    // 默认保存路径
    [SerializeField] private string assetPath = "Assets/Resources/Textures/TextureArray/MyTexture2DArray.asset";

    [MenuItem("Tools/Create Texture2DArray Asset")]
    public static void OpenWindow()
    {
        GetWindow<Texture2DArrayCreator>("Texture2DArray Creator");
    }

    private void OnGUI()
    {
        // 将当前窗口对象包裹成 SerializedObject，方便使用 PropertyField
        SerializedObject so = new SerializedObject(this);

        // 绘制“源纹理”数组
        EditorGUILayout.PropertyField(so.FindProperty("sourceTextures"), new GUIContent("Source Textures"), true);
        // 绘制“是否生成 Mipmap”
        EditorGUILayout.PropertyField(so.FindProperty("generateMipmaps"), new GUIContent("Generate Mipmaps"));
        // 绘制“保存路径”
        EditorGUILayout.PropertyField(so.FindProperty("assetPath"), new GUIContent("Save Path"));

        so.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("Create Texture2DArray Asset", GUILayout.Height(25)))
        {
            if (sourceTextures != null && sourceTextures.Length > 0)
            {
                CreateTexture2DArray(sourceTextures, generateMipmaps, assetPath);
            }
            else
            {
                Debug.LogError("No Source Textures assigned! Please add at least one Texture2D.");
            }
        }
    }

    private void CreateTexture2DArray(Texture2D[] textures, bool mipmaps, string savePath)
    {
        // 1) 检查贴图尺寸是否一致
        int width  = textures[0].width;
        int height = textures[0].height;
        for (int i = 1; i < textures.Length; i++)
        {
            if (textures[i].width != width || textures[i].height != height)
            {
                Debug.LogError($"All source textures must have the same dimensions! " +
                               $"Mismatch at index {i} ({textures[i].width}x{textures[i].height}) != ({width}x{height}).");
                return;
            }
        }

        // 2) 准备要写入的最终路径（确保文件夹存在 + 文件名不冲突 + 补后缀）
        string finalPath = PrepareSavePath(savePath);

        // 3) 以第一张纹理的格式为准（若与 Graphics.CopyTexture 不兼容，可以再做进一步处理）
        TextureFormat format = textures[0].format;

        // 4) 创建 Texture2DArray
        Texture2DArray textureArray = new Texture2DArray(
            width,
            height,
            textures.Length,
            format,
            mipmaps,
            false // 是否线性空间，如需自定义可自行添加字段
        );

        // 5) 拷贝每张纹理到对应的切片
        for (int i = 0; i < textures.Length; i++)
        {
            Graphics.CopyTexture(textures[i], 0, 0, textureArray, i, 0);
        }

        // 6) 写入 .asset 文件
        AssetDatabase.CreateAsset(textureArray, finalPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=green>Created Texture2DArray:</color> {finalPath}");
    }

    /// <summary>
    /// 根据给定路径，自动：
    /// 1) 确保文件夹存在（如不存在则逐级创建）；
    /// 2) 如果缺少 .asset 后缀则自动添加；
    /// 3) 如果已存在同名文件则在后面加 (1), (2), ...。
    /// 返回最终可用的路径字符串。
    /// </summary>
    private string PrepareSavePath(string rawPath)
    {
        // 若用户没输入任何东西，则给个默认值
        if (string.IsNullOrEmpty(rawPath))
        {
            rawPath = "Assets/Resources/Textures/TextureArray/MyTexture2DArray.asset";
        }

        // 分离出文件夹和文件名
        string folderPath = Path.GetDirectoryName(rawPath);
        string fileName   = Path.GetFileName(rawPath);

        // 如果没有后缀，自动加上 .asset
        if (Path.GetExtension(fileName) != ".asset")
        {
            fileName = Path.GetFileNameWithoutExtension(fileName) + ".asset";
        }

        // 确保文件夹存在，不存在则逐级创建
        EnsureFolderExists(folderPath);

        // 组合成完整路径
        string combinedPath = Path.Combine(folderPath, fileName).Replace('\\', '/');

        // 若已存在同名文件，则自动加 (1), (2), ... 直到不重复
        string uniquePath = GetUniqueAssetPath(combinedPath);

        return uniquePath;
    }

    /// <summary>
    /// 若文件夹不存在，则逐级创建
    /// </summary>
    private void EnsureFolderExists(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath)) return;

        // 用'/'分割
        string[] parts = folderPath.Split('/');
        if (parts.Length == 0) return;

        // 假设项目中路径从 "Assets" 开始
        string current = parts[0]; // "Assets"
        for (int i = 1; i < parts.Length; i++)
        {
            string nextFolder = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(nextFolder))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = nextFolder;
        }
    }

    /// <summary>
    /// 若已存在同名文件，则在文件名后加 (1), (2), ... 
    /// </summary>
    private string GetUniqueAssetPath(string originalPath)
    {
        string directory = Path.GetDirectoryName(originalPath);
        string fileName  = Path.GetFileNameWithoutExtension(originalPath);
        string extension = Path.GetExtension(originalPath);

        int counter = 1;
        string path = originalPath;
        while (AssetDatabase.LoadAssetAtPath<Object>(path) != null)
        {
            // 例如 MyTexture2DArray(1).asset, MyTexture2DArray(2).asset...
            string newFileName = $"{fileName}({counter}){extension}";
            path = Path.Combine(directory, newFileName).Replace('\\', '/');
            counter++;
        }
        return path;
    }
}
