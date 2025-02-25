using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    [SerializeField] private string parameterName;
    [SerializeField] private float parameterValue;

    [SerializeField] public bool isSong1Playing = false;
    [SerializeField] public bool isSong2Playing = false;
    [SerializeField] public bool isSong3Playing = false;
    [SerializeField] public bool isSong4Playing = false;
    [SerializeField] public bool isSong5Playing = false;
    [SerializeField] public bool isSong6Playing = false;

    [SerializeField] private EventInstance musicEventInstance1;
    [SerializeField] private EventInstance musicEventInstance2;
    [SerializeField] private EventInstance musicEventInstance3;
    [SerializeField] private EventInstance musicEventInstance4;
    [SerializeField] private EventInstance musicEventInstance5;
    [SerializeField] private EventInstance musicEventInstance6;


    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "AudioManager";
                    _instance = obj.AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        InitializeAmbience(FMODEvents.instance.Sandstorm);
        AudioManager.Instance.SetAmbienceParameter(parameterName, parameterValue);
        
        
        //SetSongStatus(true, false, false, false, false, false);
        //StopSongs();
        //PlaySongs();
    }

    private void SetSongStatus(bool song1, bool song2, bool song3, bool song4, bool song5, bool song6)
    {
        isSong1Playing = song1;
        isSong2Playing = song2;
        isSong3Playing = song3;
        isSong4Playing = song4;
        isSong5Playing = song5;
        isSong6Playing = song6;
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }


    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }


    #region  Songs

    public void PlaySongs()
    {
        if (isSong1Playing)
        {
            isSong1Playing = true;
            isSong2Playing = false;
            isSong3Playing = false;
            isSong4Playing = false;
            isSong5Playing = false;
            isSong6Playing = false;
            StopSongs();
            // Initialize and start playing Song 1
            musicEventInstance1 = CreateInstance(FMODEvents.instance.Music1);
            musicEventInstance1.start();
        }
        else if (isSong2Playing)
        {
            isSong1Playing = false;
            isSong2Playing = true;
            isSong3Playing = false;
            isSong4Playing = false;
            isSong5Playing = false;
            isSong6Playing = false;
            StopSongs();
            // Initialize and start playing Song 1
            musicEventInstance2 = CreateInstance(FMODEvents.instance.Music2);
            musicEventInstance2.start();
        }
        else if (isSong3Playing)
        {
            isSong1Playing = false;
            isSong2Playing = false;
            isSong3Playing = true;
            isSong4Playing = false;
            isSong5Playing = false;
            isSong6Playing = false;
            StopSongs();
            // Initialize and start playing Song 3
            musicEventInstance3 = CreateInstance(FMODEvents.instance.Music3);
            musicEventInstance3.start();
        }
        else if (isSong4Playing)
        {
            isSong1Playing = false;
            isSong2Playing = false;
            isSong3Playing = false;
            isSong4Playing = true;
            isSong5Playing = false;
            isSong6Playing = false;
            StopSongs();
            // Initialize and start playing Song 4
            musicEventInstance4 = CreateInstance(FMODEvents.instance.Music4);
            musicEventInstance4.start();
        }
        else if (isSong5Playing)
        {
            isSong1Playing = false;
            isSong2Playing = false;
            isSong3Playing = false;
            isSong4Playing = false;
            isSong5Playing = true;
            isSong6Playing = false;
            StopSongs();
            // Initialize and start playing Song 5
            musicEventInstance5 = CreateInstance(FMODEvents.instance.Music5);
            musicEventInstance5.start();
        }

        else if (isSong6Playing)
        {
            isSong1Playing = false;
            isSong2Playing = false;
            isSong3Playing = false;
            isSong4Playing = false;
            isSong5Playing = false;
            isSong6Playing = true;
            StopSongs();
            // Initialize and start playing Song 6
            musicEventInstance6 = CreateInstance(FMODEvents.instance.Music6);
            musicEventInstance6.start();
        }
    }

    public void StopSongs()
    {
        if (!isSong1Playing && musicEventInstance1.isValid())
        {
            musicEventInstance1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        if (!isSong2Playing && musicEventInstance2.isValid())
        {
            musicEventInstance2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        if (!isSong3Playing && musicEventInstance3.isValid())
        {
            musicEventInstance3.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        if (!isSong4Playing && musicEventInstance4.isValid())
        {
            musicEventInstance4.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        if (!isSong5Playing && musicEventInstance5.isValid())
        {
            musicEventInstance5.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        if (!isSong6Playing && musicEventInstance6.isValid())
        {
            musicEventInstance6.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    #endregion

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}