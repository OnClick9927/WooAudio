/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using UnityEngine;

namespace WooAudio
{
    class AudioPlayer
    {
        private AudioSource _source;
        private float volume = 0f;
        private bool _stop;
        private bool _loading;
        private int sound_id;
        public bool IsWork => _source.isPlaying || _loading;
        public AudioAsset asset { get; private set; }
        public AudioPlayer(AudioSource source)
        {
            _source = source;
            _source.playOnAwake = false;
        }
        private float GetTargetVolume(float percent) => volume * (1 + percent);
        public void SetVolume(float volume)
        {
            this.volume = volume;
            if (_source.isPlaying && sound_id != 0)
                _source.volume = GetTargetVolume(Audio.ins.config.GetSoundVolume(sound_id));
        }


        private void PlayAudio()
        {
            if (!_stop)
            {
                AudioClip clip = asset.GetClip();
                _source.clip = clip;
                _source.volume = GetTargetVolume(Audio.ins.config.GetSoundVolume(sound_id));
                _source.loop = Audio.ins.config.GetSoundLoop(sound_id);
                _source.Play();
            }
        }
        public void Play(int sound_id)
        {
            _stop = false;
            this.sound_id = sound_id;
            asset = Audio.Prepare(sound_id);
            if (asset.isDone)
                PlayAudio();
            else
                _loading = true;
        }
        public void Update()
        {
            if (_loading)
            {
                if (!asset.isDone || _stop) return;
                if (asset.isDone)
                {
                    _loading = false;
                    PlayAudio();
                }
            }
        }
        public void Stop()
        {
            if (_stop) return;
            if (_loading)
            {
                _loading = false;
            }
            _stop = true;
            _source.Stop();
            _source.clip = null;
            Audio.ReleaseAsset(asset);
            asset = null;
        }


    }

}
