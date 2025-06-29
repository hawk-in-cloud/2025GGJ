using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    public class AudioManager : Singleton<AudioManager>
    {
        // Background music playback component
        private AudioSource bkMusic = null;

        // Background music volume
        private float bkMusicValue = 0.5f;
        public float BkMusicValue
        {
            get
            {
                return bkMusicValue;
            }
        }

        // Manage currently playing sound effects
        private List<AudioSource> soundList = new List<AudioSource>();
        public List<AudioSource> SoundList
        {
            get
            {
                return soundList;
            }
        }
        // Sound effect volume
        private float soundValue = 0.5f;
        public float SoundValue
        {
            get
            {
                return soundValue;
            }
        }
        // Whether sound effects are playing
        private bool soundIsPlay = true;

        private AudioManager()
        {
            MonoManager.Instance.AddFixedUpdateListener(Update);
        }


        private void Update()
        {
            if (!soundIsPlay)
                return;

            // Continuously traverse the container to check if any sound effects have finished playing. If they have, remove and destroy them.
            // To avoid issues with modifying the list while traversing, we traverse in reverse order.
            for (int i = soundList.Count - 1; i >= 0; --i)
            {
                if (!soundList[i].isPlaying)
                {
                    // The sound effect has finished playing and is no longer in use. We set the clip to null.
                    soundList[i].clip = null;
                    PoolManager.Instance.PushObj(soundList[i].gameObject);
                    soundList.RemoveAt(i);
                }
            }
        }


        // Play background music
        public void PlayBKMusic(string name)
        {
            // Dynamically create a component to play background music, and ensure it is not destroyed when changing scenes.
            // This ensures that background music continues to play across scene changes.
            if (bkMusic == null)
            {
                GameObject obj = new GameObject();
                obj.name = "BKMusic";
                GameObject.DontDestroyOnLoad(obj);
                bkMusic = obj.AddComponent<AudioSource>();
            }

            // Play background music based on the provided name
            ABResManager.Instance.LoadResAsync<AudioClip>("Music", name, (clip) =>
            {
                bkMusic.clip = clip;
                bkMusic.loop = true;
                bkMusic.volume = bkMusicValue;
                bkMusic.Play();
            });
        }

        // Stop background music
        public void StopBKMusic()
        {
            if (bkMusic == null)
                return;
            bkMusic.Stop();
        }

        // Pause background music
        public void PauseBKMusic()
        {
            if (bkMusic == null)
                return;
            bkMusic.Pause();
        }

        public void ResumeBKMusic()
        {
            if (bkMusic == null)
                return;
            bkMusic.UnPause(); // 继续播放暂停的音频
        }

        // Set background music volume
        public void ChangeBKMusicValue(float v)
        {
            bkMusicValue = v;
            if (bkMusic == null)
                return;
            bkMusic.volume = bkMusicValue;
        }

        /// <summary>
        /// Play sound effect
        /// </summary>
        /// <param name="name">Sound effect name</param>
        /// <param name="isLoop">Whether to loop</param>
        /// <param name="isSync">Whether to load synchronously</param>
        /// <param name="callBack">Callback after loading is complete</param>
        public void PlaySound(string name, bool isLoop = false, bool isSync = false, UnityAction<AudioSource> callBack = null)
        {
            // Load sound effect resource and play it
            ABResManager.Instance.LoadResAsync<AudioClip>("Sound", name, (clip) =>
            {
                // Get the sound effect object from the object pool and obtain the corresponding component
                AudioSource source = PoolManager.Instance.GetObj("SoundComponent", "soundObj").GetComponent<AudioSource>();
                // If the sound effect retrieved was previously in use, stop it first
                source.Stop();

                source.clip = clip;
                source.loop = isLoop;
                source.volume = soundValue;
                source.Play();
                // Store in the container for recording purposes, to facilitate later checks on whether it has stopped
                // Since retrieving an object from the pool might return a previously used one (when exceeding the limit),
                // we need to check if the container already contains it before adding it to avoid duplicates
                if (!soundList.Contains(source))
                    soundList.Add(source);
                // Pass to external use
                callBack?.Invoke(source);
            }, isSync);
        }

        /// <summary>
        /// Stop playing sound effect
        /// </summary>
        /// <param name="source">Sound effect component object</param>
        public void StopSound(AudioSource source)
        {
            if (soundList.Contains(source))
            {
                // Stop playing
                source.Stop();
                // Remove from the container
                soundList.Remove(source);
                // Clear the clip to avoid占用
                source.clip = null;
                // Return to the object pool
                PoolManager.Instance.PushObj(source.gameObject);
            }
        }

        /// <summary>
        /// Change sound effect volume
        /// </summary>
        /// <param name="v"></param>
        public void ChangeSoundValue(float v)
        {
            soundValue = v;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].volume = v;
            }
        }

        /// <summary>
        /// Resume or pause all sound effects
        /// </summary>
        /// <param name="isPlay">Whether to resume playing (true for play, false for pause)</param>
        public void PlayOrPauseSound(bool isPlay)
        {
            if (isPlay)
            {
                soundIsPlay = true;
                for (int i = 0; i < soundList.Count; i++)
                    soundList[i].Play();
            }
            else
            {
                soundIsPlay = false;
                for (int i = 0; i < soundList.Count; i++)
                    soundList[i].Pause();
            }
        }

        /// <summary>
        /// Clear sound effect records <b>Call this before clearing the object pool when changing scenes</b>
        /// </summary>
        public void ClearSound()
        {
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Stop();
                soundList[i].clip = null;
                PoolManager.Instance.PushObj(soundList[i].gameObject);
            }
            soundList.Clear();
        }
    }
}
