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

    public void CreateExplosion(Vector3 position)
    {
        if (EnemyExplosion != null)
        {
            var explosion = Instantiate(EnemyExplosion);
            explosion.transform.position = position;
        }
    }
}
