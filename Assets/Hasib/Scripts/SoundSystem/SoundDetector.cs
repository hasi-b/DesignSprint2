using UnityEngine;

public class SoundDetector : MonoBehaviour, ISoundListener
{



    public float detectionRadius = 10f;

    private Vector3 lastHeardPosition;
    private bool heardSound = false;

    public void OnSoundHeard(Vector3 soundSourcePosition)
    {
        float distance = Vector3.Distance(transform.position, soundSourcePosition);
        if (distance <= detectionRadius)
        {
            heardSound = true;
            lastHeardPosition = soundSourcePosition;
            Debug.Log($"{gameObject.name} heard a sound at {soundSourcePosition}");
        }
    }

    void Update()
    {
        if (heardSound)
        {

           // Debug.Log("Sound heard");
            // Move towards or investigate the sound
            // Example: transform.position = Vector3.MoveTowards(...)
        }
    }



}
