using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EZEffects;

public class GunControl : MonoBehaviour
{
    [SerializeField]
    public AudioClip clip;
    [SerializeField]
    public AudioSource audioSource;

    //[SerializeField] public GameObject controllerRight;
    [SerializeField]
    public EffectTracer TracerEffect;
    [SerializeField]
    public Transform gunBarrel;

    public float fireRate = 0.5f;
    private float nextFire = 0.0f;

    private Transform enemy;
    private string enemyName;
    private GameObject npcHealthContainer;
    private Slider npcHealthSlider;

    public float hitCount = 0.2f;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get reference of audio source
        audioSource.clip = clip;                   // Loading audio source with audio clip
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            audioSource.Play();                   // Play sound
            Shoot();                         // Call raycast gun function
        }
    }

    private void Shoot()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(gunBarrel.position, gunBarrel.forward);
        TracerEffect.ShowTracerEffect(gunBarrel.position, gunBarrel.forward, 250f);
        Debug.Log("Shoot");

        if (Physics.Raycast(ray, out hit, 5000f))
        {
            Debug.Log("Loop 1");
            if (hit.collider.transform.CompareTag("Enemy"))
            {

                enemy = hit.collider.transform;
                enemyName = enemy.name;

                npcHealthSlider = enemy.Find("NPCHealthContainer/NPCHealthCanvas/NPCHeathUI/NPCHealthSlider").GetComponent<Slider>();

                npcHealthContainer = enemy.Find("NPCHealthContainer").gameObject; // get health ui

                npcHealthContainer.SetActive(true);         // make health appear

                Debug.Log("We've hit " + enemyName);

                npcHealthSlider.value -= hitCount;
            }
        }
    }
}

    