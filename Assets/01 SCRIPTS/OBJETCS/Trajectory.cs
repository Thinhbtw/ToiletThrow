using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] int dotNumber;
    [SerializeField] GameObject dotsParent;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] float dotSpacing;
    [SerializeField] [Range(0.01f, 0.1f)] float dotMinScale;
    [SerializeField] [Range(0f, 0.1f)] float dotMaxScale;

    Vector2 pos;
    float timeStamp;
    Transform[] dotsList;
    bool show;

    private void Start()
    {
        Hide();
        PrepareDots();
    }

    void PrepareDots()
    {
        dotsList = new Transform[dotNumber];
        dotPrefab.transform.localScale = Vector3.one * dotMaxScale;

        float scale = dotMaxScale;
        float scaleFactor = scale / dotNumber;

        for (int i = 0; i < dotNumber; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, null).transform;
            dotsList[i].parent = dotsParent.transform;

            dotsList[i].localScale = Vector3.one * scale;
            if (scale > dotMinScale)
                scale -= scaleFactor;
        }
    }
    public void UpdateDots(Vector3 ballPos, Vector2 forceApplied)
    {
        if(!show)
        {
            Show();
        }
        timeStamp = dotSpacing;
        for (int i = 0; i < dotNumber; i++)
        {
            pos.x = (ballPos.x + forceApplied.x * timeStamp);
            pos.y = (ballPos.y + forceApplied.y * timeStamp) - (Physics2D.gravity.magnitude * timeStamp * timeStamp) / 2f;
            dotsList[i].position = pos;
            timeStamp += dotSpacing;
        }
    }
    public void Show()
    {
        dotsParent.SetActive(true);
        show = true;
    }
    public void Hide()
    {
        dotsParent.SetActive(false);
        show = false;
    }
}
