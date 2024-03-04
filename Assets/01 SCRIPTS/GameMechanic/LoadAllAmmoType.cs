using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAllAmmoType : MonoBehaviour
{
    [SerializeField] List<Queue<int>> ammoType = new List<Queue<int>>();
    [SerializeField] List<AmmoPhysics> list_ammo;
    [SerializeField] Queue<int> test = new Queue<int>();
    public GameManager gameManager;
    [SerializeField] ChangeAmmoType changeAmmoType;
    [SerializeField] Transform bulletHasShootHolder;
    [SerializeField] HandFollow handFollow;
    int otherAmmoType;
    int countBulletHasCheck;
    private void Start()
    {
        gameManager = GameManager.Instance;
        changeAmmoType = gameManager.GetChangeAmmoType();

        changeAmmoType.SetLoadAmmoType(this);
        gameManager.SetLoadAmmoType(this);
        otherAmmoType = countBulletHasCheck = 0;

        for (int i = 0; i < Enum.GetNames(typeof(AmmoType)).Length; i++)
        {
            
            for (int j = countBulletHasCheck; j < list_ammo.Count; j++)
            {

                //neu' dung' loai. dan. thi` them vao` queue test
                if (list_ammo[j].GetAmmoType() == i)
                {
                    if (list_ammo[j].CheckIfLevelDontHaveThisBullet()) //neu' dan. duoc tich' la ko co trong man` thi` duyet. tiep vien tiep theo
                    {
                        countBulletHasCheck++;
                        if (countBulletHasCheck == list_ammo.Count) //neu' vien vua` duyet. la` vien cuoi' cung` thi them vao queue vao list
                        {
                            ammoType.Add(test);
                            test = new Queue<int>();
                        }

                        continue;
                    }

                    test.Enqueue(j); //neu' dan. ko duoc tich' la` ko co trong man` thi them vao` queue
                    countBulletHasCheck++;

                    if(countBulletHasCheck == list_ammo.Count) //sau khi them vao` queue va` day la` vien dan. cuoi' trong so gameobject thi` add vao list
                    {
                        ammoType.Add(test);
                        test = new Queue<int>();   
                        break;
                    }
                }
                else //neu' dang duyet. dan. nay` ma chuyen? qua dan. khac' thi them queue vao list
                {
                    ammoType.Add(test);
                    test = new Queue<int>();
                    break;
                }

            }
        }

        gameManager.SetRigidBodyForCurrentBullet(BulletToShoot(0));
        changeAmmoType.ChangeButtonInteractWhenCreateNewMap();
    }

    public void PlayerChosingAmmoType(int AmmoType_Index)
    {
        //het' sach. dan. thi` dung`
        if (ChangeBulletTypeWhenRunOut() == -1)
        {
            handFollow.SetWhichAmmoInHand(null);
            return;
        }

            //neu' ban' 1 vien dan. va` bam' nut' chuyen? sang kieu dan. khac'
        if (otherAmmoType != AmmoType_Index && CountBulletInQueue(otherAmmoType) > 0) 
        {
            list_ammo[ammoType[otherAmmoType].Peek()].gameObject.SetActive(false);
            list_ammo[ammoType[AmmoType_Index].Peek()].gameObject.SetActive(true);
            otherAmmoType = AmmoType_Index;
        }
        else
        {
            list_ammo[ammoType[AmmoType_Index].Peek()].gameObject.SetActive(true);
        }
        
        gameManager.SetRigidBodyForCurrentBullet(list_ammo[ammoType[AmmoType_Index].Peek()]);
        handFollow.SetWhichAmmoInHand(list_ammo[ammoType[AmmoType_Index].Peek()].spriteRenderer.sprite);

        if(AmmoType_Index == 0) //set lai scale gameobject dan. o? tay cam` luc' Idle do kich thuoc 2 vien dan khac nhau
        {
            handFollow.SetAmmoScaleInHand(new Vector2(1, 1));
        }
        else
        {
            handFollow.SetAmmoScaleInHand(new Vector2(.5f, .5f));
        }
    }

    public AmmoPhysics BulletToShoot(int AmmoType_Index)
    {
        return list_ammo[ammoType[AmmoType_Index].Peek()];
    }

    public void Fired(int AmmoType_Index)
    {
        if (ammoType[AmmoType_Index].Count < 1) //cha hieu sao tren firebase queue van null dc nen them vao` day de? xem
        {
            PlayerChosingAmmoType(ChangeBulletTypeWhenRunOut());
            return;
        }

        //neu' ban het' dan. thi` tu. dong. doi?
        handFollow.RemoveAmmoFromSpriteAfterShoot(list_ammo[ammoType[AmmoType_Index].Peek()].spriteRenderer);
        //ban' vien nao` xong thi` deque ra khoi? queue
        list_ammo[ammoType[AmmoType_Index].Peek()].transform.SetParent(bulletHasShootHolder);
        ammoType[AmmoType_Index].Dequeue();

        if (ammoType[AmmoType_Index].Count < 1)
        {
            PlayerChosingAmmoType(ChangeBulletTypeWhenRunOut());
            return;
        }
        
        //chua het' dan.
        PlayerChosingAmmoType(AmmoType_Index);

    }

    public int CountBulletInQueue(int AmmoType_Index)
    {
        if (AmmoType_Index >= ammoType.Count) //neu' trong man` de? chi? 1 loai. dan. thi` cac' loai. khac ko tinh' den
        {
            return 0;
        }
        return ammoType[AmmoType_Index].Count;
    }

    public int ChangeBulletTypeWhenRunOut()
    {
        for(int i = 0; i < ammoType.Count; i++)
        {
            if(ammoType[i].Count > 0)
            {
                return i;
            }
        }
        return -1;
        
    }

}
