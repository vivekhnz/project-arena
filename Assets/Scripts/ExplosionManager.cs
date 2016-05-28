using UnityEngine;
using System.Collections;

public class ExplosionManager : MonoBehaviour
{
    public ParticleSystem EnemyExplosion;
    public ParticleSystem BulletExplosion;

    void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}

    public void CreateEnemyExplosion(Vector3 position, float angle)
    {
        CreateExplosion(EnemyExplosion, position, angle);
    }

    public void CreateBulletExplosion(Vector3 position, float angle)
    {
        CreateExplosion(BulletExplosion, position, angle);
    }

    private void CreateExplosion(ParticleSystem explosionPrefab, Vector3 position, float angle)
    {
        if (explosionPrefab != null)
        {
            var explosion = Instantiate(explosionPrefab);
            explosion.transform.position = position;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            var velocity = explosion.velocityOverLifetime;
            velocity.enabled = true;
            velocity.x = new ParticleSystem.MinMaxCurve(direction.x * 15.0f);
            velocity.y = new ParticleSystem.MinMaxCurve(direction.y * 15.0f);
        }
    }
}
