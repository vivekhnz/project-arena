using UnityEngine;
using System.Collections;

public class ShockwaveController : PooledObject
{
    public void Initialize(Vector3 position)
    {
        transform.position = position;
        
        var audioControllerObj = GameObject.FindGameObjectWithTag("AudioController");
        if (audioControllerObj != null)
        {
            var audioController = audioControllerObj.GetComponent<AudioController>();
            audioController.ApplyShockwaveEffect();
        }
    }
}
