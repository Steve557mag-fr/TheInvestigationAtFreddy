using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;

public class DataHandle
{
    const string FILE = "data.dt";
    const string DEV_TAG = "DEV-D";

    static readonly JObject DEFAULT_DATA = new()
    {
        {"_VERSION", DataVersion},
        {"sensitivity",1f},
        {"graphicMode",1},
        {"postprocess",true},
        {"mapping",new JObject(){
            {"forward",     (int)KeyCode.W},
            {"backward",    (int)KeyCode.S},
            {"left",        (int)KeyCode.A},
            {"right",       (int)KeyCode.D},
            {"run",         (int)KeyCode.LeftShift},
            {"interact",    (int)KeyCode.Mouse0},
            {"slide",       (int)KeyCode.Mouse1}
        }},
    };
    static JObject data;

    static string DataVersion
    {
        get { return $"{Application.version}_{DEV_TAG}"; }
    }
    static string DataPath
    {
        get { return $"{Application.persistentDataPath}/{FILE}"; }
    }

    public static void LoadData(){
        JustLoadData();
        UpdateData();
    }

    static void JustLoadData()
    {
        if (!File.Exists(DataPath)) ResetData();
        data = JObject.Parse(File.ReadAllText(DataPath));
    }

    static int UpdRecursive(JObject jo, string k, int exc = 0)
    {
        if (exc == 64) return -64;
        JToken PARENT = k == "" ? jo : DEFAULT_DATA.SelectToken(jo.Path + $".{k}");
        JToken INDATA = k == "" ? data : data.SelectToken(jo.Path + $".{k}");
        Debug.Log("PARENT: " + PARENT + "\n INDATA: "+INDATA);

        int n = 0;
        foreach(var kp in jo)
        {
            if (kp.Value.Type == JTokenType.Object)
            {
                if(INDATA.SelectToken(kp.Key) == null) INDATA[kp.Key] = new JObject();
                int e = UpdRecursive(kp.Value.ToObject<JObject>(), kp.Key, exc++);
                n+=e;
                continue;
            }
            if (INDATA.SelectToken(kp.Key) != null) continue;
            Debug.Log($"New Key! ({kp.Key})");
            INDATA[kp.Key] = kp.Value;
        }
        return n;
    }

    static int UpdateData()
    {
        if (data == null) LoadData();
        if (GetData<string>("_VERSION") == DataVersion) return -2;
        int ne = UpdRecursive(DEFAULT_DATA, "");
        SetData("", "_VERSION", DataVersion);
        SaveData();
        return ne;
    }

    public static void ResetData()
    {
        data = DEFAULT_DATA;
        SaveData();
    }

    public static void SaveData()
    {
        File.WriteAllText(DataPath, data.ToString());
    }

    public static T GetData<T>(string keyPath)
    {
        return data.SelectToken(keyPath).ToObject<T>();
    }

    public static void SetData(string keyPath, string keyName, object value)
    {
        if (data == null) LoadData();
        (keyPath == "" ? data :data.SelectToken(keyPath))[keyName] = JToken.FromObject(value);
    }

    #if UNITY_EDITOR

    [UnityEditor.MenuItem("Data/List All")]
    public static void EditorListData()
    {
        if (data == null) LoadData();

        StringBuilder bld = new();
        bld.AppendLine("__ List of Data __");
        bld.AppendLine($"Path: {DataPath}\n");
        foreach(var kp in data)
        {
            bld.AppendLine($"{kp.Key} = {kp.Value}");
        }
        UnityEditor.EditorUtility.DisplayDialog("Data Result", bld.ToString(),"OK");
    }

    [UnityEditor.MenuItem("Data/Reset Data")]
    public static void EditorResetData()
    {
        ResetData();
        UnityEditor.EditorUtility.DisplayDialog("Data Result", "Reset Done!", "OK");
    }

    [UnityEditor.MenuItem("Data/Update Data")]
    public static void EditorUpdateData()
    {
        if (data == null) JustLoadData();
        StringBuilder bld = new();
        bld.AppendLine("__ Update Report __\n");
        bld.AppendLine($"Current System Version: {DataVersion}");
        bld.AppendLine($"\nOld Version: ${GetData<string>("_VERSION")}");
        int ne = UpdateData();
        bld.AppendLine($"New Count: ${ne}");
        bld.AppendLine($"New Version: ${GetData<string>("_VERSION")}");
        UnityEditor.EditorUtility.DisplayDialog("Data Result", bld.ToString() , "OK");
    }

    [UnityEditor.MenuItem("Data/Reload Data")]
    public static void EditorReloadData()
    {
        LoadData();
        UnityEditor.EditorUtility.DisplayDialog("Data Result", "Reload Done!", "OK");
    }

    #endif

}
