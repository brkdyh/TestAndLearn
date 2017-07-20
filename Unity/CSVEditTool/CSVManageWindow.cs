using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Events;

public class CSVManageWindow : EditorWindow
{
    static CSVManageWindow currentCSVWindow = null;

    static string ConfigCsvPath = "";
    public static string GetConfigCsvPath()
    {
        return ConfigCsvPath;
    }

    List<string> files = new List<string>();

    Vector2 ScrollPos = Vector2.zero;

    GUIStyle SelectStyle = null;
    GUIStyle DefaulfStyle = null;

    int CurSelectIndex = -1;

    bool ListenEvent = true;

    float lineHeight = 0;

    [MenuItem("数据表/数据表管理工具")]
    public static void Open()
    {
        if (currentCSVWindow == null)
        {
            currentCSVWindow = new CSVManageWindow();
            currentCSVWindow.minSize = new Vector2(600, 400);
            currentCSVWindow.titleContent = new GUIContent("数据表管理工具");

            if (ConfigCsvPath == string.Empty)
                ConfigCsvPath = Application.dataPath + "/StreamingAssets";

            if (!Directory.Exists(ConfigCsvPath))
                Directory.CreateDirectory(ConfigCsvPath);
        }

        RefreshFiles();

        currentCSVWindow.Show();
    }

    private void OnEnable()
    {
        DefaulfStyle = new GUIStyle();
        DefaulfStyle.normal.textColor = Color.white;

        SelectStyle = new GUIStyle();
        SelectStyle.normal.textColor = Color.blue;
        lineHeight = SelectStyle.lineHeight;
    }

    private void OnFocus()
    {
        RefreshFiles();
        ListenEvent = true;

    }

    private void OnLostFocus()
    {
        ListenEvent = false;
    }

    private void OnDestroy()
    {
        
    }

    private void OnGUI()
    {
        try
        {
            #region Draw UI

            float fCur_Draw_H = 0; //当前绘制的高度位置

            float fwindowHeight = currentCSVWindow.position.yMax - currentCSVWindow.position.yMin;
            float fwindowWidth = currentCSVWindow.position.max.x - currentCSVWindow.position.min.x;

            EditorGUILayout.BeginHorizontal();
            ConfigCsvPath = EditorGUILayout.TextField("包含路径:", ConfigCsvPath);

            if (GUILayout.Button("浏览"))
            {
                ConfigCsvPath = EditorUtility.OpenFolderPanel("浏览", Application.dataPath, "StreamingAssets");

                if (ConfigCsvPath == string.Empty)
                {
                    ConfigCsvPath = Application.dataPath + "/StreamingAssets/";
                }

                RefreshFiles();
            }

            EditorGUILayout.EndHorizontal();

            fCur_Draw_H += 24;  //+24

            Rect scroll = new Rect(0, fCur_Draw_H, fwindowWidth, fwindowHeight - 130);
            Rect scrollView = new Rect(0, 0, fwindowWidth, files.Count * 28);

            ScrollPos = GUI.BeginScrollView(scroll, ScrollPos, scrollView);// "U2D.CreateRect"
            {
                for (int i = 0; i < files.Count; i++)
                {
                    if (CurSelectIndex == i)
                    {
                        GUI.Label(new Rect(0, 28 * i, fwindowWidth, 24), files[i], "flow node 6");
                        //EditorGUILayout.LabelField(files[i], SelectStyle, GUILayout.Height(20));                       
                    }
                    else
                    {
                        GUI.Label(new Rect(0, 28 * i, fwindowWidth, 24), files[i], "flow node 0");
                        //EditorGUILayout.LabelField(files[i], DefaulfStyle, GUILayout.Height(20));
                    }
                }
            }
            GUI.EndScrollView();

            fCur_Draw_H += fwindowHeight - 120; //+fwindowHeight - 120

            fCur_Draw_H += 2;

            if (GUI.Button(new Rect(0, fCur_Draw_H, fwindowWidth, 20), "新建数据表"))
            {
                CreateNew();
            }

            fCur_Draw_H += 25;//+50;

            if (CurSelectIndex > -1)
            {
                if (GUI.Button(new Rect(0, fCur_Draw_H, fwindowWidth, 20), "打开数据表"))
                {
                    OpenCSVEditWindow();
                }

                fCur_Draw_H += 25;//+50;

                if (GUI.Button(new Rect(0, fCur_Draw_H, fwindowWidth, 20), "删除数据表"))
                {
                    DeleteCSV();
                }
            }
            else
            {
                GUI.Box(new Rect(0, fCur_Draw_H, fwindowWidth, 20), "打开数据表", "PreBackground");

                fCur_Draw_H += 25;//+50;

                GUI.Box(new Rect(0, fCur_Draw_H, fwindowWidth, 20), "删除数据表", "PreBackground");
            }

            fCur_Draw_H += 25;//+50;

            if (GUI.Button(new Rect(0, fCur_Draw_H, fwindowWidth, 20), "关闭窗口"))
            {
                currentCSVWindow.Close();
            }

            #endregion

            #region Event Listen

            if (ListenEvent)
            {
                if (Event.current.type == EventType.mouseDown)
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    if (scroll.Contains(mousePos))
                    {
                        //Debug.LogError("MouseDown pos :" + mousePos);

                        //Debug.LogError("相对坐标: " + (mousePos.y - scroll.yMin));
                        //Debug.LogError("line height ：" + lineHeight);
                        //Debug.LogError(ScrollPos);
                        //Debug.LogError((int)((mousePos.y + ScrollPos.y - scroll.yMin) / 24));
                        CurSelectIndex = (int)((mousePos.y + ScrollPos.y - scroll.yMin) / 28);
                        Repaint();
                    }
                }
            }
            #endregion
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            this.Close();
        }
    }

    void CreateNew()
    {
        CsvData csvData = new CsvData(string.Empty);
        csvData.AddRow();
        csvData.AddColumn();
        CSVEditWindow csveWindow = new CSVEditWindow(csvData);
        csveWindow.Show();

        RefreshFiles();
    }

    void OpenCSVEditWindow()
    {
        if (CurSelectIndex < 0 || CurSelectIndex > files.Count)
            return;

        string filePath = files[CurSelectIndex];
        CSVEditWindow window = new CSVEditWindow(CsvEditTool.OpenCsvData(filePath));
        int index = filePath.LastIndexOf('/') + 1;
        //Debug.LogError(index + " ==> " + filePath.Length);
        string fileName = filePath.Substring(index, filePath.Length - index);
        window.titleContent = new GUIContent(fileName);
        window.Show();

        CurSelectIndex = -1;
    }

    void DeleteCSV()
    {
        if (CurSelectIndex < 0 || CurSelectIndex > files.Count)
            return;

        string filePath = files[CurSelectIndex];

        if (File.Exists(filePath))
            File.Delete(filePath);

        RefreshFiles();

        CurSelectIndex = -1;
    }

    public static void RefreshFiles()
    {
        if (ConfigCsvPath == string.Empty)
            return;

        currentCSVWindow.files.Clear();

        //string[] dirs = Directory.GetDirectories(ConfigCsvPath);

        List<string> dirs = new List<string>();
        GetDirectories(ConfigCsvPath, ref dirs);
        

        foreach (string dir in dirs)
        {
            //Debug.LogError("Search File Path = " + dir);

            string[] files = Directory.GetFiles(dir);

            foreach (string file in files)
            {
                if (file.Contains(".csv") && !file.Contains(".meta"))
                {
                    string sfile = file.Replace('\\', '/');
                    currentCSVWindow.files.Add(sfile);
                }
            }
        }

        if (currentCSVWindow != null)
            currentCSVWindow.Repaint();
    }

    static void GetDirectories(string root, ref List<string> dirs)
    {
        if (!dirs.Contains(root))
            dirs.Add(root);

        string[] sub = Directory.GetDirectories(root);
        //Debug.LogError("子路径数量： " + sub.Length);

        if (sub.Length > 0)
        {
            for (int i = 0; i < sub.Length; i++)
            {
                dirs.Add(sub[i]);
                //Debug.LogError("Add = " + sub[i]);
                GetDirectories(sub[i], ref dirs);
            }
        }
        else
        {
            return;
        }
    }

    public static void CreateNewFile()
    {
        if (currentCSVWindow != null)
            currentCSVWindow.CreateNew();
    }

    public static void OpenCSV(string filePath)
    {
        CSVEditWindow window = new CSVEditWindow(CsvEditTool.OpenCsvData(filePath));
        int index = filePath.LastIndexOf("/") + 1;
        string fileName = filePath.Substring(index, filePath.Length - index);
        window.titleContent = new GUIContent(fileName);
        window.Show();
    }
}