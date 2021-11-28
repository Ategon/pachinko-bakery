using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UIElements;
//using UnityEditor.UIElements;
using System;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    [SerializeField] private Debugging _debug;
    [SerializeField] private AudioTrack[] _tracks;

    private float _masterVolume = 5;
    private bool _masterMute;

    private Hashtable _audioTable;
    private Hashtable _jobTable;

    public static AudioManager Instance { get { return _instance; } private set {; } }

    [System.Serializable]
    public class AudioObject
    {
        public string name;
        public AudioClip intro;
        public AudioClip clip;
    }

    [System.Serializable]
    public class AudioTrack
    {
        public AudioTrackInfo info = new AudioTrackInfo();

        public AudioSource source;
        public AudioSource secondSource;
        public bool secondSourceActive;

        public AudioObject[] audio;

        public double nextLoopTime;

        public IEnumerator currentVolumeCoroutineSource;
        public IEnumerator currentVolumeCoroutineSecondSource;

        public IEnumerator currentPitchCoroutineSource;
        public IEnumerator currentPitchCoroutineSecondSource;

        public AudioObject currentAudio;
    }


    #region Public Functions

    /// <summary>
    /// Plays a sound on an audio track.
    /// </summary>
    /// <param name="name">The name of the sound that will be played.</param>
    /// <param name="fade">Whether you want the audio to fade. (Only on advanced tracks)</param>
    /// <param name="keepTime">Whether the current track position should carry on to the new sound. (Only on advanced tracks)</param>
    /// <param name="bypassIntro">Whether the sound should bypass the intro section</param>
    public void Play(string name, bool fade = false, bool keepTime = false, bool bypassIntro = false)
    {
        AudioTrack track = (AudioTrack)_audioTable[name];

        if (track == null)
        {
            LogWarning($"Cannot play audio [{name}] that does not exist in the audio table.");
        }
        else
        {

            float volumeMultiplier = (float)Math.Pow(((float)track.info.volume / 10) * ((float)_masterVolume / 10), 0.5f);

            if ((track.info.mute || _masterMute)) volumeMultiplier = 0;

            AudioObject audio = GetAudioFromAudioTrack(name, track);
            if (audio == null)
            {
                LogWarning($"Cannot play audio  [{name}] that does not exist on track [{track.info.trackName}].");
            }
            else
            {

                track.currentAudio = audio;

                if (track.info.advanced)
                {
                    if (track.secondSourceActive)
                    {
                        if (audio.intro != null && !bypassIntro && !(keepTime && track.secondSource.time != 0))
                        {
                            track.source.clip = audio.intro;
                            track.nextLoopTime = audio.intro.length;
                        }
                        else
                        {
                            track.source.clip = audio.clip;
                            track.nextLoopTime = audio.clip.length;
                        }



                        track.source.Play();
                        if (keepTime && track.currentAudio.clip.length > track.secondSource.time) track.source.time = track.secondSource.time;
                        track.secondSourceActive = false;



                        if (!fade)
                        {
                            track.source.volume = volumeMultiplier;
                            track.secondSource.Stop();
                            track.secondSource.clip = null;
                        }
                        else
                        {
                            if (track.currentVolumeCoroutineSource != null) StopCoroutine(track.currentVolumeCoroutineSource);
                            track.currentVolumeCoroutineSource = FadeAudio(track.source, 0f, true, 1f, track);
                            StartCoroutine(track.currentVolumeCoroutineSource);

                            if (track.currentVolumeCoroutineSecondSource != null) StopCoroutine(track.currentVolumeCoroutineSecondSource);
                            track.currentVolumeCoroutineSecondSource = FadeAudio(track.secondSource, track.secondSource.volume, false, 1f, track);
                            StartCoroutine(track.currentVolumeCoroutineSecondSource);
                        }

                    }
                    else
                    {
                        if (audio.intro != null && !bypassIntro && !(keepTime && track.source.time != 0))
                        {
                            track.secondSource.clip = audio.intro;
                            track.nextLoopTime = audio.intro.length;
                        }
                        else
                        {
                            track.secondSource.clip = audio.clip;
                            track.nextLoopTime = audio.clip.length;
                        }


                        track.secondSource.Play();
                        if (keepTime && track.currentAudio.clip.length > track.source.time) track.secondSource.time = track.source.time;
                        track.secondSourceActive = true;

                        if (!fade)
                        {
                            track.secondSource.volume = volumeMultiplier;
                            track.source.Stop();
                            track.source.clip = null;
                        }
                        else
                        {
                            if (track.currentVolumeCoroutineSource != null) StopCoroutine(track.currentVolumeCoroutineSource);
                            track.currentVolumeCoroutineSource = FadeAudio(track.source, track.source.volume, false, 1f, track);
                            StartCoroutine(track.currentVolumeCoroutineSource);

                            if (track.currentVolumeCoroutineSecondSource != null) StopCoroutine(track.currentVolumeCoroutineSecondSource);
                            track.currentVolumeCoroutineSecondSource = FadeAudio(track.secondSource, 0f, true, 1f, track);
                            StartCoroutine(track.currentVolumeCoroutineSecondSource);

                        }
                    }
                }
                else
                {
                    track.source.volume = volumeMultiplier;
                    if (audio.intro != null)
                    {
                        track.source.clip = audio.intro;
                        track.nextLoopTime = audio.intro.length;
                    }
                    else
                    {
                        track.source.clip = audio.clip;
                        track.nextLoopTime = audio.clip.length;
                    }
                    track.source.Play();
                }

            }
        }
    }

    /// <summary>
    /// Stops an audio track.
    /// </summary>
    /// <param name="name">The name of the track that you are stopping.</param>
    /// <param name="fade">Whether you want the audio to fade.</param>
    public void Stop(string name = null, bool fade = false)
    {
        Pitch(0f, name, fade);
    }

    /// <summary>
    /// Plays an audio track.
    /// </summary>
    /// <param name="amount">The amount you want to set the volume to(-1f is to get instead of set).</param>
    /// <param name="name">The name of the track that you are getting or changing the volume of(null gets the master volume).</param>
    /// <param name="fade">Whether you want the audio to fade.</param>
    /// <returns>A float corresponding to the volume</returns>
    public float Volume(float amount = -1f, string name = null, bool fade = false)
    {
        if (name == null)
        {
            if (amount == -1f)
            {
                return _masterVolume;
            }
            else
            {
                if (fade)
                {
                    _masterVolume = Math.Min(Math.Max(amount, 0f), 10f);
                    foreach (AudioTrack track in _tracks)
                    {
                        if (track.info.advanced)
                        {
                            if (track.currentVolumeCoroutineSource != null) StopCoroutine(track.currentVolumeCoroutineSource);
                            if (track.currentVolumeCoroutineSecondSource != null) StopCoroutine(track.currentVolumeCoroutineSecondSource);
                            track.currentVolumeCoroutineSource = null;
                            track.currentVolumeCoroutineSecondSource = null;

                            if (track.secondSourceActive)
                            {
                                track.currentVolumeCoroutineSecondSource = FadeAudio(track.secondSource, track.secondSource.volume, true, 1f, track);
                                track.currentVolumeCoroutineSource = FadeAudio(track.source, track.source.volume, false, 1f, track);
                                StartCoroutine(track.currentVolumeCoroutineSecondSource);
                                StartCoroutine(track.currentVolumeCoroutineSource);
                            }
                            else
                            {
                                track.currentVolumeCoroutineSource = FadeAudio(track.source, track.source.volume, true, 1f, track);
                                track.currentVolumeCoroutineSecondSource = FadeAudio(track.secondSource, track.secondSource.volume, false, 1f, track);
                                StartCoroutine(track.currentVolumeCoroutineSource);
                                StartCoroutine(track.currentVolumeCoroutineSecondSource);
                            }
                        }
                        else
                        {
                            if (track.currentVolumeCoroutineSource != null) StopCoroutine(track.currentVolumeCoroutineSource);
                            track.currentVolumeCoroutineSource = FadeAudio(track.source, track.source.volume, true, 1f, track);
                            StartCoroutine(track.currentVolumeCoroutineSource);
                        }


                    }
                    return _masterVolume;
                }
                else
                {
                    _masterVolume = Math.Min(Math.Max(amount, 0f), 10f);
                    ResetVolumeCoroutines();
                    return _masterVolume;
                }
            }
        }
        else
        {
            AudioTrack track = Array.Find(_tracks, t => t.info.trackName == name);
            if (track != null)
            {
                if (amount == -1f)
                {
                    return track.info.volume;
                }
                else
                {
                    if (fade)
                    {
                        _tracks[Array.IndexOf(_tracks, track)].info.volume = Math.Min(Math.Max(amount, 0f), 10f);

                        if (track.info.advanced)
                        {
                            if (track.currentVolumeCoroutineSource != null) StopCoroutine(track.currentVolumeCoroutineSource);
                            if (track.currentVolumeCoroutineSecondSource != null) StopCoroutine(track.currentVolumeCoroutineSecondSource);
                            track.currentVolumeCoroutineSource = null;
                            track.currentVolumeCoroutineSecondSource = null;

                            if (track.secondSourceActive)
                            {
                                track.currentVolumeCoroutineSecondSource = FadeAudio(track.secondSource, track.secondSource.volume, true, 1f, track);
                                track.currentVolumeCoroutineSource = FadeAudio(track.source, track.source.volume, false, 1f, track);
                                StartCoroutine(track.currentVolumeCoroutineSecondSource);
                                StartCoroutine(track.currentVolumeCoroutineSource);
                            }
                            else
                            {
                                track.currentVolumeCoroutineSource = FadeAudio(track.source, track.source.volume, true, 1f, track);
                                track.currentVolumeCoroutineSecondSource = FadeAudio(track.secondSource, track.secondSource.volume, false, 1f, track);
                                StartCoroutine(track.currentVolumeCoroutineSource);
                                StartCoroutine(track.currentVolumeCoroutineSecondSource);
                            }
                        }
                        else
                        {
                            if (track.currentVolumeCoroutineSource != null) StopCoroutine(track.currentVolumeCoroutineSource);
                            track.currentVolumeCoroutineSource = FadeAudio(track.source, track.source.volume, true, 1f, track);
                            StartCoroutine(track.currentVolumeCoroutineSource);
                        }

                        return Math.Min(Math.Max(amount, 0f), 10f);
                    }
                    else
                    {
                        _tracks[Array.IndexOf(_tracks, track)].info.volume = Math.Min(Math.Max(amount, 0f), 10f);
                        ResetVolumeCoroutines();
                        return Math.Min(Math.Max(amount, 0f), 10f);
                    }
                }
            }
            else
            {
                LogWarning($"You are trying to get the volume of track [{name}] that does not exist");
            }
        }
        return -1f;
    }

    /// <summary>
    /// Changes the pitch of an audio track
    /// </summary>
    /// <param name="amount">The amount you want to set the pitch to.</param>
    /// <param name="name">The name of the track that you are changing.</param>
    /// <param name="fade">Whether you want the audio to fade.</param>
    public void Pitch(float amount = -1f, string name = null, bool fade = false)
    {
        if (name == null)
        {

        }
        else
        {
            AudioTrack track = Array.Find(_tracks, t => t.info.trackName == name);
            if (track != null)
            {
                if (fade)
                {
                    if (track.info.advanced)
                    {
                        if (track.currentPitchCoroutineSource != null) StopCoroutine(track.currentPitchCoroutineSource);
                        if (track.currentPitchCoroutineSecondSource != null) StopCoroutine(track.currentPitchCoroutineSecondSource);
                        track.currentPitchCoroutineSource = null;
                        track.currentPitchCoroutineSecondSource = null;

                        track.currentPitchCoroutineSecondSource = FadePitch(track.secondSource, amount, 1f, track);
                        track.currentPitchCoroutineSource = FadePitch(track.source, amount, 1f, track);
                        StartCoroutine(track.currentPitchCoroutineSecondSource);
                        StartCoroutine(track.currentPitchCoroutineSource);
                    }
                    else
                    {
                        if (track.currentPitchCoroutineSource != null) StopCoroutine(track.currentPitchCoroutineSource);
                        track.currentPitchCoroutineSource = FadePitch(track.source, amount, 1f, track);
                        StartCoroutine(track.currentPitchCoroutineSource);
                    }
                }
                else
                {
                    if (track.currentPitchCoroutineSource != null) StopCoroutine(track.currentPitchCoroutineSource);
                    if (track.currentPitchCoroutineSecondSource != null) StopCoroutine(track.currentPitchCoroutineSecondSource);
                    track.currentPitchCoroutineSource = null;
                    track.currentPitchCoroutineSecondSource = null;

                    if (track.info.advanced) track.currentPitchCoroutineSecondSource = FadePitch(track.secondSource, amount, 0f, track);
                    track.currentPitchCoroutineSource = FadePitch(track.source, amount, 0f, track);
                    StartCoroutine(track.currentPitchCoroutineSource);
                    if (track.info.advanced) StartCoroutine(track.currentPitchCoroutineSecondSource);
                }
            }
            else
            {
                LogWarning($"You are trying to set the pitch of track [{name}] that does not exist");
            }
        }
    }

    /// <summary>
    /// Resume an audio track.
    /// </summary>
    /// <param name="name">The name of the track that you are resuming.</param>
    /// <param name="fade">Whether you want the audio to fade.</param>
    public void Resume(string name = null, bool fade = false)
    {
        Pitch(1f, name, fade);
    }

    /// <summary>
    /// Gives a list containing the info of all audio tracks.
    /// </summary>
    /// <returns>A list of AudioTrackInfo objects</returns>
    public List<AudioTrackInfo> Tracks()
    {
        List<AudioTrackInfo> trackInfos = new List<AudioTrackInfo>();
        for (int i = 0; i < _tracks.Length; i++)
        {
            trackInfos.Add(_tracks[i].info);
        }
        return trackInfos;
    }

    #endregion

    #region Private Functions


    private void Configure()
    {
        _instance = this;
        _audioTable = new Hashtable();
        _jobTable = new Hashtable();
        GenerateAudioTable();
        DontDestroyOnLoad(this.gameObject);

        foreach (AudioTrack track in _tracks)
        {
            GameObject childObject = new GameObject(track.info.trackName);
            childObject.transform.parent = this.transform;
            track.source = childObject.AddComponent<AudioSource>();

            track.nextLoopTime = 10;

            if (track.info.advanced)
            {
                GameObject childObject2 = new GameObject($"{track.info.trackName} 2");
                childObject2.transform.parent = this.transform;
                track.secondSource = childObject2.AddComponent<AudioSource>();
            }
        }
    }

    private void Dispose()
    {
        /*foreach (DictionaryEntry entry in _jobTable) {
            IEnumerator job = (IEnumerator)entry.Value;
            StopCoroutine(job);
        }*/
    }

    private void GenerateAudioTable()
    {
        foreach (AudioTrack track in _tracks)
        {
            track.info.volume = 5; //TEMP
            foreach (AudioObject obj in track.audio)
            {
                //do not duplicate keys, check if something already exists before adding it into here
                if (_audioTable.ContainsKey(obj.name))
                {
                    LogWarning($"You are trying to register audio [{obj.name}] on {track.info.trackName} that has already been registered somewhere");
                }
                else
                {
                    _audioTable.Add(obj.name, track);
                    Log($"Registering audio [{obj.name}] on {track.info.trackName}");
                }
            }
        }
    }

    private void ResetVolumeCoroutines()
    {
        foreach (AudioTrack track in _tracks)
        {
            if (track.currentVolumeCoroutineSource != null) StopCoroutine(track.currentVolumeCoroutineSource);
            if (track.currentVolumeCoroutineSecondSource != null) StopCoroutine(track.currentVolumeCoroutineSecondSource);
            track.currentVolumeCoroutineSource = null;
            track.currentVolumeCoroutineSecondSource = null;

            if (track.info.mute || _masterMute)
            {
                if (track.info.advanced)
                {
                    track.source.volume = 0f;
                    track.secondSource.volume = 0f;
                }
                else
                {
                    track.source.volume = 0f;
                }
            }
            else
            {
                if (track.info.advanced)
                {
                    if (track.secondSourceActive)
                    {
                        track.secondSource.volume = (float)Math.Pow(((float)track.info.volume / 10) * ((float)_masterVolume / 10), 0.5f);
                        track.source.volume = 0f;
                    }
                    else
                    {
                        track.source.volume = (float)Math.Pow(((float)track.info.volume / 10) * ((float)_masterVolume / 10), 0.5f);
                        track.secondSource.volume = 0f;
                    }
                }
                else
                {
                    track.source.volume = (float)Math.Pow(((float)track.info.volume / 10) * ((float)_masterVolume / 10), 0.5f);
                }
            }
        }
    }

    private AudioObject GetAudioFromAudioTrack(string name, AudioTrack track)
    {
        foreach (AudioObject obj in track.audio)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }

    private IEnumerator FadeAudio(AudioSource source, float initial, bool fadeState, float duration, AudioTrack track)
    {
        float volumeMultiplier = (float)Math.Pow(((float)track.info.volume / 10) * ((float)_masterVolume / 10), 0.5f);
        duration /= 1.5f;

        if ((track.info.mute || _masterMute))
        {
            volumeMultiplier = 0;
        }

        float target;
        float timer = 0f;

        if (fadeState)
        {
            target = volumeMultiplier;
        }
        else
        {
            target = 0f;
        }

        while (timer <= duration)
        {
            source.volume = Mathf.Lerp(initial, target, timer / duration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        source.volume = target;

        if (source.volume == 0f)
        {
            source.Stop();
        }
    }

    private IEnumerator FadePitch(AudioSource source, float target, float duration, AudioTrack track)
    {
        float pitchStart = source.pitch;

        if (duration > 0f)
        {
            duration /= 1.5f;

            float timer = 0f;

            while (timer <= duration)
            {
                source.pitch = Mathf.Lerp(pitchStart, target, timer / duration);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        source.pitch = target;
    }



    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Configure();
        }
    }

    private void OnDisable()
    {
        //dispose of coroutines in the event the object is deleted(to avoid things running in the background and potentionally causing a memory leak)
        Dispose();
    }

    void Update()
    {
        for (int i = 0; i < _tracks.Length; i++)
        {
            if (_tracks[i].info.advanced)
            {
                if (_tracks[i].secondSourceActive)
                {
                    if (_tracks[i].secondSource.time + 0.005 > _tracks[i].nextLoopTime)
                    {
                        Play(_tracks[i].currentAudio.name, false, false, true);
                    }
                }
                else
                {
                    if (_tracks[i].source.time + 0.005 > _tracks[i].nextLoopTime)
                    {
                        Play(_tracks[i].currentAudio.name, false, false, true);
                    }
                }
            }
        }

    }

    #endregion

    [System.Serializable]
    public class Debugging
    {
        public bool log;
        public bool inspector;
    }

    #region Log Functions

    private void Log(string message, bool shouldSendLog = false)
    {
        shouldSendLog = shouldSendLog || _debug.log;
        if (shouldSendLog) Debug.Log($"[Audio Manager]: {message}");
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"[Audio Manager]: {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[Audio Manager]: {message}");
    }

    #endregion
    /*
    #region Custom Editor

    [CustomPropertyDrawer(typeof(Debugging))]
    public class DebuggingDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            // Label
            GUI.contentColor = Color.HSVToRGB(40f / 360f, 0.4f, 0.8f);
            EditorGUI.LabelField(position, "Debug ", EditorStyles.boldLabel);

            // Content rects
            var logTextRect = new Rect(position.x + 80, position.y, 30, position.height);
            var logRect = new Rect(position.x + 110, position.y, 30, position.height);
            var inspectorTextRect = new Rect(position.x + 140, position.y, 60, position.height);
            var inspectorRect = new Rect(position.x + 200, position.y, 30, position.height);

            // Draw Content
            GUI.contentColor = Color.white;
            EditorGUI.LabelField(logTextRect, "Log");
            EditorGUI.PropertyField(logRect, property.FindPropertyRelative("log"), GUIContent.none);
            EditorGUI.LabelField(new Rect(logTextRect.x, logTextRect.y, logTextRect.width + logRect.width, logTextRect.height + logRect.height), new GUIContent("", "Should log messages be outputted to the console?"));

            EditorGUI.LabelField(inspectorTextRect, "Inspector");
            EditorGUI.PropertyField(inspectorRect, property.FindPropertyRelative("inspector"), GUIContent.none);
            EditorGUI.LabelField(new Rect(inspectorTextRect.x, inspectorTextRect.y, inspectorTextRect.width + inspectorRect.width, inspectorTextRect.height + inspectorRect.height), new GUIContent("", "Should the script in the inspector switch to a view with a bunch of detail?"));

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(AudioTrackInfo))]
    public class AudioTrackInfoDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            float bufferAmount = (position.width - 20) / 8;

            //Content Rects
            var nameRect = new Rect(position.x, position.y, bufferAmount * 4, position.height);
            var advTextRect = new Rect(position.x + bufferAmount * 4f + 5, position.y, 40, position.height);
            var advRect = new Rect(position.x + bufferAmount * 4f + 5 + 40, position.y, 30, position.height);

            //Drawing Content
            GUI.contentColor = Color.HSVToRGB(60f / 360f, 0.7f, 1);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("trackName"), GUIContent.none);
            EditorGUI.LabelField(nameRect, new GUIContent("", "The name of the track"));

            GUI.contentColor = Color.white;
            EditorGUI.LabelField(advTextRect, "ADV?");
            EditorGUI.PropertyField(advRect, property.FindPropertyRelative("advanced"), GUIContent.none);
            EditorGUI.LabelField(new Rect(advTextRect.x, advTextRect.y, advTextRect.width + advRect.width, advTextRect.height + advRect.height), new GUIContent("", "Should this track loop and fade?"));

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(AudioTrack))]
    public class AudioTrackDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            float bufferAmount = (position.width - 20) / 8;

            //Content Rects
            var nameRect = new Rect(position.x, position.y, bufferAmount * 4, position.height);
            var advTextRect = new Rect(position.x + bufferAmount * 4f + 5, position.y, 40, position.height);
            var advRect = new Rect(position.x + bufferAmount * 4f + 5 + 40, position.y, 30, position.height);


            //Drawing Content
            GUI.contentColor = Color.HSVToRGB(60f / 360f, 0.7f, 1);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("info"), GUIContent.none);
            EditorGUI.LabelField(nameRect, new GUIContent("", "The name of the track"));


            //Drawing Track Audio
            GUI.contentColor = Color.HSVToRGB(80f / 360f, 0.5f, 0.8f);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("audio"), new GUIContent($"{property.FindPropertyRelative("info").FindPropertyRelative("trackName").stringValue} Audio"));

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(AudioObject))]
    public class AudioObjectDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            float bufferAmount = (position.width - 20) / 8;

            //Content Rects
            var nameRect = new Rect(position.x, position.y, bufferAmount * 3f, position.height);
            var clipTextRect = new Rect(position.x + bufferAmount * 3f + 5, position.y, 15, position.height);
            var clipRect = new Rect(position.x + bufferAmount * 3f + 20, position.y, bufferAmount * 2, position.height);
            var introTextRect = new Rect(position.x + bufferAmount * 5f + 25, position.y, 15, position.height);
            var introRect = new Rect(position.x + bufferAmount * 5f + 40, position.y, bufferAmount * 2, position.height);

            //Drawing Content
            GUI.contentColor = Color.HSVToRGB(70f / 360f, 0.6f, 0.9f);
            GUI.backgroundColor = Color.HSVToRGB(70f / 360f, 0f, 0.8f);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.LabelField(nameRect, new GUIContent("", "The name of the audio"));

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            EditorGUI.LabelField(clipTextRect, "C");
            EditorGUI.PropertyField(clipRect, property.FindPropertyRelative("clip"), GUIContent.none);
            EditorGUI.LabelField(new Rect(clipTextRect.x, clipTextRect.y, clipTextRect.width + clipRect.width, clipTextRect.height + clipRect.height), new GUIContent("", "The audio clip used"));

            EditorGUI.LabelField(introTextRect, "I");
            EditorGUI.PropertyField(introRect, property.FindPropertyRelative("intro"), GUIContent.none);
            EditorGUI.LabelField(new Rect(introTextRect.x, introTextRect.y, introTextRect.width + introRect.width, introTextRect.height + introRect.height), new GUIContent("", "An optional intro that plays before the clip and doesnt loop"));

            EditorGUI.EndProperty();
        }
    }

    [CustomEditor(typeof(AudioManager))]
    [CanEditMultipleObjects]
    public class AudioManagerEditor : Editor {

        SerializedProperty debugProp;
        SerializedProperty tracksProp;

        void OnEnable() {
            // Setup the SerializedProperties.
            debugProp = serializedObject.FindProperty("_debug");
            tracksProp = serializedObject.FindProperty("_tracks");
        }



        public override void OnInspectorGUI() {
            serializedObject.Update();

            AudioManager audioManager = (AudioManager)target;

            int songCount = 0;
            int sfxCount = 0;
            foreach (AudioTrack track in audioManager._tracks) {
                if (track.info.advanced) songCount += track.audio.Length;
                else sfxCount += track.audio.Length;
            }

            string songText = songCount == 1 ? "ADV" : "ADV";
            string trackText = audioManager._tracks.Length == 1 ? "Track" : "Tracks";

            if (audioManager._debug.inspector == true) {
                GUILayout.Label($"{audioManager._tracks.Length} {trackText} | {songCount} {songText} | {sfxCount} NOM | Debug Mode");

                EditorGUILayout.PropertyField(debugProp);
                GUILayout.Label($"-----");
                GUIStyle style = new GUIStyle();
                style.richText = true;

                string masterMutedText = audioManager._masterMute ? "(Muted)" : "";
                GUILayout.Label($"Master Volume: {audioManager._masterVolume} {masterMutedText}");


                foreach (AudioTrack track in audioManager._tracks) {
                    string soundtrackText = track.info.advanced ? "(ADV)" : "";
                    string mutedText = track.info.mute ? "(Muted)" : "";
                    GUILayout.Label($"<size=15><color=#99992eFF><b>{track.info.trackName} {soundtrackText}</b></color> </size> <color=#bebebeFF> VOL: {track.info.volume} {mutedText} SOUNDS: {track.audio.Length} </color>", style);
                    if (track.source != null) {
                        string activeText = (!track.secondSourceActive) && track.source.isPlaying ? "(Active)" : "";
                        if (track.source.clip != null) {
                            GUILayout.Label($"---> Source TIME: {track.source.time} VOL: {track.source.volume} CLIP: {track.source.clip.name} {activeText}");
                        } else {
                            GUILayout.Label($"---> Source TIME: {track.source.time} VOL: {track.source.volume} CLIP: NONE {activeText}");
                        }

                    }
                    if (track.secondSource != null) {
                        string activeText = track.secondSourceActive && track.secondSource.isPlaying ? "(Active)" : "";

                        if (track.secondSource.clip != null) {
                            GUILayout.Label($"---> Source 2 TIME: {track.secondSource.time} VOL: {track.secondSource.volume} CLIP: {track.secondSource.clip.name} {activeText}");
                        } else {
                            GUILayout.Label($"---> Source 2 TIME: {track.secondSource.time} VOL: {track.secondSource.volume} CLIP: NONE {activeText}");
                        }
                    }

                }
            } else {
                GUILayout.Label($"{audioManager._tracks.Length} {trackText} | {songCount} {songText} | {sfxCount} NOM");

                EditorGUILayout.PropertyField(debugProp);
                GUI.contentColor = Color.HSVToRGB(60f / 360f, 0.5f, 0.8f);
                EditorGUILayout.PropertyField(tracksProp);
            }




            serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion
    */
}

[System.Serializable]
public class AudioTrackInfo
{
    public string trackName;
    public bool advanced;
    public float volume = 5;
    public bool mute;
}
