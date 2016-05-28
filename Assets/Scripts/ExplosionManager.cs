using UnityEngine;
using System.Collections;

public class ExplosionManager : MonoBehaviour
{
    public ParticleSystem EnemyExplosion;

	void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}

    public void CreateExplosion(Vector3 position, Vector3 direction)
    {
        if (EnemyExplosion != null)
        {
            var explosion = Instantiate(EnemyExplosion);
            explosion.transform.position = position;

            var velocity = explosion.velocityOverLifetime;
            velocity.enabled = true;
            velocity.x = new ParticleSystem.MinMaxCurve(direction.x * 15.0f);
            velocity.y = new ParticleSystem.MinMaxCurve(direction.y * 15.0f);
            velocity.z = new ParticleSystem.MinMaxCurve(direction.z * 15.0f);
        }
    }
}
