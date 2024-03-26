using System.Collections.Generic;
using Components;
using Tools;
using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] private SoundPlayer soundPlayerPrefab;
        [SerializeField] private AudioClip[] soundClips;
   
        Dictionary<string, AudioClip> _soundDictionary = new Dictionary<string, AudioClip>();
        private void Start()
        {
            BuildDictionary();
        }
   
        void BuildDictionary()
        {
            foreach (var clip in soundClips)
            {
                _soundDictionary.Add(clip.name, clip);
            }
        }

        public void PlaySound(string sound)
        {
            var soundPlayer = Instantiate(soundPlayerPrefab, transform);
            soundPlayer.SetupSound(_soundDictionary[sound]);
        }

        public void PlaySound(string sound, float count)
        {
            var  pitch = 1 + (count - 1) * 0.1f;
            var soundPlayer = Instantiate(soundPlayerPrefab, transform);
            soundPlayer.SetupSound(_soundDictionary[sound], pitch);
        }
    }
}
