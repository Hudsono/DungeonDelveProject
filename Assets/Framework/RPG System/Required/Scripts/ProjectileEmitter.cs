using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEmitter : MonoBehaviour
{
    [Tooltip("How these projectiles deal damage")]
    public DamageType damageType;

    [Tooltip("Do these projectiles explode upon impact with the environment? Requires the assigned DamageType to have AoE configured.")]
    public bool explosive = false;

    public ParticleSystem pas;
    public List<ParticleCollisionEvent> collisionEvents;
    ParticleSystem.Particle[] m_particles;

    int numParticles = 0;

    // Start is called before the first frame update
    void Start()
    {
        pas = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        Init();

        if (pas != null)
            numParticles = pas.GetParticles(m_particles);

    }

    void Init()
    {
        if (pas == null)
            pas = GetComponent<ParticleSystem>();

        if (m_particles == null || m_particles.Length < pas.main.maxParticles)
            m_particles = new ParticleSystem.Particle[pas.main.maxParticles];
    }

    private void OnParticleCollision(GameObject other)
    {
        Vector3? sourcePos = null;

        // If not explosive, the projectile needs to collide with an RPGEntity in order to deal damage.
        if (!explosive)
        {
            RPGEntity otherEntity = other.GetComponent<RPGEntity>();
            if (otherEntity == null)
                return;

            int numCollEvents = pas.GetCollisionEvents(other, collisionEvents);

            ParticleSystem.Particle collidedParticle = new ParticleSystem.Particle();
            bool foundParticle = false;
            float colRadius = pas.collision.radiusScale;
            float marginError = 0.01f;
            for (int i = 0; i < numCollEvents; i++)
            {
                Vector3 intersection = collisionEvents[i].intersection;

                for (int j = 0; j < m_particles.Length; j++)
                {
                    float dist = Vector3.Distance(intersection, m_particles[j].position);
                    if (dist <= colRadius + marginError)
                    {
                        //Debug.Log(dist);
                        Debug.DrawLine(intersection, m_particles[j].position, Color.red, 5);
                        m_particles[j].startColor = Color.red;
                        m_particles[j].remainingLifetime = 0;
                        //pas.SetParticles(m_particles);
                        sourcePos = m_particles[j].position;
                        //ApplyParticle(m_particles[j]);
                        //Debug.DrawRay(m_particles[j].position, m_particles[j].velocity, Color.red);
                        //break;
                    }
                }
            }

        }
        damageType.DealDamage(other.gameObject, sourcePos);
    }
}
