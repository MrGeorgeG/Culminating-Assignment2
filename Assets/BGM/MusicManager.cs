using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
 00 - Overworld
 01 - Battle

 */

//  <summary>
//  Responsible for playing music, modifying volume, transitoning music etc.
//  </summary>
public class MusicManager : MonoBehaviour
{
    private MusicManager() { }
    private static MusicManager instance;
    public static MusicManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MusicManager>();
            }
            return instance;
        }
        private set { }
    }

    private static bool keepFadingIn;
    private static bool keepFadingout;

    [SerializeField]
    AudioSource musicSource;

    [SerializeField]
    AudioClip[] trackList;

    [SerializeField]
    AudioMixer musicMixer;

    [SerializeField]
    float volumeMin_dB = -80.0f;
    [SerializeField]
    float volumeMax_dB = 0.0f;

    public enum Track
    {
        Overworld,
        Battle
    }

    // Start is called before the first frame update
    void Start()
    {
        //GameController encounterManager = GameState.player.GetComponent<GameController>();
        //encounterManager.onEnterEncounter.AddListener(OnEncounterEnterHandler);
        //encounterManager.onExitEncounter.AddListener(OnEncounterExitHandler);

        //When we start... look for copies of MusicManager...
        //Then politely ask them to leave this dimension
        MusicManager[] musicManagers = FindObjectsOfType<MusicManager>();
        foreach (MusicManager mgr in musicManagers)
        {
            if (mgr != Instance)
            {
                Destroy(mgr.gameObject);

            }
        }

        DontDestroyOnLoad(transform.root);
    }

    public void OnEncounterEnterHandler()
    {
        PlayTrack(Track.Battle);
    }

    public void OnEncounterExitHandler()
    {
        FadeInTrackOverSeconds(Track.Overworld, 5.0f);
    }

    public void PlayTrack(MusicManager.Track trackID, float fadeDuration = 1)
    {
        musicSource.clip = trackList[(int)trackID];
        musicSource.Play();

        StartCoroutine(FadeInTrackOverSecondsCoroutione(fadeDuration));
    }

    public void FadeInTrackOverSeconds(MusicManager.Track trackID, float duration)
    {
        musicSource.volume = 0.0f;

        PlayTrack(trackID);
        StartCoroutine(FadeInTrackOverSecondsCoroutione(duration));
    }

    IEnumerator FadeInTrackOverSecondsCoroutione(float duration)
    {
        float timer = 0.0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float normalizedTime = timer / duration;

            //musicSource.volume = Mathf.SmoothStep(0, 1, normalizedTime);
            //musicSource.volume = Mathf.Lerp(1, 0, normalizedTime);
            musicSource.volume = Mathf.Lerp(0, 1, normalizedTime);

            //fade volum
            //yield return new WaitForEndOfFrame();
            yield return null;

            //yield return new WaitForSeconds(waitTime);
        }
    }

    public void SetMusicVolume(float volumeNormalized)
    {
        musicMixer.SetFloat("Music Volume", Mathf.Lerp(volumeMin_dB, volumeMax_dB, volumeNormalized));
    }
}
