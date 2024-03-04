using DG.Tweening;
using UnityEngine;
using System.Collections;

public class AmmoPhysics : MonoBehaviour
{
    public Rigidbody2D rb;
    public CircleCollider2D cir2D;
    [SerializeField] AmmoType type;
    public SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] img_ammo;
    [SerializeField] Sprite[] toiletPaper_Faces;
    bool hasBrokeGlass, hasZoom, firstSpawnLevel;
    public bool notInThisLevel;
    CameraFollow cameraFollow;
    [SerializeField] LoadAllAmmoType loadAllAmmo;
    

    public Vector3 pos
    {
        get { return transform.position; }
    }

    GameManager gameManager;
    StarSystem starSystem;
    private void OnEnable()
    {
        if (!firstSpawnLevel)
        {
            gameManager = GameManager.Instance;
            firstSpawnLevel = true;
        }
        else
        {
            gameManager = loadAllAmmo.gameManager;
        }
        starSystem = gameManager.GetStarSystem();
        cameraFollow = gameManager.GetCameraFollow();
        cameraFollow.ResetCamToDefault(3f);

        cir2D.isTrigger = true;
        hasZoom = false;
        switch(type)
        {
            case AmmoType.ToiletRoll:
                this.transform.localScale = new Vector2(0.4f, 0.4f);
                spriteRenderer.sprite = img_ammo[0];
            break;
            case AmmoType.Stone:
                this.transform.localScale = new Vector2(0.2f, 0.2f);
                spriteRenderer.sprite = img_ammo[1];
            break;
            case AmmoType.Bomb:
                this.transform.localScale = new Vector2(0.2f, 0.2f);
                spriteRenderer.sprite = img_ammo[1];
                break;
        }
    }

    public void Push(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);

        if (type == AmmoType.ToiletRoll)
        {
            spriteRenderer.sprite = toiletPaper_Faces[1];
        }
        //bat trail renderer
        this.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void ActivateRb()
    {
        rb.isKinematic = cir2D.isTrigger = false;
    }
    public void DesactivateRb()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }

    public int GetAmmoType()
    {
        return (int)type;
    }

    public bool CheckIfLevelDontHaveThisBullet()
    {
        return notInThisLevel;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        switch(type)
        {
            case AmmoType.ToiletRoll:
                if (col.collider.CompareTag("EndPoint"))
                {
                    cameraFollow.ResetCamToDefault(3f);

                    gameManager.PlayerComplete(true, 1.5f);
                    col.gameObject.GetComponent<ToiletAnimation>().RunAnimationWin();

                    SoundManager.Instance.PlaySound(SoundManager.SoundType.Win);
                    SoundManager.Instance.PlaySound(SoundManager.SoundType.Win2);

                    this.gameObject.SetActive(false);

                }
                break;
            case AmmoType.Stone:
                if (col.collider.CompareTag("EndPoint"))
                {
                    gameManager.PlayerComplete(false, 1.5f);
                    col.gameObject.GetComponent<ToiletAnimation>().RunAnimationLose();
                    if (!DATA.GetVibrationState())
                    {
                        Handheld.Vibrate();
                    }
                    SoundManager.Instance.PlaySound(SoundManager.SoundType.Lose2);
                }
                if(col.collider.CompareTag("Glass"))
                {
                    col.gameObject.GetComponent<GlassShattered>().EnablePieces();
                    spriteRenderer.DOFade(0f, 1f);
                    cir2D.enabled = false;
                    StartCoroutine(TurnOffThisGameObject());
                    this.transform.GetChild(0).gameObject.SetActive(false);

                    SoundManager.Instance.PlaySound(SoundManager.SoundType.BreakingGlass);
                }
                break;
        }

        if (rb.velocity.magnitude < 2f) return;
        switch (type)
        {
            case AmmoType.ToiletRoll:
                if (col.collider.CompareTag("Ground") || col.collider.CompareTag("Glass"))
                {
                    SoundManager.Instance.PlaySound(SoundManager.SoundType.ToiletPaperBounce);
                }
                break;
            case AmmoType.Stone:
                if (col.collider.CompareTag("Ground"))
                {
                    SoundManager.Instance.PlaySound(SoundManager.SoundType.RockBounce);
                }
                break;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Zoom") && !hasZoom)
        {
            switch (type)
            {
                case AmmoType.ToiletRoll:
                    cameraFollow.toiletRoll = collision.transform;
                    hasZoom = true;
                    cameraFollow.SlowMoAndZoomCam();
                    SoundManager.Instance.PlaySound(SoundManager.SoundType.SlowMotion);
                    StartCoroutine(ResetTimeScaleAndCamZoom(0.2f));
                    break;
            }
        }
        if (collision.CompareTag("Star"))
        {
            starSystem.Gain1Star();
            switch (type)
            {
                case AmmoType.ToiletRoll:
                spriteRenderer.sprite = toiletPaper_Faces[2];
                break;
            }
            collision.enabled = false;
            collision.transform.GetChild(0).gameObject.SetActive(false);
            collision.transform.GetChild(1).gameObject.SetActive(true);
            SoundManager.Instance.PlaySound(SoundManager.SoundType.ClaimStar);
        }

        
    }

    IEnumerator ResetTimeScaleAndCamZoom(float time)
    {
        yield return new WaitForSeconds(time);
        cameraFollow.ResetCamToDefault(3f);
        spriteRenderer.sprite = toiletPaper_Faces[1];
    }

    IEnumerator TurnOffThisGameObject()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!gameManager.endGame) return;
        DesactivateRb();
    }
}
