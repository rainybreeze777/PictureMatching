using System;
using UnityEngine;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class ClickDetector : View, IPointerClickHandler
{
    public Signal clickSignal = new Signal();

    public void OnPointerClick(PointerEventData eventData)
    {
        clickSignal.Dispatch();
    }
}