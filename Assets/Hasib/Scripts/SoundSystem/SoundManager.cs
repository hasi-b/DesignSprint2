using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static void BroadcastSound(Vector3 position, float radius)
    {
        Collider[] listeners = Physics.OverlapSphere(position, radius);
        foreach (var listener in listeners)
        {
            ISoundListener soundListener = listener.GetComponent<ISoundListener>();
            if (soundListener != null)
            {
                soundListener.OnSoundHeard(position);
            }
        }
    }
}
