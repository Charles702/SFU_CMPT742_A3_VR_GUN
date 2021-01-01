using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Gun : MonoBehaviour {
    public GameObject notification;
    public RawImage gameInfoImg; 
    public RawImage loseImage;
    public GameObject shootSound;
    public GameObject bulletHole;
    public GameObject MuzzleFlash;
    public GameObject end, start; // The gun start and end point
    public GameObject gun;
    public Animator animator;
    
    public GameObject spine;
    public GameObject handMag;
    public GameObject gunMag;

    float gunShotTime = 0.1f;
    float gunReloadTime = 1.0f;
    Quaternion previousRotation;
    public float health = 100;
    public bool isDead;
 

    public Text magBullets;
    public Text remainingBullets;
    public Text playerHealth;

    public Text en_reamin;

    int magBulletsVal = 30;
    int remainingBulletsVal = 90;
    int magSize = 30;
    public GameObject headMesh;
    public static bool leftHanded { get; private set; }
    private GameObject emeny;

    public Camera fixCamera;
    public Camera playerCamera;
    private float em_damage = 20;
    public int room_id = 1;

    public GameObject[] enemies;

    // Use this for initialization
    void Start() {
        headMesh.GetComponent<SkinnedMeshRenderer>().enabled = false; // Hiding player character head to avoid bugs :)
        //emeny = GameObject.Find("Emeny");
        //hid image
        gameInfoImg.enabled = false;
        loseImage.enabled = false;
        Invoke("hidNotification",2f);
    }

    private void hidNotification()
    {
        notification.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        //Vector3 emeny_pos = emeny.transform.position;
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - emeny_pos), Time.deltaTime);
        //animator.SetBool("run", true);
        // distance 10m.   magnitude of vector transform.position - emeny_pos
        
        // Cool down times
        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }
        if (gunReloadTime >= 0.0f)
        {
            gunReloadTime -= Time.deltaTime;
        }


        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && gunShotTime <= 0 && gunReloadTime <= 0.0f && magBulletsVal > 0 && !isDead)
        { 
            shotDetection(); // Should be completed

            addEffects(); // Should be completed

            animator.SetBool("fire", true);
            gunShotTime = 0.5f;
            
            // Instantiating the muzzle prefab and shot sound
            
            magBulletsVal = magBulletsVal - 1;
            if (magBulletsVal <= 0 && remainingBulletsVal > 0)
            {
                animator.SetBool("reloadAfterFire", true);
                gunReloadTime = 2.5f;
                Invoke("reloaded", 2.5f);
            }
        }
        else
        {
            animator.SetBool("fire", false);
        }

        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.R)) && gunReloadTime <= 0.0f && gunShotTime <= 0.1f && remainingBulletsVal > 0 && magBulletsVal < magSize && !isDead )
        {
            animator.SetBool("reload", true);
            gunReloadTime = 2.5f;
            Invoke("reloaded", 2.0f);
        }
        else
        {
            animator.SetBool("reload", false);
        }
        updateText();

        //when enemies are all dead, game over;
        int dead_number = 0;
        print("dead_number====================="+dead_number);
        foreach(GameObject en in enemies)
        {
            dead_number += en.GetComponent<EmenyGun>().en_dead? 1:0;
            print("dead_number===="+dead_number);
            //update death of enemy
            en_reamin.text = (enemies.Length - dead_number).ToString();
        }    
        if( dead_number == 4)
        {
            print("All enemies died");
            win_reset();
        }
       
    }

  

    public void Being_shot(float damage) // getting hit from enemy
    {
        //lose health
        if(health > 0)
        {
            health -= damage;
            //update player health panel
            playerHealth.text = health.ToString();
            print("current health:" + health);
        }
        else  //die
        {
            print("pleyer die");
            isDead = true;
            animator.SetBool("dead",true);
            // switch cameara
            playerCamera.enabled = false;
            headMesh.GetComponent<SkinnedMeshRenderer>().enabled = true; 
            lose_reset();
        }
    }

    public void ReloadEvent(int eventNumber) // appearing and disappearing the handMag and gunMag
    {
        if (eventNumber == 1)
        {
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
        if (eventNumber == 2)
        {
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
    }

    void reloaded()
    {
        int newMagBulletsVal = Mathf.Min(remainingBulletsVal + magBulletsVal, magSize);
        int addedBullets = newMagBulletsVal - magBulletsVal;
        magBulletsVal = newMagBulletsVal;
        remainingBulletsVal = Mathf.Max(0, remainingBulletsVal - addedBullets);
        animator.SetBool("reloadAfterFire", false);
    }

    void updateText()
    {
        magBullets.text = magBulletsVal.ToString() ;
        remainingBullets.text = remainingBulletsVal.ToString();
    }

    void shotDetection() // Detecting the object which player shot 
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit rayHit;
        if(Physics.Raycast(end.transform.position, (end.transform.position- start.transform.position).normalized, out rayHit, 100.0f))
        {
            print(rayHit.collider.tag);
            if(rayHit.transform.tag == "enemy")
            {
                print("tag is emeny");
               // rayHit.transform.GetComponent<>().
               rayHit.transform.GetComponent<EmenyGun>().Being_shot(em_damage);
            }
            else
            {
                GameObject BulletHoleObject = Instantiate(bulletHole, rayHit.point+ rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
            }
        }

        //Muzzle Flash
        GameObject flash = Instantiate(MuzzleFlash, end.transform.position, end.transform.rotation);
        flash.GetComponent<ParticleSystem>().Play();
        Destroy(flash, 0.1f);

        //shot sound
        Destroy((GameObject) Instantiate(shootSound, transform.position, transform.rotation), 2.0f);

    }

    void addEffects() // Adding muzzle flash, shoot sound and bullet hole on the wall
    {

    }

    public void reStartGame()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void win_reset()
    {
        print("=======win========");
        notification.gameObject.SetActive(true);
        gameInfoImg.enabled= true;
        notification.GetComponentInChildren<UnityEngine.UI.Text>().text = "Congratulations! You win! \n Game will restart in 10 seconds";
        Invoke("reStartGame",10f);       
    }

    private void lose_reset()
    {
        print("=======lost========");
        notification.gameObject.SetActive(true);
        loseImage.enabled = true;
        notification.GetComponentInChildren<UnityEngine.UI.Text>().text = "Sorry You lose! \n Game will restart in 10 seconds";
        Invoke("reStartGame",10f);           
    }

}
