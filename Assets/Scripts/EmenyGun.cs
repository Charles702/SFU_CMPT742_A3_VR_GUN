using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmenyGun : MonoBehaviour
{
    public int room_id;
    public GameObject em_shootSound;
    public GameObject em_MuzzleFlash;
    
    float gunShotTime = 0.3f;
    public GameObject em_end, em_start; // The gun start and end point
    public GameObject en_gun;
    public Animator animator;
    private GameObject player;
    // Start is called before the first frame update
    //private Animator animator;
    GameObject walkPath;

    public GameObject[] targets;
    public bool en_dead = false;

    private float error;
    int state = 1;
    private float player_damage = 20.0f;
    private float em_health = 80f;
    private Vector3 curTargetPos; 

    private int i_target = 0;   //initialize the target of walk path
    private float viewOfFiled = 0.1f; //-1: opposite to player, 0: perpendicular to player, 1: same direction to player.
    public GameObject floor;
    private bool Isbeingshot = false;
    private Gun playerSc;
    void Start()
    {
        //Invoke("reloading", 3f);
        player = GameObject.Find("player");
        playerSc = player.GetComponent<Gun>();
        //walkPath = GameObject.Find("Walkpath1");
        // foreach(Transform child in walkPath.transform)
        // {
        //     targets.Add(walkPath.transform.position);
        // }
        // set current target position
        curTargetPos = targets[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //IsSameRoom();
        //print("is there wall? ==="+IsWallbetweenP());

        //when enmey detects player or is being shot, then turns to player and shoot.
        if( (InPlayerInView() || Isbeingshot) && !en_dead && room_id == playerSc.room_id )    
        {
            //distance between player and enemy
            float dis_toPlayer = Vector3.Distance(player.transform.position,transform.position);
            // enmey turn to player
            transform.rotation =  Quaternion.LookRotation(player.transform.position - transform.position );

            //when detect player run until 10 m distance
            if (dis_toPlayer > 10 )
            {

                print("+++++ run +++++");
                animator.SetBool("emeny_run",true);
                //animator.SetBool("stop2run",true);
                
            }
            else
            {
                print("---- stop + shoot-----");
                animator.SetBool("emeny_run",false);
                if (gunShotTime >= 0.0f)
                {
                    gunShotTime -= Time.deltaTime;
                }                
                if(gunShotTime<=0 && playerSc.isDead == false)
                {
                    print("shot frequence : +++" );
                    animator.SetTrigger("emeny_fire");
                    shotDetection();
                    gunShotTime = 0.3f;
                }
                //animator.SetBool("emeny_fire",true);
            }

            print("I see you");
            //shoot player
        }
        else  // player is out of view of filed, anemy keeps walking around towards targets.
        {
            print("I lost you");
            Vector3 target_pos = targets[0].transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(curTargetPos - transform.position), Time.deltaTime);

            //animator.SetBool("run", true);
            float distance_tar = Vector3.Distance(transform.position,curTargetPos);
            if (distance_tar < 1.0f)
            {
                //turn to next target
                i_target +=1;
                if(i_target == targets.Length)
                    i_target = 0;
                curTargetPos = targets[i_target].transform.position;
            }   
        }


    }

    void reloading()
    {
        animator.SetTrigger("emeny_fire");
   
        //transform.rotation =  Quaternion.LookRotation(targets[1].transform.position - transform.position);
    }

    void shotDetection() // Detecting the object which player shot 
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        //randomize shooting vector
        RaycastHit rayHit;
        if(Physics.Raycast(em_end.transform.position, (em_end.transform.position- em_start.transform.position + new Vector3(0,0, Random.RandomRange(0,5))).normalized, out rayHit, 100.0f))
        {
            print(rayHit.collider.tag);
            //print("emeney hit -----"+rayHit.transform.tag);
            if(rayHit.transform.tag == "Player")
            {
                //player lose health;
                print("I hit player, yaya !!! ");
                player.GetComponent<Gun>().Being_shot(player_damage);
                // rayHit.transform.GetComponent<>().
            }
            // else
            // {
            //     GameObject BulletHoleObject = Instantiate(bulletHole, rayHit.point+ rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
            // }
        }

        //Muzzle Flash
        GameObject flash = Instantiate(em_MuzzleFlash, em_end.transform.position, em_end.transform.rotation);
        flash.GetComponent<ParticleSystem>().Play();
        Destroy(flash, 0.1f);

        //shot sound
        Destroy((GameObject) Instantiate(em_shootSound, transform.position, transform.rotation), 2.0f);

    }

    public void Being_shot(float damage) // getting hit from player
    {
        //lose health
        if(em_health > 0)
        {
            em_health -= damage;
            //update player health panel
            //playerHealth.text = health.ToString();
            print("emeny current health:" + em_health);
            Isbeingshot = true;
        }
        else  //die
        {
            print("emeny die");
            en_dead = true;
            //trigger die animation
            animator.SetTrigger("enemy_die");
            setEnGunEffect();
        }
    }

    void setEnGunEffect()
    {
        //print("en_gun--parent before"+en_gun.transform.parent.name);
        en_gun.transform.SetParent(null);
        print("en_gun--parent after"+en_gun.transform.parent);
        CapsuleCollider col_gun = en_gun.AddComponent<CapsuleCollider>();
        col_gun.direction = 2;
        col_gun.radius = 0.4f;
        col_gun.height = 0.8f;
        Rigidbody  rigidGun =  en_gun.AddComponent<Rigidbody>();
        rigidGun.mass = 2;
        rigidGun.useGravity = true;
    }

    private bool InPlayerInView()
    {
        // detect player 
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toPlayer = (player.transform.position - transform.position).normalized ;

        if(Vector3.Dot(forward, toPlayer) > viewOfFiled)
            return true;
        else
            return false;
    }
}
