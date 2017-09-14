using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class NPCController : MonoBehaviour {

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip[] npcAudio;

    // Objects
    public Transform player;
    public Transform head;
    private GameObject sword; // Declare sword collider variable
	Animator anim;            // Declare Animator Variable

    // Nav Mesh
    //private NavMeshAgent NavComponent;

    // State Control:
    string state = "patrol";
    public GameObject[] waypoints;
    int currentWP = 0;
    public float rotSpeed = 0.2f;
    public float speed = 1.5f;
    public float detectionDistance = 25f;
    public float viewRange = 60f;
    float accuracyWP = 5.0f;        // So the NPC doesn't float around looking for an exact spot

    //Anger Values:
    public float anger = 0.0f;
    public float frustrationRate = 1f;
    public float frustrationLimit = 3f;
    public float runSpeed = 4f;

    // NPC Voice Delays:
    public float speechRate = 8f;
    public float walkingRate = 4f;
    public float runningRate = 3f;
    private float nextSpeech = 0f;
    public float attackRate = 3f;

    // NPC idle delay:
    public float idleTime = 30f;
    private float goTime = 0f;

    // Health bar Slider:
    private GameObject healthContainer;
    public Slider healthSlider;
    public int maxHealth;

    // Use this for initialization
    void Start () 
	{
		anim = GetComponent<Animator>(); // Attach Animator Variable to the Animator Component in Unity

        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        //NavComponent = this.gameObject.GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () 
	{
        StayOnGround();
        
        if (state == "dead")
        {
            return;
        }

        healthContainer = transform.Find("NPCHealthContainer").gameObject;
        sword = transform.Find("Bip001/Bip001 Prop1/Sphere").gameObject;    // Store sword collider
        Vector3 direction = player.position - this.transform.position;      // Work out direction from player to npc
        direction.y = 0;                                                    // Dont let the NPC rotate around the Y axis and 'tip over'  
        float angle = Vector3.Angle(direction,head.up);                     // Get angle value within the direction of the HEAD not whole body (UP because Y axis is set to front of head)

        if (state == "patrol" && waypoints.Length > 0)
        {     
            resetAnimationStates();

            if (Vector3.Distance(waypoints[currentWP].transform.position, transform.position) < accuracyWP) // test distance between NPC and waypoint
            {
                //Random Patrol:
                currentWP = Random.Range(0, waypoints.Length);
                //NavComponent.SetDestination(waypoints[currentWP].transform.position);
                goTime = Time.time + idleTime;
                
                // make idle for a bit:
                if (Time.time < goTime)
                {
                    anim.SetBool("isIdle", true);
                    anim.SetBool("isWalking", false);
                
                }

                // Structured Patrol:
                //currentWP++; // Send to next waypoint
                //if(currentWP >= waypoints.Length)
                //{
                //    currentWP = 0;
                //}
            }

            // continue walking:
            if (Time.time >= goTime)
            {
                anim.SetBool("isIdle", false);
                anim.SetBool("isWalking", true);

                // rotate towards waypoint:
                direction = waypoints[currentWP].transform.position - transform.position;
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,               // Rotate towards direction
                                        Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
                this.transform.Translate(0, 0, Time.deltaTime * speed);
            }

        }

        if (Vector3.Distance(player.position, this.transform.position) < detectionDistance && (angle < viewRange || state == "pursue")) // Check distance between character and npc and they will only move if they can see the player within the angle defined
		{
            state = "pursue";                                            // Once the NPC has seen the player set pursing to true 

            resetAnimationStates();

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,               // Rotate towards direction
                                        Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
			
			if((direction.magnitude > 2) && (anger < frustrationLimit))                    // While more than 5 away... and under frustration limit
			{
                if (Time.time > nextSpeech)
                {
                    nextSpeech = Time.time + walkingRate;
                    PlayClipAt(npcAudio[1], this.transform.position);                        // Play sound
                    anger = anger + frustrationRate;                                         // increase frustration
                }

                this.transform.Translate(0,0, Time.deltaTime * speed);                                          // ..Move npc towards the player
				anim.SetBool("isWalking",true);                                               // Set walking animation
				anim.SetBool("isAttacking",false);                                            // Don't attack
                anim.SetBool("isRunning", false);                                            // Don't attack
     
            } else if ((direction.magnitude) > 2 && (anger >= frustrationLimit))           // While more than 5 away... and over frustration limit.
            {
                if (Time.time > nextSpeech)
                {
                    nextSpeech = Time.time + runningRate;
                    PlayClipAt(npcAudio[3], this.transform.position);                  // Play sound
                }
                anim.SetBool("isRunning", true);                                               // Set walking animation
                anim.SetBool("isAttacking", false);                                            // Don't attack
                anim.SetBool("isWalking", false);                                            // Don't attack
                this.transform.Translate(0, 0, Time.deltaTime * runSpeed);                                          // ..Move npc towards the player


            } else if(direction.magnitude <= 2)                                                  // Very close then attack..

            {
                if (Time.time > nextSpeech)
                {
                    nextSpeech = Time.time + attackRate;
                    PlayClipAt(npcAudio[2], this.transform.position);                  // Play sound
                }
                anim.SetBool("isAttacking",true);                                             // Attack
				anim.SetBool("isWalking",false);                                              // No longer walk
                anim.SetBool("isRunning", false);                                            // Don't run
            }

		}
		else 
		{
            state = "patrol";                                                                  // Once the NPC is out of range set patrol to true

            if (Time.time > nextSpeech)
            {
                nextSpeech = Time.time + speechRate;
                Debug.Log("Play");
                PlayClipAt(npcAudio[0], this.transform.position);                  // Play sound
            }
            
        }

    if (healthSlider.value <= 0)
    {
        Debug.Log("Death");
        NPCDeath();
        PlayClipAt(npcAudio[4], this.transform.position);                  // Play death sound

     }
}
    public void NPCDeath()
    {
        resetAnimationStates();
        anim.SetBool("isDead", true);
        state = "dead";
        Destroy(healthContainer);       // Destroy sword collider so it no longer shows above the npcs dead body
        Destroy(sword);                 // Destroy sword collider so it no longer takes health off the player on impact
    }

    public void resetAnimationStates()
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isRunning", false);                                          
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        GameObject tempGO = new GameObject("TempAudio");          // create the temp object
        tempGO.transform.position = pos;                          // set its position
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip;                                      // define the clip
        aSource.spatialBlend = 1.0f;                               // set to 3D sound
        aSource.minDistance = 0.2f;
        aSource.maxDistance = 0.3f;
        aSource.Play();                                           // start the sound
        Destroy(tempGO, clip.length);                             // destroy object after clip duration
        return aSource;                                           // return the AudioSource reference
    }

    void StayOnGround()
    {
        Ray downwardsRay = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(downwardsRay, out hit, 1.0f))
        {

            //You could check hit.collider.transform/tag to make sure
            //you're only trying to stand on the terrain
            if (hit.point.y > 0)
            {
                Vector3 currentPosition = transform.position;
                currentPosition.y = hit.point.y;
                transform.position = currentPosition;
            }
        } else
        {
            Ray upwardsRay = new Ray(transform.position, Vector3.up);
            RaycastHit upHit;
            if (Physics.Raycast(upwardsRay, out upHit, 1.0f))
            {
                Vector3 currentPosition2 = transform.position;
                currentPosition2.y = upHit.point.y;
                transform.position = currentPosition2;
            }   
        }
    }
}
