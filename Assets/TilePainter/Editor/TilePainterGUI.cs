using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

public class TilePainterGUI : EditorWindow
{
    string fileName = "FileName";

    string status = "Idle";
    string recordButton = "Record";
    bool recording = false;
    float lastFrameTime = 0.0f;
    int capturedFrame = 0;
    private TilePainter tm;


    [MenuItem("TilePainter/TilePainter GUI")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TilePainterGUI));
    }
    
    void OnGUI()
    {
        if (TilePainter.instance == null)
        {
            if (FindObjectOfType<TilePainter>() != null) return;

            GameObject[] g = Resources.LoadAll("TilePainterBase", typeof(GameObject))
             .Cast<GameObject>()
             .ToArray();

            var tpm = Instantiate(g[0]);
            tpm.name = "TilePainter";
        }

        fileName = EditorGUILayout.TextField("File Name:", fileName);
        if (TilePainter.instance != null)
        {
            if (GUILayout.Button(TilePainter.instance.isEnabled() ? "[ 1 |  ] Disable" : "[   | 0 ] Enable"))
            {
                TilePainter.instance.Enable();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("L0"))
            {
                TilePainter.instance.layer = 0;
            }
            if (GUILayout.Button("L1"))
            {
                TilePainter.instance.layer = 1;
            }
            if (GUILayout.Button("L2"))
            {
                TilePainter.instance.layer = 2;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < (TilePainter.instance != null ? TilePainter.instance.prefabs.Length : 0); i++)
            {

                if (GUILayout.Button(TilePainter.instance.prefabs[i].GetComponent<SpriteRenderer>().sprite.texture ))
                {
                    TilePainter.instance.brushId = i;
                }
                if ((i+1) % 8  == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }

            
            if (GUILayout.Button("[X] Cleaner"))
            {
                TilePainter.instance.brushId = 999;
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();


        }
        EditorGUILayout.LabelField("Status: ", status);
    }


}
 