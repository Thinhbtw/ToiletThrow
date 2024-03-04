using System;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public enum SoundType
    {
        Background,
        Win,
        Win2,
        Lose2,
        ToiletPaperBounce,
        BreakingGlass,
        Dragging,
        EndDrag,
        Click,
        RockBounce,
        ClaimStar,
        SlowMotion,
        ShopPurchasedSuccess,
        ShopPurchasedFailed,
        SkinEquip,
        Star1,
        Star2,
        UIEndWin,
    }

    [System.Serializable]
    public class Sound
    {
        public SoundType soundType;
        public AudioClip clip;
        public bool loop;
    }

    [SerializeField] ButtonManager btnManager;
    [SerializeField] Sound[] sounds;
    [SerializeField] GameObject soundPrefab;
    // Start is called before the first frame update
    private void Start()
    {
        foreach (var s in sounds)
        {
            var sound = Instantiate(soundPrefab, this.transform);
            sound.name = s.soundType.ToString();
            sound.GetComponent<AudioSource>().clip = s.clip;
            sound.GetComponent<AudioSource>().loop = s.loop;
            //sound.GetComponent<AudioSource>().volume = PlayerData.GetVolumeValue();
            sound.SetActive(false);
        }

        if (DATA.GetSoundState())
        {
            PlaySound(SoundType.Background);
        }
    }

    public void ChangingAudioVolume(float volume)
    {
        if (volume <= 0)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                transform.GetChild(i).gameObject.GetComponent<AudioSource>().volume = volume;
            }
        }
        else
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.GetComponent<AudioSource>().volume = volume;
            }

        }
    }

    public void PlaySound(SoundType soundType)
    {
        if (!DATA.GetSoundState()) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == soundType.ToString() && !transform.GetChild(i).gameObject.activeInHierarchy)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void StopAllSound()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void StopSpecificSound(SoundType soundType)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == soundType.ToString())
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void StopAllSoundExceptBG()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("BG") || transform.GetChild(i).name.Contains("Click"))
            {
                continue;
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void StopStarSound()
    {
        StopSpecificSound(SoundType.Star1);
        StopSpecificSound(SoundType.Star2);
        StopSpecificSound(SoundType.UIEndWin);
    }
}