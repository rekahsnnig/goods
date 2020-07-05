using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class readCSV_outStruct : MonoBehaviour {
    public Shader StructureShader;
    public ComputeShader pointComputeShader;
    public Material strmaterial;

    ComputeBuffer strBuffer1;
    ComputeBuffer strBuffer2;

    VertexData[] csv1;

    List<List<string[]>> csvDatas1 = new List<List<string[]>> () {
        new List<string[]> (),
        new List<string[]> (),
        new List<string[]> ()
    };

    List<List<string[]>> csvDatas2 = new List<List<string[]>> () {
        new List<string[]> (),
        new List<string[]> (),
        new List<string[]> ()
    };

    List<VertexData[]> vertexList1 = new List<VertexData[]> ();
    List<VertexData[]> vertexList2 = new List<VertexData[]> ();


    void OnDisable () {
        strBuffer1.Release ();
        strBuffer2.Release ();
    }
    void Start () {
        TextAsset csv1 = Resources.Load ("vertices1") as TextAsset;
        int count = InitializeStringList1 (csv1, 0);
        TextAsset csv2 = Resources.Load ("vcolors1") as TextAsset;
        int vccount = InitializeStringList1 (csv2, 1);
        TextAsset csv3 = Resources.Load ("uvs1") as TextAsset;
        int uvcount = InitializeStringList1 (csv3, 2);

        TextAsset csv4 = Resources.Load ("vertices2") as TextAsset;
        count = InitializeStringList2 (csv4, 0);
        TextAsset csv5 = Resources.Load ("vcolors2") as TextAsset;
        vccount = InitializeStringList2 (csv5, 1);
        TextAsset csv6 = Resources.Load ("uvs2") as TextAsset;
        uvcount = InitializeStringList2 (csv6, 2);
      //  print(uvcount);
        InitializeStructList (count, vccount, uvcount);
    }

    void Update () {
        
    }

    int InitializeStringList1 (TextAsset csv, int listIndex) {
        StringReader reader = new StringReader (csv.text);
        int vertexCount = 0;
        while (reader.Peek () != -1) {
            string line = reader.ReadLine ();
            if (line.Length < 1) { continue; }
            csvDatas1[listIndex].Add (line.Split (','));
            vertexCount += 1;
        }
        return vertexCount;
    }

     int InitializeStringList2 (TextAsset csv, int listIndex) {
        StringReader reader = new StringReader (csv.text);
        int vertexCount = 0;
        while (reader.Peek () != -1) {
            string line = reader.ReadLine ();
            if (line.Length < 1) { continue; }
            csvDatas2[listIndex].Add (line.Split (','));
            vertexCount += 1;
        }
        return vertexCount;
    }

    void InitializeStructList (int vertexCount, int vccount, int uvcount) {
        VertexData[] vertexData1 = new VertexData[vertexCount];
        VertexData[] vertexData2 = new VertexData[vertexCount];
        strBuffer1 = new ComputeBuffer (vertexCount, Marshal.SizeOf (typeof (VertexData)));
        strBuffer2 = new ComputeBuffer (vertexCount, Marshal.SizeOf (typeof (VertexData)));
        Vector3 position = Vector3.zero;
        Vector3 vColor = Vector3.zero;
        Vector2 uv = Vector2.zero;
        for (int i = 0; i < vertexCount; i++) {
            position = this.transform.position;
            position.x = -(float) double.Parse (csvDatas1[0][i][0]) * 3.0f;
            position.z = -(float) double.Parse (csvDatas1[0][i][1]) * 3.0f;
            position.y = (float) double.Parse (csvDatas1[0][i][2]) * 3.0f;

            if (i < csvDatas1[1].Count) {
                vColor.x = (float) double.Parse (csvDatas1[1][i][0]);
                vColor.y = (float) double.Parse (csvDatas1[1][i][1]);
                vColor.z = (float) double.Parse (csvDatas1[1][i][2]);
            }
            if (i < csvDatas1[2].Count) 
            {
                uv.x = (float) double.Parse (csvDatas1[2][i][0]);
                uv.y = (float) double.Parse (csvDatas1[2][i][1]);
            //    print(uv);
            }

            if((position).magnitude == 0.0f)
            {
                position =  new Vector3(Random.Range(-50.0f,50.0f),Random.Range(-50.0f,50.0f),Random.Range(-50.0f,50.0f));
                uv = new Vector2(-1.0f,-1.0f);
            }

            vertexData1[i] = new VertexData (
                position,
                vColor,
                uv,
                vccount,
                uvcount
            );
        
            position = this.transform.position;
            position.x = -(float) double.Parse (csvDatas2[0][i][0]);
            position.z = (float) double.Parse (csvDatas2[0][i][1]);
            position.y = (float) double.Parse (csvDatas2[0][i][2]) + 12.0f;

            


            if (i < csvDatas2[1].Count) {
                vColor.x = (float) double.Parse (csvDatas2[1][i][0]);
                vColor.y = (float) double.Parse (csvDatas2[1][i][1]);
                vColor.z = (float) double.Parse (csvDatas2[1][i][2]);
            }
            if (i < csvDatas2[2].Count) 
            {
                uv.x = (float) double.Parse (csvDatas2[2][i][0]);
                uv.y = (float) double.Parse (csvDatas2[2][i][1]);
            //    print(uv);
            }

            if((position).magnitude == 0.0f)
            {
                position =  new Vector3(Random.Range(-50.0f,50.0f),Random.Range(-50.0f,50.0f),Random.Range(-50.0f,50.0f));
                uv = new Vector2(-1.0f,-1.0f);
            }
            vertexData2[i] = new VertexData (
                position,
                vColor,
                uv,
                vccount,
                uvcount
            );

        }

        strBuffer1.SetData (vertexData1);
        strBuffer2.SetData (vertexData2);
    }

    private void OnRenderObject () {
        strmaterial.SetBuffer ("VertexDatas1", strBuffer1);
        strmaterial.SetBuffer ("VertexDatas2", strBuffer2);
        strmaterial.SetPass (0);
        Graphics.DrawProcedural (MeshTopology.Points, strBuffer1.count);
    }
}
