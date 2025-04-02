using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ph.Managers {
    public class BgMusic : MonoBehaviour {
        public AudioSource audioSource;
        private List<AudioClip> musicTracks = new List<AudioClip>();
        private Queue<AudioClip> trackQueue = new Queue<AudioClip>();
        private AudioClip lastPlayedTrack;

        void Start() {
            LoadMusic();
            ShuffleAndQueueTracks();
            PlayNextTrack();
        }

        private void LoadMusic() {
            musicTracks = Resources.LoadAll<AudioClip>("Music").ToList();
            if (musicTracks.Count == 0) {
                Debug.LogError("No music tracks found in Resources/Music");
            }
        }

        private void ShuffleAndQueueTracks() {
            List<AudioClip> shuffledTracks;
            do {
                shuffledTracks = musicTracks.OrderBy(x => Random.value).ToList();
            }
            while (shuffledTracks[0] == lastPlayedTrack && musicTracks.Count > 1);

            trackQueue = new Queue<AudioClip>(shuffledTracks);
        }

        private void PlayNextTrack() {
            if (trackQueue.Count == 0) {
                ShuffleAndQueueTracks();
            }

            lastPlayedTrack = trackQueue.Dequeue();
            audioSource.clip = lastPlayedTrack;
            audioSource.Play();
            StartCoroutine(WaitForTrackToEnd());
        }

        private IEnumerator WaitForTrackToEnd() {
            yield return new WaitForSeconds(audioSource.clip.length);
            PlayNextTrack();
        }
    }
}
