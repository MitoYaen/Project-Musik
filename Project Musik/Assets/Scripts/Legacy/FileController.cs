using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileController : MonoBehaviour
{
    public string FileName;
    void Start()
    {
        FileLoaderTest.GetInstance().loadfile(Application.dataPath + "/Resources", FileName + ".csv");
        Debug.Log(FileLoaderTest.GetInstance().getString(1, 1));
        Debug.Log(FileLoaderTest.GetInstance().getInt(1, 2));

    }

}
