using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /*[SerializeField] public Vector2 velocity;

    public float smoothTime = 0;
    [SerializeField] float distanceCheck = 0;
    [SerializeField] public Line line;

    int posIndex = 0;

    private void FixedUpdate()
    {
        if (posIndex < line.pointsCount && GameManager.Instance.isStartGame)
        {
            transform.position = Vector2.MoveTowards(transform.position, line.points[posIndex], smoothTime);
            posIndex++;
            if (posIndex < line.pointsCount)
            {
                if ((Mathf.Abs(transform.localPosition.x) - Mathf.Abs(Camera.main.ScreenToWorldPoint(line.points[posIndex]).x)) >= 4f)
                    this.transform.localScale = new Vector2(1f, 1f);
                else
                    this.transform.localScale = new Vector2(-1f, 1f);
            }

        }
    }*/
}
