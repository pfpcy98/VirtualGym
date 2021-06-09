using System;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine;

public class GestureEventArgs : EventArgs {
	
	public bool m_isBodyTrackingIdValid { get; private set; }
	public bool m_isGestureDetected { get; private set; }
	public float m_detectionConfidence { get; private set; }

	public string m_gestureID { get; private set; }

	public GestureEventArgs(bool isBodyTrackingIdValid, bool isGestureDetected, float detectionConfidence, string gestureID)
	{
		this.m_isBodyTrackingIdValid = isBodyTrackingIdValid;
		this.m_isGestureDetected = isGestureDetected;
		this.m_detectionConfidence = detectionConfidence;
		this.m_gestureID = gestureID;
	}
}

public class GestureDetector : IDisposable
{
    private readonly string m_dumbbellDB;

    private VisualGestureBuilderFrameSource m_vgbFrameSource = null;
    private VisualGestureBuilderFrameReader m_gbFrameReader = null;

    public event EventHandler<GestureEventArgs> OnGestureDetected;

    public GestureDetector(KinectSensor kinectSensor, int DB_Index)
    {
        if (kinectSensor == null)
        {
            throw new ArgumentNullException("kinectSensor");
        }

        this.m_vgbFrameSource = VisualGestureBuilderFrameSource.Create(kinectSensor, 0);
        this.m_vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;
        
        this.m_gbFrameReader = this.m_vgbFrameSource.OpenReader();
        if (this.m_gbFrameReader != null)
        {
            this.m_gbFrameReader.IsPaused = true;
            this.m_gbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
        }

        switch (DB_Index)
        {
            case 0:
                m_dumbbellDB = "05.Models\\GestureDB\\Dumbbell.gbd";
                break;

            case 1:
                m_dumbbellDB = "05.Models\\GestureDB\\MoveMotion.gbd";
                break;
        }

        var databasePath = Path.Combine(Application.dataPath, this.m_dumbbellDB);
        using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(databasePath))
        {
            foreach (Gesture gesture in database.AvailableGestures)
            {
                m_vgbFrameSource.AddGesture(gesture);
            }
        }
    }

    public GestureDetector(KinectSensor kinectSensor, string path)
    {
        if (kinectSensor == null)
        {
            throw new ArgumentNullException("kinectSensor");
        }

        this.m_vgbFrameSource = VisualGestureBuilderFrameSource.Create(kinectSensor, 0);
        this.m_vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

        this.m_gbFrameReader = this.m_vgbFrameSource.OpenReader();
        if (this.m_gbFrameReader != null)
        {
            this.m_gbFrameReader.IsPaused = true;
            this.m_gbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
        }
        
        var databasePath = Path.Combine(Application.dataPath, Path.Combine("05.Models\\GestureDB", path));
        using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(databasePath))
        {
            foreach (Gesture gesture in database.AvailableGestures)
            {
                m_vgbFrameSource.AddGesture(gesture);
            }
        }
    }

    public ulong TrackingId
    {
        get
        {
            return this.m_vgbFrameSource.TrackingId;
        }

        set
        {
            if (this.m_vgbFrameSource.TrackingId != value)
            {
                this.m_vgbFrameSource.TrackingId = value;
            }
        }
    }

    public bool IsPaused
    {
        get
        {
            return this.m_gbFrameReader.IsPaused;
        }

        set
        {
            if (this.m_gbFrameReader.IsPaused != value)
            {
                this.m_gbFrameReader.IsPaused = value;
            }
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (this.m_gbFrameReader != null)
            {
                this.m_gbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                this.m_gbFrameReader.Dispose();
                this.m_gbFrameReader = null;
            }

            if (this.m_vgbFrameSource != null)
            {
                this.m_vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                this.m_vgbFrameSource.Dispose();
                this.m_vgbFrameSource = null;
            }
        }
    }

    private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
    {
        VisualGestureBuilderFrameReference frameReference = e.FrameReference;
        using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
        {
            if (frame == null) { return; }

            var discreteResults = frame.DiscreteGestureResults;
            if (discreteResults == null) { return; }
            
            foreach (Gesture gesture in this.m_vgbFrameSource.Gestures)
            {
                if(gesture.GestureType == GestureType.Discrete)
                {
                    DiscreteGestureResult result = null;
                    discreteResults.TryGetValue(gesture, out result);
                    if (result == null) { return; }

                    if (OnGestureDetected != null && result.Detected == true)
                    {
                        OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, gesture.Name));
                    }
                }
            }
        }
    }

    private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
    {
        if (this.OnGestureDetected != null)
        {
            this.OnGestureDetected(this, new GestureEventArgs(false, false, 0.0f, "none"));
        }
    }
}

