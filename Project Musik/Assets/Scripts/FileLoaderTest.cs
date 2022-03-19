using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileLoaderTest 
{
    static FileLoaderTest Save;
    public List<string[]> m_ArrayData;

    public static FileLoaderTest GetInstance()
    {
        if (Save == null)
        {
            Save = new FileLoaderTest();
        }
        return Save;
    }
    FileLoaderTest() { m_ArrayData = new List<string[]>(); }

    public string getString(int row,int col)
    {
        return m_ArrayData [row][col];
    }
    public int getInt(int row, int col)
    {
        return int.Parse(m_ArrayData [row][col]);
    }

    public void loadfile(string path,string fileName)
    {
        m_ArrayData.Clear();
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + fileName);
            Debug.Log("File read with a result of succeed.");
            
        }
        catch (System.Exception)
        {
            Debug.Log("File cannot be read.");
            return;
        }
        string line;
        while((line = sr.ReadLine()) != null)
        {
            m_ArrayData.Add(line.Split(','));
        }
        sr.Close();
        sr.Dispose();
    }
}
