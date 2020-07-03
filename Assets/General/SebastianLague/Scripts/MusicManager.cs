using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;

    string sceneName;

    void Start()
    {
        AudioManager.instance.PlayMusic(menuTheme, 2);
    }

    private void OnLevelWasLoaded(int level)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != sceneName)
        {
            //0.2초 줘서 invoke하는 이유는 levelloaded가 scene이 사라지기 이전에 호출되기 때문에...? 여전히 문제 있음(missing reference)
            Invoke("PlayMusic", 0.5f);
            sceneName = newSceneName;
        }
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;

        if (sceneName == "Menu")
        {
            clipToPlay = menuTheme;
        }
        else if (sceneName == "Game")
        {
            clipToPlay = mainTheme;
        }

        if(clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }
    }

    

}
