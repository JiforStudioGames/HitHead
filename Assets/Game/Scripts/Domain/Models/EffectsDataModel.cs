using System;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Game.Scripts.Domain.Models
{
    [Serializable]
    public struct EffectsDataModel
    {
        public UnityEngine.Camera MainCamera;
        public Volume GlobalVolume;
        public AudioMixerGroup SfxMixer;
    }
}