﻿/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
namespace WooAudio
{
    [System.Serializable]
    public class AudioPref
    {
        [UnityEngine.SerializeField]
        private SerializableDictionary<int, float> pairs = new SerializableDictionary<int, float>();
        internal float GetVolume(int channel)
        {
            float vol = -1;
            pairs.TryGetValue(channel, out vol);
            return vol;
        }

        internal void SetVolume(int channel, float volume)
        {
            pairs[channel] = volume;
        }
    }
}