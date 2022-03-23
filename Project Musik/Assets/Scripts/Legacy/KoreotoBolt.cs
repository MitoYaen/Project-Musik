using UnityEngine;
using Bolt;
using SonicBloom.Koreo;

public class KoreotoBolt : MonoBehaviour
{
    [EventID]
    public string eventID;
    void Start()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, TriggerBoltEvent);
    }
    void TriggerBoltEvent (KoreographyEvent evt,int sampleTime,int sampleDelta,DeltaSlice deltaSlice)
    {
        CustomEvent.Trigger(gameObject, "Koregrapher", evt, sampleTime, sampleDelta, deltaSlice);
    }
}
