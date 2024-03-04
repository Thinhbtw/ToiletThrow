using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class HandFollow : MonoBehaviour
{
    [SerializeField] GameObject stickMan_Arm;
    [SerializeField] GameObject idleAnimation, winAnimation, loseAnimation, runOutAnimation;
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer imgAmmoInHand;
    [SerializeField] List<SpriteRenderer> allBodyPart;
    public bool checkIfPlayerContinueHolding;
    bool hasEnded;
    [Header("-----Change Skin-----")]
    [Space(20f)]

    [SerializeField] SpriteLibrary idleThrow_BodyPart;
    [SerializeField] List<SpriteResolver> idleThrow_Resolver;

    [Space(20f)]
    [SerializeField] SpriteLibrary idleRunOut_BodyPart;
    [SerializeField] List<SpriteResolver> idleRunOut_Resolver;

    [Space(20f)]
    [SerializeField] SpriteLibrary startThrow_BodyPart;
    [SerializeField] List<SpriteResolver> startThrow_Resolver;

    [Space(20f)]
    [SerializeField] SpriteLibrary win_BodyPart;
    [SerializeField] List<SpriteResolver> win_Resolver;

    [Space(20f)]
    [SerializeField] SpriteLibrary lose_BodyPart;
    [SerializeField] List<SpriteResolver> lose_Resolver;


    private SpriteLibraryAsset idleThrow_libraryAsset => idleThrow_BodyPart.spriteLibraryAsset;
    private SpriteLibraryAsset idleRunOut_libraryAsset => idleRunOut_BodyPart.spriteLibraryAsset;
    private SpriteLibraryAsset startThrow_libraryAsset => startThrow_BodyPart.spriteLibraryAsset;
    private SpriteLibraryAsset win_libraryAsset => win_BodyPart.spriteLibraryAsset;
    private SpriteLibraryAsset lose_libraryAsset => lose_BodyPart.spriteLibraryAsset;


    private void Start()
    {
        GameManager.Instance.SetHandFollow(this);
        BeginThrowing(true, 0);
        hasEnded = false;
        anim.enabled = false;

        GameManager.Instance.ChangePlayerSkinWhenSpawnLevel();

        winAnimation.SetActive(false);
        loseAnimation.SetActive(false);
        runOutAnimation.SetActive(false);
    }

    public void BeginThrowing(bool false_IfTurnOffAnimation, byte colorAlpha)
    {
        foreach (SpriteRenderer x in allBodyPart)
        {
            x.color = new Color32(255, 255, 255, colorAlpha);
        }
        idleAnimation.gameObject.SetActive(false_IfTurnOffAnimation);
    }

    public void RemoveAmmoFromSpriteAfterShoot(SpriteRenderer sprite)
    {
        allBodyPart.Remove(sprite);
        PlayAnimationThrow();
    }

    public void SetWhichAmmoInHand(Sprite sprite)
    {
        imgAmmoInHand.sprite = sprite;
        if(sprite == null) //het dan.
        {
            StartCoroutine(RunIdleAnimationOutOfAmmo());
        }
        else
        {
            StartCoroutine(RunIdleAnimationStillHaveAmmo());
        }
    }

    public void SetAmmoScaleInHand(Vector2 scale)
    {
        imgAmmoInHand.gameObject.transform.localScale = scale;
    }

    public void ChangeArmDirection(Vector3 startPoint, Vector3 endPoint)
    {
        float rotateZ;
        if (endPoint.x > 0)
        {
            rotateZ = Mathf.Atan2(endPoint.y, endPoint.x) * Mathf.Rad2Deg;
            stickMan_Arm.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Clamp(rotateZ, -40f, 40f));
            this.transform.rotation = Quaternion.Euler(0f, 0f, 77.895f);
        }
        else
        {
            rotateZ = Mathf.Atan2(endPoint.x, endPoint.y) * Mathf.Rad2Deg;
            stickMan_Arm.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Clamp(rotateZ + 90, -40f, 40f));
            this.transform.rotation = Quaternion.Euler(0f, 180f, 77.895f);
        }

    }

    public void PlayAnimationThrow()
    {
        anim.enabled = true;
        anim.Play("Throwing", -1, 0f);
    }

    IEnumerator RunIdleAnimationStillHaveAmmo()
    {
        yield return new WaitForSeconds(0.5f);
        if (hasEnded) yield break;
        anim.enabled = false;
        BeginThrowing(true, 0);

    }


    IEnumerator RunIdleAnimationOutOfAmmo()
    {
        yield return new WaitForSeconds(0.5f);
        if (hasEnded) yield break;
        anim.enabled = false;
        BeginThrowing(false, 0);
        runOutAnimation.SetActive(true);

    }

    public void PlayWinAnimation()
    {
        hasEnded = true;
        BeginThrowing(false, 0);
        runOutAnimation.SetActive(false);
        winAnimation.SetActive(true);
    }

    public void PlayLoseAnimation()
    {
        hasEnded = true;
        BeginThrowing(false, 0);
        runOutAnimation.SetActive(false);
        loseAnimation.SetActive(true);
    }

    public void ChangePlayerSkin()
    {
        for(int i = 0; i< idleThrow_Resolver.Count; i++)
        {
            string[] labels = idleThrow_libraryAsset.GetCategoryLabelNames(idleThrow_Resolver[i].GetCategory()).ToArray();
            int index = DATA.GetSelectedSkinIndex();
            string label = labels[index];

            idleThrow_Resolver[i].SetCategoryAndLabel(idleThrow_Resolver[i].GetCategory(), label);
        }

        for (int i = 0; i < startThrow_Resolver.Count; i++)
        {
            string[] labels = startThrow_libraryAsset.GetCategoryLabelNames(startThrow_Resolver[i].GetCategory()).ToArray();
            int index = DATA.GetSelectedSkinIndex();
            string label = labels[index];

            startThrow_Resolver[i].SetCategoryAndLabel(startThrow_Resolver[i].GetCategory(), label);
        }

        for (int i = 0; i < idleRunOut_Resolver.Count; i++)
        {
            string[] labels = idleRunOut_libraryAsset.GetCategoryLabelNames(idleRunOut_Resolver[i].GetCategory()).ToArray();
            int index = DATA.GetSelectedSkinIndex();
            string label = labels[index];

            idleRunOut_Resolver[i].SetCategoryAndLabel(idleRunOut_Resolver[i].GetCategory(), label);
        }


        for (int i = 0; i < win_Resolver.Count; i++)
        {
            string[] labels = win_libraryAsset.GetCategoryLabelNames(win_Resolver[i].GetCategory()).ToArray();
            int index = DATA.GetSelectedSkinIndex();
            string label = labels[index];

            win_Resolver[i].SetCategoryAndLabel(win_Resolver[i].GetCategory(), label);
        }

        for (int i = 0; i < lose_Resolver.Count; i++)
        {
            string[] labels = lose_libraryAsset.GetCategoryLabelNames(lose_Resolver[i].GetCategory()).ToArray();
            int index = DATA.GetSelectedSkinIndex();
            string label = labels[index];

            lose_Resolver[i].SetCategoryAndLabel(lose_Resolver[i].GetCategory(), label);
        }
    }

}
