using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    float masterVolumePercent = 0.2f;
    float sfxVolumePercent = 1f;
    float musicVolumePercent = 1f;

    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    //make singleton
    public static AudioManager instance;

    Transform audioListener;
    Transform playerT;

    private void Awake()
    {
        instance = this;

        musicSources = new AudioSource[2];
        for(int i=0;i<2;i++)
        {
            GameObject newMusicSource = new GameObject("Music source " + (i + 1).ToString());
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = this.transform;
        }

        audioListener = FindObjectOfType<AudioListener>().transform;
        playerT = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        if(playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if(clip != null)
        {
            //중간에 볼륨을 바꿀 수 없음. 짧은 음향 효과 재생에 적합
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
        
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
