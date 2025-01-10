/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace WooAudio
{
    public class Audio : MonoBehaviour
    {

        internal IAudioConfig config;
        internal IAudioPrefRecorder recorder;
        internal IAudioAsset asset;
        AudioPref pref;

        private static Dictionary<int, AudioChannel> channels;
        private static Audio _ins;

        internal static Audio ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new GameObject("Audio").AddComponent<Audio>();
                    DontDestroyOnLoad(ins.gameObject);
                }
                return _ins;
            }
        }
        private static Dictionary<string, AudioAsset> assets;

        public static void Init(IAudioPrefRecorder recorder, IAudioAsset asset)
        {

            ins.asset = asset;
            ins.recorder = recorder;
            ins.pref = recorder.Read();
            assets = new Dictionary<string, AudioAsset>();
            channels = new Dictionary<int, AudioChannel>();
        }
        public static void SetDefaultVolume(int channel, float vol)
        {
            if (GetVolume(channel) != -1) return;
            SetVolume(channel, vol);
        }
        public static void SetConfig(IAudioConfig config)
        {
            ins.config = config;
        }


        private void Update()
        {
            foreach (var channel in channels.Values)
            {
                channel.Update();
            }
            float delta = Time.deltaTime;
            foreach (var item in assets.Values)
            {
                if (item.MinusTime(delta) <= 0)
                    AddToRelease(item);
            }
            RleaseAssets();
        }
        private void OnDisable()
        {

            foreach (var channel in channels.Values)
                channel.Release();
            foreach (var item in assets.Values)
                AddToRelease(item);
            RleaseAssets();
        }

        public static void SetVolume(int channel, float volume)
        {
            ins.pref.SetVolume(channel, volume);
            ins.recorder.Write(ins.pref);

            AudioChannel chan = GetChannel(channel);
            chan.SetVolume(volume);
        }
        public static float GetVolume(int channel) => ins.pref.GetVolume(channel);
        public static void Play(int sound_id)
        {
            AudioChannel chan = GetChannel(ins.config.GetSoundChannel(sound_id));
            chan.Play(sound_id);
        }
        public static void Stop(int sound_id)
        {
            AudioChannel chan = GetChannel(ins.config.GetSoundChannel(sound_id));
            chan.Stop(ins.config.GetSoundPath(sound_id));
        }

        public static void ShutDown(int channel)
        {
            AudioChannel chan = GetChannel(channel);
            chan.ShutDown();
        }


        private static AudioChannel GetChannel(int channel)
        {
            AudioChannel chan;
            if (!channels.TryGetValue(channel, out chan))
            {
                chan = new AudioChannel(channel);
                channels.Add(channel, chan);
            }
            return chan;
        }
        private static Queue<AudioAsset> release = new Queue<AudioAsset>();
        private static void RleaseAssets()
        {
            while (release.Count > 0)
            {
                var item = release.Dequeue();
                assets.Remove(item.path);
                item.ReleaseAsset();
            }
        }
        private static void AddToRelease(AudioAsset asset) => release.Enqueue(asset);
        internal static AudioAsset Prepare(int sound_id)
        {
            string path = ins.config.GetSoundPath(sound_id);
            AudioAsset asset;
            if (!assets.TryGetValue(path, out asset))
            {
                asset = new AudioAsset(path, ins.config.GetSoundExistTime(sound_id));
                assets.Add(path, asset);
            }
            asset.Retain();
            return asset;
        }
        internal static void ReleaseAsset(AudioAsset asset) => asset.Release();
    }
}
