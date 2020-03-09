using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private Transform observedTransform;
    public List<Vector3> frames = new List<Vector3>();

    public Path (Transform observedTransform)
    {
        this.observedTransform = observedTransform;
    }

    public void AddFrame()
    {
        frames.Add(observedTransform.position);
    }

    public void SetFrame(int frameIndex)
    {
        observedTransform.position = frames[frameIndex];
    }

    public Vector3 GetFirstFrame()
    {
        return frames[0];
    }

    public Vector3 GetLastFrame()
    {
        return frames[frames.Count - 1];
    }

    public void ClearFrames()
    {
        frames.Clear();
    }
}