using System;
using System.Collections;
using System.Collections.Generic;
using Magneto.Track;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    private TrackManager.TrackGenerationSystem _generator;
    
    // Start is called before the first frame update
    void Start()
    {
        _generator = new TrackManager.TrackGenerationSystem();
        _generator.Init();
    }

    private int _frameCounter = 0;
    void Update()
    {
        if (_generator == null) return;
        if (!_generator.IsRunning)
        {
            _generator.Schedule();
        }
        else if (_frameCounter >= TrackManager.JOB_EXECUTION_MAXIMUM_FRAMES)
        {
            _generator.Complete();
            _generator = null;
        }
        else
        {
            _frameCounter++;
        }
    }

    private void OnDestroy()
    {
        _generator?.Complete();
        _generator = null;
    }
}
