﻿/*********************************************************************************
 *Author:         anonymous
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-08-13
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace WooAudio
{
    class AudioChannel
    {
        private Queue<AudioPlayer> sleeps = new Queue<AudioPlayer>();
        private List<AudioPlayer> players = new List<AudioPlayer>();
        private int index;
        public int channel { get; private set; }
        private float volume = 0.5f;
        private AudioPlayer Get()
        {
            AudioPlayer player = null;
            if (sleeps.Count > 0)
                player = sleeps.Dequeue();
            else
            {
                var source = new GameObject($"{channel}_{index++}").AddComponent<AudioSource>();
                source.transform.SetParent(Audio.ins.transform);
                player = new AudioPlayer(source);
            }
            player.SetVolume(volume);
            players.Add(player);
            return player;
        }

        public AudioChannel(int channel)
        {
            this.channel = channel;
        }
        public void Play(int sound_id)
        {
            if (Audio.ins.config.GetSoundCover(sound_id))
                ShutDown();
            Get().Play(sound_id);
        }

        public void SetVolume(float volume)
        {
            this.volume = volume;
            for (int i = 0; i < players.Count; i++)
                players[i].SetVolume(volume);
        }
        private void ShutDown(AudioPlayer player)
        {
            player.Stop();
            players.Remove(player);
            sleeps.Enqueue(player);
        }
        public void Update()
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                players[i].Update();
                if (!players[i].IsWork)
                    ShutDown(players[i]);
            }
        }
        public void ShutDown()
        {
            for (int i = players.Count - 1; i >= 0; i--)
                ShutDown(players[i]);
        }
        public void Release()
        {
            ShutDown();
        }

        public void Stop(int sound_id, bool all)
        {
            if (all)
            {
                var _players = players.FindAll(x => x.sound_id == sound_id);
                if (_players != null)
                    foreach (var player in _players)
                        ShutDown(player);
            }
            else
            {

                var player = players.Find(x => x.sound_id == sound_id);
                if (player != null)
                    ShutDown(player);
            }
        }
    }

}