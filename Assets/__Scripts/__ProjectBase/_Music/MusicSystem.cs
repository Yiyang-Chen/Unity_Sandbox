using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

//Use sound mixer to manage sounds.
//I only know a few about sound mixer, so the codes are not perfect and still need more chanegs.
public class MusicSystem : System<MusicSystem>
{
    private AudioSource bkMusic=null;
    private float bkValue = 1;

    private GameObject soundObj = null;
    private List<AudioSource> soundList = new List<AudioSource>();
    private float sValue = 1;
    // Please use make changes on bkValue and sValue though mixer parameters.
    // You can find example in SystemSettingsPanel.
    // I didn't have time to add this feature into this manager.
    /*  
        audioMixer.GetFloat("MasterVolume", out currVolum);
        masterVolumSlider.value = currVolum;
        audioMixer.GetFloat("BKMusicVloume", out currVolum);
        BKMusicSlider.value = currVolum;
        audioMixer.GetFloat("SFXVolume", out currVolum);
        SFXVloumeSlider.value = currVolum;
    */


    //Some references to the sound path for easy accesses and changes.
    public string mouseEnterSound = "_Using/296741-Browse-sci-fi-bleep-04";
    public string mouseExitSound = "_Using/audionfx_High_Tech_Random_17";
    public string mouseLeftRightSound = "_Using/DM-CGS-01";
    public string mouseStartFishingSound = "_Using/audionfx_High_Tech_Random_95";
    public string confirmSound = "_Using/audionfx_High_Tech_Random_93";
    public string tagSound = "_Using/smooth_button_click_25";
    public string switchRoomSound = "_Using/audionfx_High_Tech_Random_93";
    public string exitSound = "_Using/DM-CGS-03";
    public string switchMapSuccessSound = "_Using/smooth_button_click_15";
    public string switchMapFailSound = "_Using/audionfx_High_Tech_Random_26";
    public string applySound = "_Using/smooth_button_click_15";
    public string failToStartSound = "_Using/DM-CGS-16";
    public string successHookingSound = "_Using/DM-CGS-12";

    //Some references to the soundmixer.
    AudioMixer _mainMixer;

    protected override void OnInit()
    {
        _mainMixer = MinimalEnvironment.Instance.GetSystem<ResourceSystem>()?.Load<AudioMixer>("_Music/Mixers/MainMixer");
        MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.AddUpdateListener(Update);
    }

    protected override void OnShutdown()
    {
        if (bkMusic != null)
        {
            GameObject.Destroy(bkMusic.gameObject);
            bkMusic = null;
        }
        if (soundObj != null)
        {
            GameObject.Destroy(soundObj);
            soundObj = null;
        }
        soundList.Clear();
    }

    /// <summary>
    /// Check if the sound effects have been played. If so, delete it.
    /// </summary>
    private void Update()
    {
        for(int i = soundList.Count-1; i >= 0; --i)
        {
            if (!soundList[i].isPlaying)
            {
                //??????????
                //You can use object pool here, but I don't face a condition that is necessary.
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// Play background Music.
    /// </summary>
    /// <param name="name">Path of the music. Please use reference of music pathes in this manager, so that it is easy for us to change music path.</param>
    public void PlayBkMusic(string name)
    {
        if(bkMusic == null)
        {
            GameObject obj = new();
            obj.name = "BKMusic";
            MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.DontDestoryOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }

        MinimalEnvironment.Instance.GetSystem<ResourceSystem>()?.LoadAsyn<AudioClip>("_Music/BackGround/" + name, (audioClip) =>
        {
            bkMusic.clip = audioClip;
            bkMusic.volume = bkValue;
            bkMusic.loop = true;
            bkMusic.outputAudioMixerGroup = _mainMixer.FindMatchingGroups("BKMusic")[0];
            bkMusic.Play();
        });
    }

    /// <summary>
    /// Allow two background music to have cross fading effects when switch from current music to the next one.
    /// </summary>
    /// <param name="name">The path of the next music. Please use reference of music pathes in this manager, so that it is easy for us to change music path.</param>
    public void CrossFading(string name)
    {
        if (bkMusic == null)
        {
            PlayBkMusic(name);
            bkMusic.volume = 0;
            MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.StartCoroutine(StartFade(bkMusic,5f, bkValue,false));
        }
        else
        {
            AudioSource oldBk = bkMusic;
            AudioSource newBk = bkMusic.gameObject.AddComponent<AudioSource>();

            MinimalEnvironment.Instance.GetSystem<ResourceSystem>()?.LoadAsyn<AudioClip>("_Music/BackGround/" + name, (AudioClip) =>
            {
                newBk.clip = AudioClip;
                newBk.loop = true;
                newBk.outputAudioMixerGroup = _mainMixer.FindMatchingGroups("BKMusic")[0];
                newBk.volume = 0;
                newBk.Play();
            });

            newBk.volume = 0;
            MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.StartCoroutine(StartFade(oldBk, 2f, 0,true));
            MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.StartCoroutine(StartFade(newBk, 2f, bkValue,false));

            bkMusic = newBk;
        }
    }

    /// <summary>
    /// Pause background music.
    /// </summary>
    public void PauseBkMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    /// <summary>
    /// Stop background music.
    /// </summary>
    public void StopBkMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// Play a sound effect.
    /// </summary>
    public void PlaySound(string name, bool isloop, UnityAction<AudioSource> callBack = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";
            MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.DontDestoryOnLoad(soundObj);

        }

        MinimalEnvironment.Instance.GetSystem<ResourceSystem>()?.LoadAsyn<AudioClip>("_Music/SoundEffect/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            
            source.clip = clip;
            source.volume = sValue;
            source.loop = isloop;
            source.outputAudioMixerGroup = _mainMixer.FindMatchingGroups("SFX")[0];
            source.Play();
            if (callBack != null) 
                callBack(source);
            soundList.Add(source); 
        });     
    }

    /// <summary>
    /// Play sound effect randomly from a list.
    /// </summary>
    /// <param name="names">A list of sound effect path.</param>
    public void RandomPlaySound(string[] names, bool[] isloops, UnityAction<AudioSource>[] callBacks = null)
    {
        int randomIndex = Random.Range(0, names.Length);
        UnityAction<AudioSource> callBack = null;
        if (callBacks != null) callBack = callBacks[randomIndex];
        PlaySound(names[randomIndex], isloops[randomIndex], callBack);
    }

    /// <summary>
    /// Stop sound effects.
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
    /// <summary>
    /// Music Fade IEnumerator.
    /// </summary>
    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume, bool needDestory)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if(needDestory) MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.Destory(audioSource);
        yield break;
    }
}
