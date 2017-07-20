using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Events;
using _CSVEditWindow.Sub_Window;
using System;

public class CSVEditWindow : EditorWindow
{
    #region CellHandleData

    /// <summary>
    /// 单元格Handle数据
    /// </summary>
    public class CellHandleData
    {
        /// <summary>
        /// Handel的描述，y=0 为行Handle , x = 0 为列Handle
        /// </summary>
        public Vector2 HandleDescription = Vector2.zero;
        float cellSize = 0;

        /// <summary>
        /// Handle控制的单元格尺寸
        /// </summary>
        public float CellSize
        {
            set
            {
                cellSize = value;

                if (HandleDescription.x == 0)
                {
                    HandleRect = new Rect(HandleRect.xMin, HandleRect.yMin, cellSize, HandleRect.yMax);
                }
                else if (HandleDescription.y == 0)
                {
                    HandleRect = new Rect(HandleRect.xMin, HandleRect.yMin, HandleRect.xMax, cellSize);
                }
            }
            get { return cellSize; }
        }

        /// <summary>
        /// Handle屏幕坐标
        /// </summary>
        public Rect HandleRect = Rect.zero;

        //改变Handle控制Cell尺寸的拖拽区域
        public Rect ChangeCellSizeAreaRect
        {
            get
            {
                Rect r = Rect.zero;

                if (HandleDescription.x == 0)
                {//如果是列Handle
                    r = new Rect(HandleRect.xMax - 10, HandleRect.yMin, 10, HandleRect.yMax);//宽10
                }
                else if (HandleDescription.y == 0)
                {//如果是行Handle
                    r = new Rect(HandleRect.xMin, HandleRect.yMax - 10, HandleRect.xMax, 10);//高10
                }

                return r;
            }
        }
    }

    #endregion

    #region CellHandleManager

    /// <summary>
    /// Handle管理器
    /// </summary>
    class CellHandleManager
    {
        Vector2 defaultCellSize = new Vector2(100, 25); //默认单元格尺寸

        int Row = 0;        //表格行数
        int Column = 0;     //表格列数

        Vector2 TotalSize = Vector2.zero;       //当前表格尺寸

        List<CellHandleData> row_Handle = new List<CellHandleData>();       //行Handle
        List<CellHandleData> column_Handle = new List<CellHandleData>();    //列Handle

        CSVEditWindow HostWindow = null;

        public CellHandleManager(CSVEditWindow HostWindow)
        {
            row_Handle.Add(new CellHandleData());       //添加第0行
            column_Handle.Add(new CellHandleData());    //添加第0列

            this.HostWindow = HostWindow;
        }

        /// <summary>
        /// 设置表格尺寸
        /// </summary>
        /// <param name="csvRow">csv数据行数</param>
        /// <param name="csvColumn">csv数据列数</param>
        public void SetSize(int csvRow, int csvColumn)
        {
            this.Row = csvRow + 1;             //增加Handle在表格中的行
            this.Column = csvColumn + 1;       //增加Handle在表格中的列

            TotalSize = new Vector2(this.Column * defaultCellSize.x, this.Row * defaultCellSize.y);     //初始化Size;
            //Debug.LogError("Init Total Size : " + TotalSize);
            SetHandle();       //设置Handle
        }

        void SetHandle()
        {
            //add row handle

            float rowCellSizeSum = defaultCellSize.y;

            for (int r = row_Handle.Count; r < Row; r++)
            {
                CellHandleData rd = new CellHandleData();

                rd.HandleDescription = new Vector2(r, 0);
                rd.CellSize = 25;
                rd.HandleRect = new Rect(0, rowCellSizeSum, 100, rd.CellSize);
                rowCellSizeSum += rd.CellSize;
                row_Handle.Add(rd);
            }

            //add column handle

            float columnCellSizeSum = defaultCellSize.x;

            for (int c = column_Handle.Count; c < Column; c++)
            {
                CellHandleData cd = new CellHandleData();

                cd.HandleDescription = new Vector2(0, c);
                cd.CellSize = 100;
                cd.HandleRect = new Rect(columnCellSizeSum, 0, cd.CellSize, 25);
                columnCellSizeSum += cd.CellSize;
                column_Handle.Add(cd);
            }
        }

        //Add Handle
        void AddHandle()
        {
            //add row handle
            for (int r = row_Handle.Count; r < Row; r++)
            {
                CellHandleData rd = new CellHandleData();

                rd.HandleDescription = new Vector2(r, 0);
                rd.CellSize = 25;
                rd.HandleRect = new Rect(0, GetTotalSize().y - rd.CellSize, 100, rd.CellSize);

                row_Handle.Add(rd);
            }

            //add column handle
            for (int c = column_Handle.Count; c < Column; c++)
            {
                CellHandleData cd = new CellHandleData();

                cd.HandleDescription = new Vector2(0, c);
                cd.CellSize = 100;
                cd.HandleRect = new Rect(GetTotalSize().x - cd.CellSize, 0, cd.CellSize, 25);

                column_Handle.Add(cd);
            }
        }

        //绘制Handle
        public void DrawHandle()
        {
            for (int r = 1; r < Row; r++)
            {
                if (row_Handle.Count > r)
                {
                    CellHandleData rd = row_Handle[r];
                    GUI.Label(rd.HandleRect, "第" + r + "行", CSVStyleSetting.GlobalHandleStyle);
                }
            }

            for (int c = 1; c < Column; c++)
            {
                if (column_Handle.Count > c)
                {
                    CellHandleData cd = column_Handle[c];
                    GUI.Label(cd.HandleRect, "第" + c + "列", CSVStyleSetting.GlobalHandleStyle);
                }
            }
        }

        //Handle拖拽检测
        bool beginDrag = false;
        Vector2 mousePosOnDragBegin = Vector2.zero;
        Vector2 dragHandlePos = Vector2.zero;
        public void EventListen()
        {
            if (Event.current.type == EventType.mouseDown)
            {
                Vector2 mousePos = Event.current.mousePosition;

                for (int r = 1; r < row_Handle.Count; r++)
                {

                    if (row_Handle[r].ChangeCellSizeAreaRect.Contains(mousePos))
                    {
                        //Debug.LogError("row Handle: " + row_Handle[r].HandlePostion);

                        beginDrag = true;
                        mousePosOnDragBegin = mousePos;
                        dragHandlePos = row_Handle[r].HandleDescription;
                    }

                }

                for (int c = 1; c < column_Handle.Count; c++)
                {
                    if (column_Handle[c].ChangeCellSizeAreaRect.Contains(mousePos))
                    {
                        //Debug.LogError("column_Handle: " + column_Handle[c].HandlePostion);
                        beginDrag = true;
                        mousePosOnDragBegin = mousePos;
                        dragHandlePos = column_Handle[c].HandleDescription;
                    }
                }
            }

            if (Event.current.type == EventType.mouseUp)
            {
                beginDrag = false;
            }

            if (beginDrag)
            {
                //Debug.LogError("drag!  delta mouse : " + (Event.current.mousePosition - mousePosOnDragBegin));
                ChangeCellSzie(dragHandlePos, Event.current.mousePosition - mousePosOnDragBegin);
                mousePosOnDragBegin = Event.current.mousePosition;
                HostWindow.Repaint();
            }
        }

        /// <summary>
        /// 改变单元格尺寸
        /// </summary>
        /// <param name="HandlePosDes">Handle位置描述</param>
        /// <param name="deltaSize"></param>
        public void ChangeCellSzie(Vector2 HandlePosDes, Vector2 deltaSize)
        {
            //Debug.LogError("Change! +" + HandlePos);
            //改变行
            if (row_Handle.Count > HandlePosDes.x && HandlePosDes.x != 0)
            {
                //Debug.LogError("Change Row!");
                if (row_Handle[(int)HandlePosDes.x] != null)
                {
                    row_Handle[(int)HandlePosDes.x].CellSize += deltaSize.y;

                    //Debug.LogError("Change Row!   " + row_Handle[(int)HandlePos.x].CellSize + "  /  " + deltaSize.y);

                    //后续偏移
                    for (int i = (int)HandlePosDes.x + 1; i < row_Handle.Count; i++)
                    {
                        Rect orginal = row_Handle[i].HandleRect;
                        Rect r = new Rect(orginal.x, orginal.y + deltaSize.y, orginal.width, orginal.height);
                        row_Handle[i].HandleRect = r;
                    }

                    ChangeTotalSize(new Vector2(0, deltaSize.y));
                }
            }

            //改变列
            if (column_Handle.Count > HandlePosDes.y && HandlePosDes.y != 0)
            {
                if (column_Handle[(int)HandlePosDes.y] != null)
                {
                    column_Handle[(int)HandlePosDes.y].CellSize += deltaSize.x;

                    //后续偏移
                    for (int i = (int)HandlePosDes.y + 1; i < column_Handle.Count; i++)
                    {
                        Rect orginal = column_Handle[i].HandleRect;
                        Rect r = new Rect(orginal.x + deltaSize.x, orginal.y, orginal.width, orginal.height);
                        column_Handle[i].HandleRect = r;
                    }

                    ChangeTotalSize(new Vector2(deltaSize.x, 0));
                }
            }
        }

        //获取单元格尺寸
        public Vector2 GetCellSize(int Row, int Column)
        {
            Row += 1;
            Column += 1;

            if (row_Handle.Count > Row && column_Handle.Count > Column)
            {
                Vector2 size = new Vector2(column_Handle[Column].CellSize, row_Handle[Row].CellSize);
                return size;
            }

            return new Vector3(100, 25);
        }

        //获取单元格位置
        public Vector2 GetCellPos(int Row, int Column)
        {
            Row += 1;
            Column += 1;

            if (row_Handle.Count > Row && column_Handle.Count > Column)
            {
                Vector2 size = new Vector2(column_Handle[Column].HandleRect.x, row_Handle[Row].HandleRect.y);
                return size;
            }

            return Vector3.zero;
        }

        //获取表格总尺寸
        public Vector2 GetTotalSize()
        {
            return TotalSize;
        }

        void ChangeTotalSize(Vector2 deltaSize)
        {
            TotalSize += deltaSize;
            //Debug.LogError("Change Total Size : " + TotalSize);
        }

        //增加Handle
        public void AddHandle(bool bRow)
        {
            if (bRow)
            {
                Row++;
                ChangeTotalSize(new Vector2(0, defaultCellSize.y));
            }
            else
            {
                Column++;
                ChangeTotalSize(new Vector2(defaultCellSize.x, 0));
            }

            AddHandle();
        }

        //移除Handle
        public void RemoveHandle(bool bRow)
        {
            if (bRow)
            {
                Row--;
                ChangeTotalSize(new Vector2(0, -row_Handle[Row].CellSize));
                row_Handle.RemoveAt(Row);
            }
            else
            {
                Column--;
                ChangeTotalSize(new Vector2(-column_Handle[Column].CellSize, 0));
                column_Handle.RemoveAt(Column);
            }
        }
    }

    #endregion

    CsvData csvData = null;
    public CsvData GetCsvData()
    {
        return csvData;
    }

    Vector2 defaultCellOffset = new Vector2(100, 25);   //默认单元格坐标偏移值

    CellHandleManager cellHandleManager = null;

    bool eventListenEnable = false;

    public CSVEditWindow()
    {
        this.minSize = new Vector2(600, 400);
        cellHandleManager = new CellHandleManager(this);
    }

    public CSVEditWindow(CsvData data)
    {
        this.minSize = new Vector2(600, 400);
        cellHandleManager = new CellHandleManager(this);

        csvData = data;
        cellHandleManager.SetSize(csvData.MaxRow, csvData.MaxColumn);
    }

    Vector2 ScrollPos = Vector2.zero;

    string Tip = "";

    int selectId = -1;
    private void OnGUI()
    {
        try
        {
            #region 绘制菜单

            GUI.BeginGroup(new Rect(0, 0, Width, 20), "", "Button");
            selectId = GUI.Toolbar(new Rect(0, 0, 240, 20), selectId, new string[] { "文件", "设置", "关闭窗口" }, "button");
            GUI.EndGroup();

            switch (selectId)
            {
                case 0:
                    {
                        SubFileWindow sub = new SubFileWindow(this);
                        sub.ShowAsDropDown(new Rect(this.position.x, this.position.y + 20, 0, 0), SubFileWindow.Size);
                        selectId = -1;
                        break;
                    }
                case 1:
                    {
                        SubSettingWindow sub = new SubSettingWindow(this);
                        sub.ShowAsDropDown(new Rect(this.position.x + (250 / 3), this.position.y + 20, 0, 0), SubSettingWindow.Size);
                        selectId = -1;
                        break;
                    }
                case 2:
                    {
                        this.Close();
                        CsvEditTool.CloseCsvData(csvData);
                        selectId = -1;
                        break;
                    }
            }

            #endregion

            ScrollPos = GUI.BeginScrollView(new Rect(0, 25, Width, Height - 50), ScrollPos,
                new Rect(0, 0, cellHandleManager.GetTotalSize().x + defaultCellOffset.x, cellHandleManager.GetTotalSize().y + defaultCellOffset.y), true, false);
            {
                //绘制Cell Handle
                cellHandleManager.DrawHandle();

                #region 绘制单元格

                for (uint c = 0; c < csvData.MaxColumn; c++)
                {
                    for (uint r = 0; r < csvData.MaxRow; r++)
                    {
                        Vector2 pos = cellHandleManager.GetCellPos((int)r, (int)c);
                        Vector2 size = cellHandleManager.GetCellSize((int)r, (int)c);
                        Rect rc = new Rect(pos.x, pos.y, size.x, size.y);
                        string s = csvData.Read(c, r);
                        csvData.Write(c, r, DrawCell(rc, ref s));
                    }
                }
                #endregion

                if (eventListenEnable)
                {
                    //事件监听
                    cellHandleManager.EventListen();
                }

                //增加行
                if (GUI.Button(new Rect(0, cellHandleManager.GetTotalSize().y, 50, 25), "+", CSVStyleSetting.GlobalHandleStyle))
                {
                    csvData.AddRow();
                    cellHandleManager.AddHandle(true);
                }

                //删除行
                if (GUI.Button(new Rect(50, cellHandleManager.GetTotalSize().y, 50, 25), "—", CSVStyleSetting.GlobalHandleStyle))
                {
                    if (csvData.MaxRow > 1)
                    {
                        csvData.RemoveRow();
                        cellHandleManager.RemoveHandle(true);
                    }
                }

                //增加列
                if (GUI.Button(new Rect(cellHandleManager.GetTotalSize().x, 0, 50, 25), "+", CSVStyleSetting.GlobalHandleStyle))
                {
                    csvData.AddColumn();
                    cellHandleManager.AddHandle(false);
                }

                //删除列
                if (GUI.Button(new Rect(cellHandleManager.GetTotalSize().x + 50, 0, 50, 25), "—", CSVStyleSetting.GlobalHandleStyle))
                {
                    if (csvData.MaxColumn > 1)
                    {
                        csvData.RemoveColumn();
                        cellHandleManager.RemoveHandle(false);
                    }
                }
            }
            GUI.EndScrollView();

            float tiplong = Width - 200;
            GUI.Label(new Rect(100, Height - 20, tiplong, 30), Tip);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            this.Close();
        }
    }

    private void OnDestroy()
    {
        CsvEditTool.CloseCsvData(csvData);
    }

    private void OnFocus()
    {
        eventListenEnable = true;
    }

    private void OnLostFocus()
    {
        eventListenEnable = false;
    }

    //绘制单元格
    string DrawCell(Rect rect, ref string sContent, bool bSelected = false)
    {
        //EditorGUI.TextField
        sContent = EditorGUI.TextField(rect, "", sContent, CSVStyleSetting.GlobalCellStyle);
        return sContent;
    }

    void EventListen()
    {

    }

    float Width
    {
        get
        {
            //Debug.LogError(this.position.width + "   /   " + (this.position.xMax - this.position.xMin));
            return this.position.width;
        }
    }

    float Height
    {
        get { return this.position.height; }
    }

    public void Save()
    {
        if (CsvEditTool.SaveCsvData(csvData))
            ShowTip("最后一次保存于: " + csvData.CsvDataPath + " , at : " + DateTime.Now);
    }

    public void SaveAs()
    {
        string savePath = CsvEditTool.SaveAsCsvData(csvData, this);

        if (savePath != string.Empty)
            ShowTip("另存为于: " + savePath + " , at : " + DateTime.Now);
    }

    public void Exit()
    {
        this.Close();
        CsvEditTool.CloseCsvData(csvData);
    }

    public void ShowTip(string tip)
    {
        Tip = tip;
        this.Focus();
    }
}

namespace _CSVEditWindow.Sub_Window
{
    public class SubFileWindow : EditorWindow
    {
        CSVEditWindow HostWindow = null;

        public static Vector2 Size = new Vector2(150, 170);

        public SubFileWindow(CSVEditWindow HostWindow)
        {
            this.HostWindow = HostWindow;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("新建", "ButtonMid"))
            {
                CSVManageWindow.CreateNewFile();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打开", "ButtonMid"))
            {
                string file = EditorUtility.OpenFilePanel("选择要打开的文件", CSVManageWindow.GetConfigCsvPath(), "csv");
                if (file != string.Empty && file.Contains(".csv"))
                    CSVManageWindow.OpenCSV(file);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("保存", "ButtonMid"))
            {
                HostWindow.Save();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("另存为", "ButtonMid"))
            {
                HostWindow.SaveAs();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("退出编辑", "ButtonMid"))
            {
                HostWindow.Exit();
                this.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
    }

    public class SubSettingWindow : EditorWindow
    {
        CSVEditWindow HostWindow = null;

        public static Vector2 Size = new Vector2(150, 40);

        public SubSettingWindow(CSVEditWindow HostWindow)
        {
            this.HostWindow = HostWindow;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("样式设置", "ButtonMid"))
            {
                CSVStyleSetting.Open();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
    }
}
