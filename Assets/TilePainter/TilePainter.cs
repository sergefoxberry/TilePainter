using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TilePainter : MonoBehaviour
{
    public static TilePainter instance;
    private Vector3 cmCurPos;
    private Vector2 drawBox;
    private Ray ray;
    private RaycastHit hit;

    [SerializeField]
    private Transform transformParent;

    public GameObject[] prefabs;

    public int brushId, layer;

    private bool isEnabledPriv;

    public bool isEnabled()
    {
        return isEnabledPriv;
    }

    private void Awake()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        if (instance == null) instance = this; else if (instance != this) { Destroy(instance.gameObject); instance = this; }
    }

    private void OnEnable()
    {
        //SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    public void Enable()
    {
        isEnabledPriv = !isEnabledPriv;
        if (isEnabledPriv)
        {
#if UNITY_EDITOR
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            print("subscribe");
# endif
        }
        else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    private void Start()
    {
        prefabs = Resources.LoadAll("PrefabsBlocks", typeof(GameObject))
             .Cast<GameObject>()
             .ToArray();
        print(prefabs.Length);
    }

    private void OnSceneGUI(SceneView sceneview)
    {
        if (instance == null) instance = this; else if (instance != this) { Destroy(instance.gameObject); instance = this; }

        Event e = Event.current;

        if (e != null && instance != null)
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                RaycastHit2D hit2d = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit2d)
                {
                    GameObject g = GameObject.Find("tile_" + "L" + layer + "_X" + cmCurPos.x + "Y" + cmCurPos.y);
                    GameObject brush = (GameObject)prefabs[0];
                    if (g != null) DestroyImmediate(g);
                }

                if (brushId != 999) // 999 - delete block
                {
                    SpawnTile();
                }
            }
            else if (e.isMouse && e.type == EventType.MouseMove)
            {
                ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                if (e.button == 0 && e.type == EventType.MouseDown)
                {
                    drawBox.x = Mathf.Floor(hit.point.x);
                    drawBox.y = Mathf.Floor(hit.point.y);
                }
                else if (e.type == EventType.MouseMove)
                {
                    drawBox.x = Mathf.Floor(hit.point.x);
                    drawBox.y = Mathf.Floor(hit.point.y);
                }
                cmCurPos.x = (float)Mathf.Floor(hit.point.x);
                cmCurPos.y = (float)Mathf.Floor(hit.point.y);
            }
        }

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.Label(cmCurPos, " ", EditorStyles.boldLabel);

            if ((cmCurPos.x != drawBox.x || cmCurPos.y != drawBox.y))
            {
                if (cmCurPos.x >= drawBox.x)
                {
                    if (cmCurPos.y <= drawBox.y)
                    {
                        Handles.DrawLine(new Vector3(drawBox.x, drawBox.y + 1, 0), new Vector3(cmCurPos.x + 1, drawBox.y + 1, 0));
                        Handles.DrawLine(new Vector3(cmCurPos.x + 1, drawBox.y + 1, 0), new Vector3(cmCurPos.x + 1, cmCurPos.y, 0));
                        Handles.DrawLine(new Vector3(cmCurPos.x + 1, cmCurPos.y, 0), new Vector3(drawBox.x, cmCurPos.y, 0));
                        Handles.DrawLine(new Vector3(drawBox.x, cmCurPos.y, 0), new Vector3(drawBox.x, drawBox.y + 1, 0));
                    }
                    else
                    {
                        Handles.DrawLine(new Vector3(drawBox.x, drawBox.y, 0), new Vector3(cmCurPos.x + 1, drawBox.y, 0));
                        Handles.DrawLine(new Vector3(cmCurPos.x + 1, drawBox.y, 0), new Vector3(cmCurPos.x + 1, cmCurPos.y, 0));
                        Handles.DrawLine(new Vector3(cmCurPos.x + 1, cmCurPos.y, 0), new Vector3(drawBox.x, cmCurPos.y, 0));
                        Handles.DrawLine(new Vector3(drawBox.x, cmCurPos.y, 0), new Vector3(drawBox.x, drawBox.y, 0));
                    }
                }
                else
                {
                    if (cmCurPos.y <= drawBox.y)
                    {
                        Handles.DrawLine(new Vector3(drawBox.x + 1, drawBox.y + 1, 0), new Vector3(cmCurPos.x + 1, drawBox.y + 1, 0));
                        Handles.DrawLine(new Vector3(cmCurPos.x + 1, drawBox.y + 1, 0), new Vector3(cmCurPos.x + 1, cmCurPos.y, 0));
                        Handles.DrawLine(new Vector3(cmCurPos.x + 1, cmCurPos.y, 0), new Vector3(drawBox.x + 1, cmCurPos.y, 0));
                        Handles.DrawLine(new Vector3(drawBox.x + 1, cmCurPos.y, 0), new Vector3(drawBox.x + 1, drawBox.y + 1, 0));
                    }
                    else
                    {
                        Handles.DrawLine(new Vector3(drawBox.x + 1, drawBox.y, 0), new Vector3(cmCurPos.x + 1, drawBox.y, 0));
                        Handles.DrawLine(new Vector3(cmCurPos.x + 1, drawBox.y, 0), new Vector3(cmCurPos.x + 1, cmCurPos.y, 0));
                        Handles.DrawLine(new Vector3(cmCurPos.x + 1, cmCurPos.y, 0), new Vector3(drawBox.x + 1, cmCurPos.y, 0));
                        Handles.DrawLine(new Vector3(drawBox.x + 1, cmCurPos.y, 0), new Vector3(drawBox.x + 1, drawBox.y, 0));
                    }
                }
            }
            else
            {
                Handles.DrawLine(cmCurPos + new Vector3(0, 0, 0), cmCurPos + new Vector3(1, 0, 0));
                Handles.DrawLine(cmCurPos + new Vector3(1, 0, 0), cmCurPos + new Vector3(1, 1, 0));
                Handles.DrawLine(cmCurPos + new Vector3(1, 1, 0), cmCurPos + new Vector3(0, 1, 0));
                Handles.DrawLine(cmCurPos + new Vector3(0, 1, 0), cmCurPos + new Vector3(0, 0, 0));
            }
            Handles.EndGUI();
        }
    }

    private void SpawnTile()
    {
        if (transformParent == null) transformParent = FindObjectOfType<TilePainter>().gameObject.transform;
        var t = Instantiate(prefabs[brushId], drawBox + new Vector2(0.5f, 0.5f), Quaternion.identity, transformParent) as GameObject;
        t.name = "tile_" + "L" + layer + "_X" + cmCurPos.x + "Y" + cmCurPos.y;
        t.GetComponent<SpriteRenderer>().sortingOrder = layer;
    }
}