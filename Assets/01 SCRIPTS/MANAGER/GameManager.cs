using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public bool isStartDrawLine = false;
    [HideInInspector] public bool isStartGame = false;
    Camera cam;
    public AmmoPhysics currentBullet;
    public AmmoPhysics lastToiletRoll;
    public Trajectory trajectory;
    [SerializeField] float pushForce = 4f;

    [Space(10f)]
    [Header("Bool Variable")]
    bool isDragging = false;
    public bool endGame;
    public bool hasRunOutOfAmmo, startShoot;
    
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 direction;
    
    float distanceMaxtoDrawTrajectory;
    float distance;

    [SerializeField]float timerToCheckLose;
    float timer_WhenShootLastAmmo;
    bool isShootLastAmmo;
    
    Vector2 forceMaxtoDrawTrajectory;
    Vector2 force;

    bool delayShot;
    
    //SCript
    [Header("----Script----")]
    [Space(20f)]
    [SerializeField] ChangeAmmoType changeAmmo;
    LoadAllAmmoType loadAllAmmoType;
    ToiletAnimation toiletAnimation;
    HandFollow handFollow;
    [SerializeField] CountDown countDown;
    [SerializeField] ButtonManager buttonManager;
    [SerializeField] StarSystem starSystem;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] ShopUI shopUI;
    [SerializeField] SoundManager soundManager;
    [SerializeField] FirebaseManager firebaseManager;
    [SerializeField] GameTutorial gameTutorial;
    [SerializeField] CameraFollow cameraFollow;

    [Header("Faded Button When Shoot")]
    [Space(20f)]
    [SerializeField] List<Image> faded_Button;
    [SerializeField] Text timer_text, coin_text;
    bool btnStartFaded;
    

    private void Start()
    {
        Application.lowMemory += (() => Resources.UnloadUnusedAssets());
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        cam = Camera.main;
        Input.multiTouchEnabled = false;
        hasRunOutOfAmmo = startShoot = delayShot = isShootLastAmmo = btnStartFaded = false;
        timer_WhenShootLastAmmo = timerToCheckLose;
        soundManager.PlaySound(SoundManager.SoundType.Background);

        shopUI.UpdateCoinText();
        shopUI.GenerateSkinItem();
        shopUI.SetSelectedSkinWhenOpenGame();
    }
    private void Update()
    {
        cameraFollow.ResetZoomAfterThrow();
        if (delayShot) return;
        if (endGame) return;    //het' level
        if (hasRunOutOfAmmo) return; //het' dan.
        if (Input.GetMouseButtonDown(0) && currentBullet != null)
        {
            if (IsPointerOverUIObject()) //check xem chuot. co' dang o? UI khong
            {
                return;
            }
            isDragging = true;
            //loadAllAmmoType.FirstAimingToiletRoll(changeAmmo.GetAmmoType());
            OnDragStart();
        }
        if (Input.GetMouseButtonUp(0) && isDragging) 
        {
            isDragging = false;
            OnDragEnd();
        }
        if (isDragging)
        {
            OnDrag();
            return;
        }
        
    }

    private void FixedUpdate()
    {
        if (lastToiletRoll == null) return;
        //check thang' thua khi toilet roll cuoi' cung` ko di chuyen? nua~ hoac. bay ra ngoai map
        if (isShootLastAmmo)
        {
            timer_WhenShootLastAmmo -= Time.deltaTime;
        }
        if (timer_WhenShootLastAmmo < 0f)
        {
            if (Mathf.Abs(lastToiletRoll.rb.velocity.x) < 0.1f || lastToiletRoll.rb.velocity.y < -40f)
            {
                lastToiletRoll = null;
                toiletAnimation.RunAnimationLose();
                PlayerComplete(false, 1.5f);

                SoundManager.Instance.PlaySound(SoundManager.SoundType.Lose2);
            }
        }
    }

    private void OnDragStart()
    {
        currentBullet.DesactivateRb();
        startPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        startShoot = false;

        handFollow.BeginThrowing(false, 255);

        gameTutorial.EnableTutorialLevel1(false);

        handFollow.StopAllCoroutines();
        soundManager.PlaySound(SoundManager.SoundType.Dragging);

        if(!btnStartFaded)
        {
            btnStartFaded = true;
            foreach(Image x in faded_Button)
            {
                x.color = new Color32(255, 255, 255, 50);
            }
            timer_text.color = new Color32(0, 0, 0, 50);
            coin_text.color = new Color32(106, 0, 133, 50);
        }
    }
    private void OnDrag()
    {
        endPoint = cam.ScreenToWorldPoint(Input.mousePosition);


        distance = Vector2.Distance(startPoint, endPoint) * 0.7f; //giam? toc do keo' day
        if (distance <= 0f) return;

        distanceMaxtoDrawTrajectory = distance;

        if (distanceMaxtoDrawTrajectory > 3f)
        {
            distanceMaxtoDrawTrajectory = 3f;
        }
        cameraFollow.ZoomOutWhenThrow(distanceMaxtoDrawTrajectory);

        direction = (startPoint - endPoint).normalized;
        forceMaxtoDrawTrajectory = direction * distanceMaxtoDrawTrajectory * pushForce;
        force = direction * distanceMaxtoDrawTrajectory * pushForce;
        trajectory.UpdateDots(currentBullet.pos, forceMaxtoDrawTrajectory);
        
        handFollow.ChangeArmDirection(startPoint, direction);

    }
    private void OnDragEnd()
    {
        delayShot = true;
        StartCoroutine(ResetDelayShot());

        currentBullet.ActivateRb();
        trajectory.Hide();
        currentBullet.Push(force);

        //set vien dan. cuoi' cung de check thua neu' vien dan. ko den dich' thi` thua
        if (changeAmmo.GetAmmoType() == 0 && loadAllAmmoType.CountBulletInQueue(0) == 1) 
        {
            lastToiletRoll = currentBullet;
            isShootLastAmmo = true;
        }
        loadAllAmmoType.Fired(changeAmmo.GetAmmoType());
        changeAmmo.Fired();

        //handFollow.PlayAnimationThrow();
        //bat' dau` dem' nguoc.
        startShoot = true;

        soundManager.StopSpecificSound(SoundManager.SoundType.Dragging);
        soundManager.PlaySound(SoundManager.SoundType.EndDrag);

        if (btnStartFaded)
        {
            btnStartFaded = false;
            foreach (Image x in faded_Button)
            {
                x.color = new Color32(255, 255, 255, 255);
            }
            timer_text.color = new Color32(0, 0, 0, 255);
            coin_text.color = new Color32(106, 0, 133, 255);
        }
    }

    public void TurnOnCompletePanel(bool t)
    {
        buttonManager.TurnOnEndUI(t);
    }

    public void PlayerComplete(bool trueIfWon, float second)
    {
        endGame = true;
        changeAmmo.StopAllCoroutines();
        lastToiletRoll = null;
        trajectory.Hide();
        StartCoroutine(TurnOnEndPanelAfterSeconds(trueIfWon, second));
    }

    IEnumerator TurnOnEndPanelAfterSeconds(bool trueIfWon, float second)
    {
        yield return new WaitForSeconds(second);
        buttonManager.EndUICaseWinLose(trueIfWon);
        TurnOnCompletePanel(true);

        //them sao vao` level
        if (trueIfWon)
        {
            firebaseManager.PassLevel(levelLoader.GetCurrentLevel(), starSystem.cur_star);
            
            DATA.AddLevelComplete(levelLoader.GetCurrentLevel(), starSystem.cur_star);

            if (!PlayerPrefs.HasKey("HasOpenRateUs"))
            {
                if ((levelLoader.GetCurrentLevel() + 1) % 5 == 0 && levelLoader.GetCurrentLevel() > 0)
                {
                    buttonManager.rateUsCanvas.SetActive(true);
                }
            }
        }

        shopUI.UpdateCoinText();
        
    }

    IEnumerator ResetDelayShot()
    {
        yield return new WaitForSeconds(.5f);
        delayShot = false;
    }

    public void SetRigidBodyForCurrentBullet(AmmoPhysics paper)
    {
        currentBullet = paper;
    }

    public void RigidBodyOfLastToiletRoll(AmmoPhysics lastRoll)
    {
        lastToiletRoll = lastRoll;
    }

    public void SetLoadAmmoType(LoadAllAmmoType load)
    {
        loadAllAmmoType = load;
    }

    public void SetScriptToiletAnimation(ToiletAnimation t)
    {
        toiletAnimation = t;
    }

    public void SetHandFollow(HandFollow hand)
    {
        handFollow = hand;
    }

    public void ResetEveryThingToDefault()
    {
        hasRunOutOfAmmo = startShoot = endGame = delayShot = isShootLastAmmo = isDragging = false;
        timer_WhenShootLastAmmo = timerToCheckLose;
        TurnOnCompletePanel(false);
        changeAmmo.ResetAmmoTypeAndButton();
        countDown.ResetAll();
        soundManager.StopStarSound();
        buttonManager.StopAllCoroutines();
        StopAllCoroutines();

    }

    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        //check touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                return true;
        }
        return false;
    }

    public void ChangePlayerSkin()
    {
        handFollow.ChangePlayerSkin();
    }

    public void ChangePlayerSkinWhenSpawnLevel()
    {
        ChangePlayerSkin();
    }

    public ChangeAmmoType GetChangeAmmoType()
    {
        return changeAmmo;
    }

    public CameraFollow GetCameraFollow()
    {
        return cameraFollow;
    }

    public StarSystem GetStarSystem()
    {
        return starSystem;
    }
}
