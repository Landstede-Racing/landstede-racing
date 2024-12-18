using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private GameObject TrackPath;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        TrackPath = this.gameObject;

        int path_num = TrackPath.transform.childCount;
        lineRenderer.positionCount = path_num;

        for (int i = 0; i < path_num; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(TrackPath.transform.GetChild(i).transform.position.x,
            50, TrackPath.transform.GetChild(i).transform.position.z));
        }

        lineRenderer.SetPosition(path_num, lineRenderer.GetPosition(0)); //for aligning 

        lineRenderer.startWidth = 10f;
        lineRenderer.endWidth = 10f;

        // lineRenderer.numCornerVertices = 10;
    }

    void Update()
    {

    }
}