using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    [HideInInspector] public List<Vector2> points = new List<Vector2>();
    [HideInInspector] public int pointsCount = 0;

    [Header("Line Information")]
    [SerializeField] float LineLength = 0;


    float pointsMinDistance = 0.1f;

    /*private void Update() 
    {
        if (GameManager.Instance.startShoot)
            AddPoint(this.transform.parent.position);
    }*/

    public void AddPoint(Vector2 newpoint)
    {
        if (pointsCount > 1 && Vector2.Distance(newpoint, GetLastPoint()) < pointsMinDistance)
            return;
        if (pointsCount > 1)
            SetLineLength(GetLastPoint(), newpoint);
        points.Add(newpoint);
        pointsCount++;

        //Line Renderer
        lineRenderer.positionCount = pointsCount;
        lineRenderer.SetPosition(pointsCount - 1, newpoint);

    }
    public void SetLineLength(Vector2 lastpoint, Vector2 newpoint)
    {
        LineLength += Vector2.Distance(lastpoint, newpoint);
    }
    public float GetLineLength()
    {
        return LineLength;
    }
    public Vector2 GetLastPoint()
    {
        return (Vector2)lineRenderer.GetPosition(pointsCount - 1);
    }
    public int GetLineRendererCount()
    {
        return lineRenderer.positionCount;
    }
    public void SetLineColor(Gradient colorGradient)
    {
        lineRenderer.colorGradient = colorGradient;
    }
    public void SetPointsMinDistance(float distance)
    {
        pointsMinDistance = distance;
    }
    public void SetLineWidth(float width)
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }
}
