using UnityEngine;
using System.Collections;

namespace DigitalRuby.PyroParticles
{
    [System.Serializable]
    public struct RangeOfIntegers
    {
        public int Minimum;
        public int Maximum;
    }

    [System.Serializable]
    public struct RangeOfFloats
    {
        public float Minimum;
        public float Maximum;
    }

    public class FireBaseScript : MonoBehaviour
    {
        [Tooltip("Optional audio source to play once when the script starts.")]
        public AudioSource AudioSource;

        [Tooltip("How long the script takes to fully start. This is used to fade in animations and sounds, etc.")]
        public float StartTime = 1.0f;

        [Tooltip("How long the script takes to fully stop. This is used to fade out animations and sounds, etc.")]
        public float StopTime = 3.0f;

        [Tooltip("How long the effect lasts. Once the duration ends, the script lives for StopTime and then the object is destroyed.")]
        public float Duration = 2.0f;

        [Tooltip("How much force to create at the center (explosion), 0 for none.")]
        public float ForceAmount;

        [Tooltip("The radius of the force, 0 for none.")]
        public float ForceRadius;

        [Tooltip("A hint to users of the script that your object is a projectile and is meant to be shot out from a person or trap, etc.")]
        public bool IsProjectile;

        [Tooltip("Particle systems that must be manually started and will not be played on start.")]
        public ParticleSystem[] ManualParticleSystems;

        private float startTimeMultiplier;
        private float startTimeIncrement;

        private float stopTimeMultiplier;
        private float stopTimeIncrement;

        private AudioSource fireAudio;
        private int waterCount = 0;
        
        public bool Starting { get; private set; }

        public float StartPercent { get; private set; }

        public bool Stopping { get; private set; }

        public float StopPercent { get; private set; }

        private IEnumerator CleanupEverythingCoRoutine()
        {
            // 2 extra seconds just to make sure animation and graphics have finished ending
            yield return new WaitForSeconds(StopTime + 2.0f);

            GameObject.Destroy(gameObject);
        }

        protected virtual void Awake()
        {
            Starting = true;
        }

        protected virtual void Start()
        {
            if (AudioSource != null)
                AudioSource.Play();

            // precalculate so we can multiply instead of divide every frame
            stopTimeMultiplier = 1.0f / StopTime;
            startTimeMultiplier = 1.0f / StartTime;
            fireAudio = transform.Find("WallOfFireSound").GetComponent<AudioSource>();

            foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
                p.Play();
        }

        protected virtual void Update()
        {
            if (Stopping)
            {
                // increase the stop time
                stopTimeIncrement += Time.deltaTime;
                if (stopTimeIncrement < StopTime)
                {
                    StopPercent = stopTimeIncrement * stopTimeMultiplier;
                }
                fireAudio.volume = 1 - StopPercent;
            }
            else if (Starting)
            {
                // increase the start time
                startTimeIncrement += Time.deltaTime;
                if (startTimeIncrement < StartTime)
                {
                    StartPercent = startTimeIncrement * startTimeMultiplier;
                }
                else
                {
                    Starting = false;
                }
            }
        }

        public static void CreateExplosion(Vector3 pos, float radius, float force)
        {
            if (force <= 0.0f || radius <= 0.0f)
            {
                return;
            }

            // find all colliders and add explosive force
            Collider[] objects = UnityEngine.Physics.OverlapSphere(pos, radius);
            foreach (Collider h in objects)
            {
                Rigidbody r = h.GetComponent<Rigidbody>();
                if (r != null)
                {
                    r.AddExplosionForce(force, pos, radius);
                }
            }
        }

        public virtual void Stop()
        {
            if (Stopping)
            {
                return;
            }
            Stopping = true;

            // cleanup particle systems
            foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
            {
                p.Stop();
            }

            StartCoroutine(CleanupEverythingCoRoutine());
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Player")
            {
                GameObject.Find("_EventSystem").GetComponent<_gameSettings>().Load();
            }
        }

        void OnParticleCollision(GameObject other)
        {
            if (other.name == "Water")
            {
                waterCount++;
                if (waterCount >= 150)
                    Stop();
            }
        }
    }
}