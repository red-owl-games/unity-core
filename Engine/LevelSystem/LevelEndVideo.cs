﻿using UnityEngine;
using UnityEngine.Video;

namespace RedOwl.Core
{
    [RequireComponent(typeof(VideoPlayer))]
    public class LevelEndVideo : MonoBehaviour
    {
        void Awake()
        {
            var videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.loopPointReached += v => LevelManager.LoadNextLevel();
        }
    }
}

