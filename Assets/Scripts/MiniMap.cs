using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private GameObject TrackPath;

    void Start()
    {
        Debug.Log("MiniMap :O");
        // lineRenderer = GetComponent<LineRenderer>();
        TrackPath = this.gameObject;

        int path_num = TrackPath.transform.childCount;
        if (path_num > 0)
        {
            lineRenderer.positionCount = path_num;

            for (int i = 0; i < path_num; i++)
            {
                Vector3 childPosition = TrackPath.transform.GetChild(i).transform.position;
                lineRenderer.SetPosition(i, new Vector3(childPosition.x, 50, childPosition.z));
            }

            lineRenderer.SetPosition(path_num - 1, lineRenderer.GetPosition(0));
        }
        else
        {
            Debug.LogWarning("TrackPath has no children.");
        }

        // lineRenderer.startWidth = 10f;
        // lineRenderer.endWidth = 10f;
    }

    public void SetSize(float startWidth, float endWidth)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = endWidth;
        }
        else
        {
            Debug.LogError("LineRenderer is not assigned.");
        }
    }
}
