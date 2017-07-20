using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class CSVStyleSetting : EditorWindow
{
    string[] HandleStyles = { "flow node 0", "flow node 1", "flow node 2", "flow node 3", "flow node 4", "flow node 5", "flow node 6" };
    string[] CellStyles = { "flow node 0", "flow node 1", "flow node 2", "flow node 3", "flow node 4", "flow node 5", "flow node 6" };

    string[] ToolBar = { "单元格样式", "控制器样式" };

    static CSVStyleSetting _window = null;

    Vector2 HandleStylesScroll = Vector2.zero;

    public static string GlobalHandleStyle = "flow node 0";
    public static string GlobalCellStyle = "flow node 1";

    int SelectItem = 0;
    void OnGUI()
    {
        SelectItem = GUILayout.Toolbar(SelectItem, ToolBar);

        GUILayout.Space(8);

        HandleStylesScroll = GUILayout.BeginScrollView(HandleStylesScroll);
        {
            switch (SelectItem)
            {
                case 0:
                    {
                        for (int i = 0; i < CellStyles.Length; i++)
                        {
                            GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                            GUILayout.Space(7);
                            if (GUILayout.Button("    单元格样式    ", CellStyles[i]))
                            {
                                //Debug.LogError(HandleStyles[i]);
                                GlobalCellStyle = CellStyles[i];
                            }
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.SelectableLabel("\"" + CellStyles[i] + "\"");
                            GUILayout.EndHorizontal();
                            GUILayout.Space(11);
                        }
                        break;
                    }
                case 1:
                    {
                        for (int i = 0; i < HandleStyles.Length; i++)
                        {
                            GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                            GUILayout.Space(7);
                            if (GUILayout.Button("    控制器样式    ", HandleStyles[i]))
                            {
                                GlobalHandleStyle = HandleStyles[i];
                            }
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.SelectableLabel("\"" + HandleStyles[i] + "\"");
                            GUILayout.EndHorizontal();
                            GUILayout.Space(11);
                        }
                        break;
                    }
            }
        }
        GUILayout.EndScrollView();
    }

    [MenuItem("数据表/样式设置")]
    public static void Open()
    {
        if (_window != null)
        {
            _window.Focus();
            return;
        }

        _window = new CSVStyleSetting();
        _window.minSize = new Vector2(300, 400);
        _window.Show();
    }

    private void OnDestroy()
    {
        _window = null;
    }
}
