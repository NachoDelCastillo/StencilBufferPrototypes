using Core.CustomInput;
using Core.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Audio
{
    #region Enums y DataClasses

    // Enums para identificar audio
    public enum SFX_Id
    {
        Jump,
        Read,
        StandUp,
        Door,

        // UI
        UISelect,
        UIFocusOption,
        ExitRoom,

        // Gameplay
        //Parry? >:>
    }
    public enum Music_Id
    {
        None,

        KKSlider_Lofi,
        AnimalCrossing_NewLeaf_8PM_Lofi,
        AnimalCrossing_NewHorizons_Theme_Lofi,
        Hakaisu_MellowSkies,
        Revolutxy_KK_Lofi,
        Nujabes_CountingStars
    }

    [System.Serializable]
    public class SFXData
    {
        public SFX_Id id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [System.Serializable]
    public class MusicData
    {
        public Music_Id id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    #endregion

    /// <summary>
    /// Administrador central de audio.
    /// Gestiona música y efectos usando enums, controla volúmenes globales
    /// (master, música y SFX), aplica fades y guarda la configuración con PlayerPrefs.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        #region Inspector

        [Header("Audio Clips")]
        [SerializeField] private List<SFXData> sfxClips;
        [SerializeField] private List<MusicData> musicClips;

        [Header("Volume Settings")]
        //[ReadOnly, SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        //[ReadOnly, SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
        //[ReadOnly, SerializeField, Range(0f, 1f)] private float musicVolume = 1f;

        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;

        #endregion

        #region PlayerPrefs Keys

        private const string MASTER_KEY = "MasterVolume";
        private const string SFX_KEY = "SFXVolume";
        private const string MUSIC_KEY = "MusicVolume";

        #endregion

        #region Variables

        // Datos de los sonidos/musica
        private Dictionary<SFX_Id, SFXData> sfxDictionary;
        private Dictionary<Music_Id, MusicData> musicDictionary;

        // Componentes AudioSources responsables de reproducir los sonidos/musica
        private AudioSource musicSource;
        private List<AudioSource> sfxSources = new List<AudioSource>();

        // Recuento de cuando fue el ultimo frame que se ejecuto un sonido en concreto
        // Se utiliza para no reproducir el mismo sonido multiples veces en la misma iteracion
        private Dictionary<SFX_Id, int> sfxLastPlayedFrame = new Dictionary<SFX_Id, int>();

        #endregion

        #region Initialize

        protected override void Awake()
        {
            base.Awake();

            InitializeDictionaries();
            InitializeMusicSource();
            LoadVolumeSettings();
        }

        private void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat(MASTER_KEY, .5f);
            sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
            musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);

            UpdateVolumes();
        }

        private void InitializeDictionaries()
        {
            sfxDictionary = new Dictionary<SFX_Id, SFXData>();
            foreach (var sfx in sfxClips)
                sfxDictionary[sfx.id] = sfx;

            musicDictionary = new Dictionary<Music_Id, MusicData>();
            foreach (var music in musicClips)
                musicDictionary[music.id] = music;
        }

        private void InitializeMusicSource()
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.volume = musicVolume * masterVolume;
        }

        #endregion

        #region Playback Methods

        #region Sound Logic

        /// <summary>
        /// Reproduce un SFX por su ID.
        /// </summary>
        public void PlaySFX(SFX_Id id, bool randomPitch = false, float pitch = 1)
        {
            if (!sfxDictionary.TryGetValue(id, out SFXData data))
            {
                Debug.LogWarning($"SFX {id} not found!");
                return;
            }

            // No permitir reproducir el mismo sonido multiples veces en un mismo frame
            if (sfxLastPlayedFrame.TryGetValue(id, out int lastFrame))
            {
                if (lastFrame == Time.frameCount)
                    return; // ya se intentó reproducir este frame, no volver a reproducirlo
            }
            sfxLastPlayedFrame[id] = Time.frameCount;

            AudioSource srcToUse = GetAvailableSFXSource();
            srcToUse.clip = data.clip;
            srcToUse.volume = data.volume * sfxVolume * masterVolume;
            srcToUse.loop = false;

            if (randomPitch)
                srcToUse.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            else
                srcToUse.pitch = pitch;

            srcToUse.Play();
        }

        public void PlaySFX(AudioClip clip, float volume = 1f, bool randomPitch = false)
        {
            if (clip == null)
            {
                Debug.LogWarning("PlaySFX called with null AudioClip!");
                return;
            }

            AudioSource srcToUse = GetAvailableSFXSource();
            srcToUse.clip = clip;
            srcToUse.volume = volume * sfxVolume * masterVolume;
            srcToUse.loop = false;

            if (randomPitch)
                srcToUse.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            else
                srcToUse.pitch = 1f;

            srcToUse.Play();
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (var src in sfxSources)
            {
                if (!src.isPlaying) return src;
            }

            AudioSource newSrc = gameObject.AddComponent<AudioSource>();
            sfxSources.Add(newSrc);
            return newSrc;
        }

        #endregion

        #region Music Logic

        /// <summary>
        /// Cambia la música con fade out de la actual y fade in de la nueva.
        /// </summary>
        public void CrossfadeToMusic(Music_Id musicId, float fadeDuration = 1f)
        {
            if (!musicDictionary.TryGetValue(musicId, out MusicData data))
            {
                Debug.LogWarning($"Music {musicId} not found!");
                return;
            }

            if (musicSource.clip == data.clip) return;

            StartCoroutine(CrossfadeRoutine_Coroutine(musicId, fadeDuration));
        }

        private IEnumerator CrossfadeRoutine_Coroutine(Music_Id musicId, float duration = 1f)
        {
            // Fade out de la música actual
            yield return FadeOutMusic_Coroutine(duration);

            // Fade in de la nueva música
            yield return FadeInMusic_Coroutine(musicId, duration);
        }

        /// <summary>
        /// Baja el volumen actual del AudioSource a 0 en la duración indicada.
        /// </summary>
        public void FadeOutMusic(float duration = 1f) => StartCoroutine(FadeOutMusic_Coroutine(duration));
        private IEnumerator FadeOutMusic_Coroutine(float duration = 1f)
        {
            float startVolume = musicSource.volume;
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }

            musicSource.clip = null;
            musicSource.volume = 0f;
        }

        /// <summary>
        /// Sube el volumen del AudioSource hasta el objetivo en la duración indicada.
        /// </summary>
        public void FadeInMusic(Music_Id musicId, float duration = 1f) => StartCoroutine(FadeInMusic_Coroutine(musicId, duration));
        private IEnumerator FadeInMusic_Coroutine(Music_Id musicId, float duration = 1f)
        {
            if (!musicDictionary.TryGetValue(musicId, out MusicData musicData))
            {
                Debug.LogWarning($"Music {musicId} not found!");
                yield break;
            }

            // Cambiar clip y reproducir
            musicSource.clip = musicData.clip;
            musicSource.Play();

            float t = 0f;
            float targetVolume = musicData.volume * musicVolume * masterVolume;

            while (t < duration)
            {
                t += Time.unscaledTime;
                musicSource.volume = Mathf.Lerp(0f, targetVolume, t / duration);
                yield return null;
            }

            musicSource.volume = targetVolume;
        }

        #endregion

        #endregion

        #region Volume Management

        public float MasterVolume => masterVolume;
        public float SfxVolume => sfxVolume;
        public float MusicVolume => musicVolume;

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(MASTER_KEY, masterVolume);
            UpdateVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SFX_KEY, sfxVolume);
            UpdateVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(MUSIC_KEY, musicVolume);
            UpdateVolumes();
        }

        /// <summary>
        /// Actualiza los volúmenes de todas las fuentes activas.
        /// </summary>
        private void UpdateVolumes()
        {
            // Buscar el MusicData que se esta ejecutando en este momento
            foreach (MusicData musicData in musicDictionary.Values)
            {
                if (musicData.clip == musicSource.clip)
                {
                    musicSource.volume = musicData.volume * musicVolume * masterVolume;
                    break;
                }
            }

            // Recorrer todos los AudioSource de sonido
            foreach (AudioSource src in sfxSources)
            {
                if (src.clip == null) continue;

                // Buscar el SFXData que se esta ejecutando en este AudioSource y modificarle el valor
                foreach (SFXData sfxData in sfxDictionary.Values)
                {
                    if (sfxData.clip == src.clip)
                    {
                        src.volume = sfxData.volume * sfxVolume * masterVolume;
                        break;
                    }
                }
            }
        }

        public void ResetPlayerPrefVolumeSettings()
        {
            // Borrar los PlayerPrefs
            PlayerPrefs.DeleteKey(MASTER_KEY);
            PlayerPrefs.DeleteKey(SFX_KEY);
            PlayerPrefs.DeleteKey(MUSIC_KEY);

            LoadVolumeSettings();
        }

        #endregion
    }

}