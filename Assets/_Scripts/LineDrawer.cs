using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
  [SerializeField] private LineRenderer lineRenderer;
  [SerializeField] private float showTime = 0.2f;
  [SerializeField] private float lineWidth = 0.1f;

  private void Awake()
  {
    if (lineRenderer == null)
    {
      lineRenderer = GetComponent<LineRenderer>();
    }

    if (lineRenderer == null)
    {
      return;
    }

    if (lineRenderer.sharedMaterial == null)
    {
      Shader shader = Shader.Find("Sprites/Default");
      if (shader != null)
      {
        lineRenderer.material = new Material(shader);
      }
    }

    lineRenderer.startWidth = lineWidth;
    lineRenderer.endWidth = lineWidth;
    lineRenderer.useWorldSpace = true;
    lineRenderer.textureMode = LineTextureMode.Stretch;
    lineRenderer.alignment = LineAlignment.View;
    lineRenderer.positionCount = 0;
  }

  public void DrawLine(List<Vector3> points)
  {
    if (lineRenderer == null) return;
    if (points == null || points.Count == 0) return;

    lineRenderer.positionCount = points.Count;

    for (int i = 0; i < points.Count; i++)
    {
      lineRenderer.SetPosition(i, points[i]);
    }

    StopAllCoroutines();
    StartCoroutine(ClearAfterDelay());
  }

  private IEnumerator ClearAfterDelay()
  {
    yield return new WaitForSeconds(showTime);
    lineRenderer.positionCount = 0;
  }

  public float ShowTime => showTime;
}
