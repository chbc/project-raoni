﻿using System;
using System.Collections;
using Gamekit2D;
using UnityEngine;
using UnityEngine.Video;

namespace ProjectRaoni
{
    public class CutsceneController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer = null;
        [SerializeField] private TransitionPoint transitionPoint = null;

        private IEnumerator Start()
        {
            this.videoPlayer.Play();

            yield return null;
            
            while (this.videoPlayer.isPlaying)
                yield return null;
            
            SceneController.TransitionToScene(this.transitionPoint);
        }
    }
}