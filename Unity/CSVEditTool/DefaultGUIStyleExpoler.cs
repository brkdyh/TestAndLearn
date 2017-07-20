using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DefaultGUIStyleExpoler : EditorWindow
{
    static GUIStyle[] styles = null;
    static DefaultGUIStyleExpoler w = null;

    //[MenuItem("数据表/Style展示")]
    public static void Open()
    {
        if (styles == null)
        {
            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            styles = skin.customStyles;
        }

        if (w == null)
        {
            w = new DefaultGUIStyleExpoler();
            w.Show();

        }
    }

    Vector2 scp = Vector2.zero;
    private void OnGUI()
    {
        scp = GUILayout.BeginScrollView(scp);
        {
            for (int i = 0; i < styles.Length; i++)
            {
                GUILayout.Button(styles[i].ToString(), styles[i]);

            }
        }
        GUILayout.EndScrollView();
    }

    private void OnDestroy()
    {
        w = null;
    }
}
