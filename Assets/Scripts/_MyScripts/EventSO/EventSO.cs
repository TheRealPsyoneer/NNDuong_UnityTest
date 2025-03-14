using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event SO", menuName = "Event SO")]
public class EventSO : ScriptableObject
{
    public event Action ThingHappened;
    public event Action<Cell> ThingHappenedCell;

    public void Broadcast()
    {
        ThingHappened?.Invoke();
    }

    public void Broadcast(Cell cell)
    {
        ThingHappenedCell?.Invoke(cell);
    }
}
