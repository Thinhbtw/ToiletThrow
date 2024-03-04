using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
    Vector2 viewPortSize;
    [SerializeField] Camera cam;

    Vector3 targetPos;

    Vector3 curVelocity;
    public float followDuration;
    public float maximumFollowSpeed;

    public Transform toiletRoll;
    [SerializeField] GameObject IngameUI;
    bool stillZoom, resetWhenZooming;
    // Update is called once per frame
    void LateUpdate()
    {
        if (toiletRoll == null) return;

        targetPos = toiletRoll.position - new Vector3(0, 0, 10);
        transform.position = targetPos;
        //transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref curVelocity, followDuration, maximumFollowSpeed);
    }

    public void SetCamZoom(float cameraSize)
    {
        cam.orthographicSize = cameraSize;
    }
    public void SlowMoAndZoomCam()
    {
        StartSlowMotion();
        SetCamZoom(1f);
    }

    public void ResetCamToDefault(float cameraSize)
    {
        StopSlowMotion();
        SetCamZoom(cameraSize);
        toiletRoll = null;
        transform.position = new Vector3(0, 0, -10);
    }

    public void ZoomOutWhenThrow(float camSize)
    {
        cam.orthographicSize = Mathf.Lerp(3, 3 + (camSize/3), Time.timeScale);
    }

    public void ResetZoomAfterThrow()
    {
        if (stillZoom) return;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 3, Time.timeScale * 0.1f);
    }

    public float slowMotionTimescale;

    private float startTimescale;
    private float startFixedDeltaTime;

    void Start()
    {
        startTimescale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimescale;
        stillZoom = true;
        IngameUI.SetActive(false);
    }

    private void StopSlowMotion()
    {
        Time.timeScale = startTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime;
        stillZoom = false;
        IngameUI.SetActive(true);
    }

}
