using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndUI : MonoBehaviour
{
    [SerializeField] Text txtTitle;
    [SerializeField] Image endPanelImage;
    [SerializeField] Sprite[] image;
    public void CaseWin()
    {
        txtTitle.text = "Level Completed";
        endPanelImage.sprite = image[0];
    }

    public void CaseLose()
    {
        txtTitle.text = "Level Failed";
        endPanelImage.sprite = image[1];
    }
}
