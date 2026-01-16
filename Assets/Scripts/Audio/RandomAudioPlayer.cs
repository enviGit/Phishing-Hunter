using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ph.Audio {
    public class RandomAudioPlayer : MonoBehaviour {
        private List<AudioSource> audioSources = new List<AudioSource>();
        public float maxDuration = 3f;

        void Start() {
            audioSources.AddRange(GetComponentsInChildren<AudioSource>());
            StartCoroutine(PlayRandomAudioWithDelay());
        }
        private IEnumerator PlayRandomAudioWithDelay() {
            float remainingTime = maxDuration;

            while (audioSources.Count > 0 && remainingTime > 0f) {
                int randomIndex = Random.Range(0, audioSources.Count);
                AudioSource selectedAudio = audioSources[randomIndex];
                selectedAudio.Play();

                float audioDuration = selectedAudio.clip.length;
                remainingTime -= audioDuration;

                if (remainingTime > 0f) {
                    float randomDelay = Mathf.Min(Random.Range(0.05f, 1f), remainingTime);
                    remainingTime -= randomDelay;
                }

                audioSources.RemoveAt(randomIndex);

                yield return new WaitForSeconds(Random.Range(0.05f, 1f));
            }
        }
    }
}
