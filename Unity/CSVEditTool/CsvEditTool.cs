using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;

/// <summary>
/// CSV编辑工具类
/// </summary>
public class CsvEditTool : MonoBehaviour
{
    static Dictionary<string, CsvData> CurrentCsvDatas = new Dictionary<string, CsvData>();

    public static CsvData OpenCsvData(string sCsvDataPath)
    {
        if (CurrentCsvDatas.ContainsKey(sCsvDataPath))
            return CurrentCsvDatas[sCsvDataPath];

        CsvData data = new CsvData(sCsvDataPath);
        data.Open(sCsvDataPath);
        CurrentCsvDatas.Add(sCsvDataPath, data);
        return CurrentCsvDatas[sCsvDataPath];
    }

    public static bool SaveCsvData(CsvData csvData)
    {
        return csvData.Save();
    }

    public static string SaveAsCsvData(CsvData csvData, CSVEditWindow window)
    {
        string fileName = EditorUtility.SaveFilePanel("选择保存文件位置", CSVManageWindow.GetConfigCsvPath(), window.titleContent.text, "csv");
        if (fileName == string.Empty)
        {
            return string.Empty;
        }

        //char[] cfileName = fileName.ToCharArray();
        //cfileName[fileName.LastIndexOf('/')] = '\\';
        //fileName = new string(cfileName);
        //Debug.LogError(fileName);
        return csvData.SaveAs(fileName);
    }

    public static bool CloseCsvData(CsvData data)
    {
        if (data == null)
            return false;

        if (!CurrentCsvDatas.ContainsKey(data.CsvDataPath))
            return false;

        CurrentCsvDatas.Remove(data.CsvDataPath);
        data.Close();

        return true;
    }
}

/// <summary>
/// CSV数据
/// </summary>
public class CsvData
{
    public CsvData(string csvDataPath)
    {
        this.CsvDataPath = csvDataPath;
    }

    string csvDataPath = "";
    public string CsvDataPath
    {
        get { return csvDataPath; }
        private set
        {
            csvDataPath = value;
        }
    }

    //数据结构
    Dictionary<uint, Dictionary<uint, string>> csvData = new Dictionary<uint, Dictionary<uint, string>>();

    FileStream fs = null;
    StringReader sr = null;
    StreamWriter sw = null;

    public int MaxRow = 0;
    public int MaxColumn = 0;

    /// <summary>
    /// 写数据
    /// </summary>
    /// <param name="uColumn"></param>
    /// <param name="uRow"></param>
    /// <param name="sValue"></param>
    public void Write(uint uColumn, uint uRow, string sValue)
    {
        if (!csvData.ContainsKey(uColumn))
        {//添加新的列
            csvData.Add(uColumn, new Dictionary<uint, string>());
        }

        //增加新的行
        var rowData = csvData[uColumn];
        if (rowData.ContainsKey(uRow))
        {
            rowData[uRow] = sValue;
        }
        else
        {
            rowData.Add(uRow, sValue);
        }
    }

    /// <summary>
    /// 读数据
    /// </summary>
    /// <param name="uColumn"></param>
    /// <param name="uRow"></param>
    /// <returns></returns>
    public string Read(uint uColumn, uint uRow)
    {
        if (csvData.ContainsKey(uColumn) && csvData[uColumn].ContainsKey(uRow))
        {
            return csvData[uColumn][uRow];
        }

        return string.Empty;
    }

    public void Remove(uint uColumn, uint uRow)
    {
        if (csvData.ContainsKey(uColumn))
        {
            if (csvData[uColumn].ContainsKey(uRow))
            {
                csvData[uColumn].Remove(uRow);
            }

            if (csvData[uColumn].Count == 0)
            {
                csvData.Remove(uColumn);
            }
        }
    }

    public void Open(string sPath)
    {
        if (fs == null)
        {
            fs = new FileStream(sPath, FileMode.Open);
        }

        if (fs != null)
        {
            byte[] b = new byte[fs.Length];
            //Debug.LogError(fs.ReadByte());
            fs.Read(b, 0, b.Length);

            string sb = System.Text.Encoding.UTF8.GetString(b);
            sr = new StringReader(sb);

            //释放filestream
            fs.Dispose();
            fs = null;
        }

        if (sr != null)
        {
            uint uRow = 0;

            //Debug.LogError(sr.Peek());
            //Debug.LogError(sr.Peek());

            while (sr.Peek() >= 0)
            {
                string sLine = sr.ReadLine();
                Debug.Log(sLine);
                string[] sRowData = sLine.Split(',');

                if (sRowData.Length > MaxColumn)
                    MaxColumn = sRowData.Length;

                if (sRowData != null)
                {
                    for (uint uColumn = 0; uColumn < sRowData.Length; uColumn++)
                    {
                        //Debug.Log(string.Format("Write CSV Data : R: {0} -- C: {1} -- V: {2}", r, uColume, sRowData[r]));
                        Write(uColumn, uRow, sRowData[uColumn]);
                    }
                }

                uRow++;
                //Debug.LogError(sr.Peek());
            }

            MaxRow = (int)uRow;

            //释放流
            sr.Dispose();
            sr = null;
        }
        else
        {
            Debug.LogError("SR == NULL");
        }
    }

    public void Close()
    {
        if (sr != null)
        {
            sr.Dispose();
        }

        if (sw != null)
        {
            sw.Dispose();
        }
    }

    public void AddRow()
    {
        MaxRow += 1;

        for (uint c = 0; c < MaxColumn; c++)
        {
            Write(c, (uint)MaxRow, "");
        }
    }

    public void AddColumn()
    {
        MaxColumn += 1;

        for (uint r = 0; r < MaxColumn; r++)
        {
            Write((uint)MaxColumn, r, "");
        }
    }

    public void RemoveRow()
    {
        MaxRow -= 1;

        for (uint c = 0; c < MaxColumn; c++)
        {
            Remove(c, (uint)MaxRow);
        }
    }

    public void RemoveColumn()
    {
        MaxColumn -= 1;

        for (uint r = 0; r < MaxRow; r++)
        {
            Remove((uint)MaxColumn, r);
        }
    }

    public bool Save()
    {
        if (CsvDataPath == string.Empty)
        {
            return false;
        }

        int index = CsvDataPath.LastIndexOf('/') + 1;
        string[] cPath = { CsvDataPath.Substring(0, index), CsvDataPath.Substring(index, CsvDataPath.Length - index) };
        cPath[1] = cPath[1].Replace(".csv", "_temp.csv");
        string tempPath = cPath[0] + cPath[1];

        if (File.Exists(tempPath))
            File.Delete(tempPath);

        fs = File.Create(tempPath);
        sw = new StreamWriter(fs);

        for (uint r = 0; r < MaxRow; r++)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Read(0, r));
            for (uint c = 1; c < MaxColumn; c++)
            {
                string v = Read(c, r);
                sb.Append(",");
                sb.Append(v);
            }

            sw.WriteLine(sb.ToString());
        }

        sw.Flush();
        sw.Dispose();

        if (File.Exists(CsvDataPath))
            File.Delete(CsvDataPath);

        File.Move(tempPath, CsvDataPath);

        return true;
    }

    public string SaveAs(string savePath)
    {
        string o = CsvDataPath;
        CsvDataPath = savePath;
        bool b = false;

        try
        {
            b = Save();
        }
        catch
        {
            b = false;
        }

        CsvDataPath = o;
        return b ? savePath : string.Empty;
    }
}
