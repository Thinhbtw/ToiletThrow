using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation;
    Transform thisTrans;
    public float percentThreshold = .2f;
    public float easing = 0.5f;
    public int totalPages = 1;
    private int currentPage = 1;
    int lastPage;
    [SerializeField] List<Vector3> allPanelPos = new List<Vector3>();
    [SerializeField] List<GameObject> allPanel = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        thisTrans = transform;
        panelLocation = transform.localPosition;
        LevelSelector.Instance.MoveToPanel();
        TurnOffFarPanel();
    }


    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        thisTrans.localPosition = panelLocation - new Vector3(difference, 0, 0);
    }
    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0 && currentPage < totalPages)
            {
                lastPage = currentPage;
                currentPage++;
                newLocation = new Vector3(-allPanelPos[currentPage - 1].x, 0, 0);
            }
            else if (percentage < 0 && currentPage > 1)
            {
                lastPage = currentPage;
                currentPage--;
                newLocation = new Vector3(-allPanelPos[currentPage - 1].x, 0, 0);
            }
            StartCoroutine(SmoothMove(thisTrans.localPosition, newLocation, easing));
            panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(thisTrans.localPosition, panelLocation, easing));
        }
        TurnOffFarPanel();
    }
    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            thisTrans.localPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    public void AddPositionToListPanel(Vector3 localPos, GameObject gameObject)
    {
        allPanelPos.Add(localPos);
        allPanel.Add(gameObject);
    }

    public void TurnOffFarPanel()
    {
        switch (currentPage)
        {
            case 1:
                allPanel[currentPage - 1].gameObject.SetActive(true);
                allPanel[1].gameObject.SetActive(true);
                if (totalPages < 2) break;
                for (int i = 2; i<totalPages;i++)
                {
                    allPanel[i].gameObject.SetActive(false);
                }
                break;
            case int x when x == totalPages:
                allPanel[x - 2].gameObject.SetActive(true);
                allPanel[x - 1].gameObject.SetActive(true);
                if (totalPages < 2) break;
                for (int i = 0; i < x-2; i++)
                {
                    allPanel[i].gameObject.SetActive(false);
                }
                break;

            case int x when Mathf.Abs(x - lastPage) == 1:   //page 1 2 3 tuong duong voi' allPanel 0 1 2 || x = 2 thi` tuong duong allPanel[1]
                allPanel[x - 2].gameObject.SetActive(true);
                allPanel[x - 1].gameObject.SetActive(true);
                allPanel[x].gameObject.SetActive(true); 
                for (int i = 0; i < totalPages; i++)
                {
                    if (i == x || i == x - 1 || i == x - 2) continue;

                    allPanel[i].gameObject.SetActive(false);
                }
                break;

            case int x when Mathf.Abs(x - lastPage) > 1:
                allPanel[x - 1].gameObject.SetActive(true);
                allPanel[x - 2].gameObject.SetActive(true);
                for (int i = 0; i < totalPages; i++)
                {
                    if (i == x || i == x - 1 || i == x - 2) continue;

                    allPanel[i].gameObject.SetActive(false);
                }
                break;
        }
    }

    public void MoveToRightPanel()
    {
        if (currentPage == totalPages) return;
        lastPage = currentPage;
        currentPage++;
        MovePanel();
    }

    public void MoveToLeftPanel()
    {
        if (currentPage == 1) return;
        lastPage = currentPage;
        currentPage--;
        MovePanel();
    }
    private void MovePanel()
    {
        panelLocation = new Vector3(-allPanelPos[currentPage - 1].x, 0, 0);
        StartCoroutine(SmoothMove(thisTrans.localPosition, new Vector3(-allPanelPos[currentPage - 1].x, 0, 0), easing));
        TurnOffFarPanel();
    }

    public void MoveToCurrentLevel(int index)
    {
        lastPage = currentPage;
        currentPage = index;
        panelLocation = new Vector3(-allPanelPos[currentPage - 1].x, 0, 0);
        StartCoroutine(SmoothMove(thisTrans.localPosition, new Vector3(-allPanelPos[currentPage - 1].x, 0, 0), easing));
        TurnOffFarPanel();
    }
}