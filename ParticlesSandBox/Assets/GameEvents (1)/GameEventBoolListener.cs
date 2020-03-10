using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventBoolListener : MonoBehaviour
{
    public GameEventBool Event;
    public bool inverseParameter;
    public BoolEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(bool value)
    {
        if(inverseParameter)
            Response.Invoke(!value);
        else
            Response.Invoke(value);
    }
}

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }
