using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Luban;
using lubanTable;
using SimpleJSON;

public class DataSystem : System<DataSystem>
{
    private static string _dataTablePath = "Assets/Resources/_Data";

    private Tables _tables;
    //TODO: 因为目前没有lazy load，且单个表格没有基类，所以这里直接返回Tables，后续需要修改
    public Tables Tables => _tables;
    
    protected override void OnInit()
    {
        LoadData();
    }

    protected override void OnShutdown()
    {
        _tables = null;
    }

    /// <summary>
    /// 根据mode选择路径
    /// </summary>
    private string getDataPath(EDataPath mode)
    {
        string _path = "";

        switch (mode)
        {
            case EDataPath.Streaming:
                _path += Application.streamingAssetsPath;
                break;
            case EDataPath.Persistent:
                _path += Application.persistentDataPath;
                break;
            case EDataPath.Temporary:
                _path += Application.temporaryCachePath;
                break;
            default:
                _path += Application.dataPath;
                break;
        }

        return _path;
    }

    //TODO: 可能需要lazy load机制，暂时不需要
    private void LoadData()
    {
        var tablesCtor = typeof(Tables).GetConstructors()[0];
        var loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];
        // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
        System.Delegate loader = loaderReturnType == typeof(ByteBuf) ?
            new System.Func<string, ByteBuf>(LoadByteBuf)
            : (System.Delegate)new System.Func<string, JSONNode>(LoadJson);
        _tables = (Tables)tablesCtor.Invoke(new object[] {loader});
    }

    /// <summary>
    /// 存储文件
    /// </summary>
    /// <typeparam name="T">data的class</typeparam>
    /// <param name="saveData">要存储的data的class，只支持string,bool,int,float</param>
    /// <param name="mode">DataPath.xx</param>
    /// <param name="path">string,"/xx/xx.xx"需要指定一个flie</param>
    public void Save<T>(T saveData, EDataPath mode, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string finalPath = getDataPath(mode) + path;
        //��using��ֹfile����
        using (FileStream stream = new FileStream(finalPath, FileMode.Create))
        {
            formatter.Serialize(stream, saveData);
            stream.Close();
        }
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <typeparam name="T">data的class</typeparam>
    /// <param name="mode">DataPath.xx</param>
    /// <param name="path">string,"/xx/xx.xx"需要指定一个flie</param>
    /// <returns>返回data的class，如果无法获取则返回null</returns>
    public T Load<T>(EDataPath mode, string path) where T : class
    {
        T data = null;

        string finalPath = getDataPath(mode) + path;
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("[DataMgr] load data" + finalPath);

        if (File.Exists(finalPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(finalPath, FileMode.Open))
            {
                data = formatter.Deserialize(stream) as T;
                stream.Close();
            }
        }

        return data;
    }

    /// <summary>
    /// 删除某个路径下的文件
    /// </summary>
    /// <param name="mode">DataPath.xx</param>
    /// <param name="path"></param>
    public void Delete(EDataPath mode, string path)
    {
        string _path = getDataPath(mode);

        _path += path;

        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }
    
    private static JSONNode LoadJson(string file)
    {
        return JSON.Parse(File.ReadAllText($"{_dataTablePath}/{file}.json", System.Text.Encoding.UTF8));
    }

    private static ByteBuf LoadByteBuf(string file)
    {
        return new ByteBuf(File.ReadAllBytes($"{_dataTablePath}/{file}.bytes"));
    }
}