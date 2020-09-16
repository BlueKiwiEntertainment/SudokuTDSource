using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public GameObject cameraObj;
    public Rigidbody rb;
    public float playerSpeed;
    public float mouseY;
    public float mouseXClamped;
    public float mouseX;
    public float mouseSensitivity = 100f;
    public float upDownCam;
    public float evaporating = 0.1f;
    public float InitializeDelay;
    public bool isEvaporating = false;
    public float ExplosionRadius = 30f;
    public float ExplosionForce = 200f;
    public float ExplosionDmg = 20f;
    float DashCooldown = 0.5f;
    float JumpCooldown = 0.5f;
    public float CurrentVelocity;
    public GameObject DeathCamera;

    public GameObject ModelParent;



    public GameObject spellObj;


    private void Awake()
    {
        GameManager.instance.cameraOffsetObj.GetComponent<BuildCameraObj>().PanIn(cameraObj.transform.position, cameraObj.transform.rotation);
        GameManager.instance.newCamera = cameraObj;
        cameraObj.SetActive(false);
    }


    private void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();

    }


    public void Update()
    {



        if (InitializeDelay < 0)
        {
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

            upDownCam -= mouseY;
            upDownCam = Mathf.Clamp(upDownCam, -90, 90);
            Vector3 ass = new Vector3(upDownCam, 0, 0);

            cameraObj.transform.localRotation = Quaternion.Euler(ass);

            this.gameObject.transform.Rotate(Vector3.up * mouseX);




            if (Input.GetAxis("Fire1") != 0)
            {
                Sudoku();
            }

            Evaporating();
        }
        else
        {
            InitializeDelay -= Time.deltaTime;
        }



        CurrentVelocity = rb.velocity.magnitude;
        CurrentVelocity = Mathf.Clamp(2 * CurrentVelocity, 0, 90);
        Vector3 thisone = new Vector3(CurrentVelocity, 0, 0);
        ModelParent.transform.localRotation = Quaternion.Euler(thisone);
    }


    private void FixedUpdate()
    {
        if (!isEvaporating)
        {
            //Moving
            if (InitializeDelay < 0)
            {

                float moveHorizontal = Input.GetAxis("Horizontal");
                float moveVertical = Input.GetAxis("Vertical");

                Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
                Vector3 directional = this.gameObject.transform.rotation * movement;


                rb.AddForce(directional * playerSpeed * 2);

                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    if (!FindObjectOfType<AudioManager>().IsPlaying("Walking"))
                    {
                        FindObjectOfType<AudioManager>().Play("Walking");
                    }
                }
                else
                {
                    FindObjectOfType<AudioManager>().StopPlay("Walking");
                }
                
            }

            //Dash
            if (InitializeDelay < 0)
            {
                if (Input.GetAxis("Fire3") != 0 && DashCooldown < 0)
                {
                    Vector3 lookdirection = cameraObj.transform.forward;


                    rb.AddForce(lookdirection * 70 * playerSpeed);

                    DashCooldown = 0.5f;

                    GameManager.instance.HonourHandler(50);


                    FindObjectOfType<AudioManager>().Play("Sweep2");
                }
                else
                {
                    DashCooldown -= Time.deltaTime;
                }


            }
            //Jump
            if (InitializeDelay < 0)
            {
                if (Input.GetAxis("Jump") != 0 && JumpCooldown < 0)
                {
                    rb.AddForce(Vector3.up * playerSpeed * 30);
                    JumpCooldown = 0.5f;

                    GameManager.instance.HonourHandler(10);

                    FindObjectOfType<AudioManager>().Play("Jump");
                }
                else
                {
                    JumpCooldown -= Time.deltaTime;
                }
            }

            //ExtraGravity
            if (InitializeDelay < 0)
            {
                rb.AddForce(Vector3.down * 400);
            }
        }else
        {
            ModelParent.SetActive(false);
        }
    }


    public void Sudoku()
    {
        if (!isEvaporating)
        {
            GameObject boom = Instantiate(spellObj, this.gameObject.transform.position, this.gameObject.transform.rotation);
            isEvaporating = true;
            FindObjectOfType<AudioManager>().StopPlay("Walking");
            float scale = Mathf.Clamp((CurrentVelocity * 3 / 90), 1, 3);
            boom.transform.localScale = new Vector3(scale, scale, scale);
            Destroy(boom, 15);

            Collider[] CollidersInRadius = Physics.OverlapSphere(this.transform.position, ExplosionRadius * scale);
            foreach (Collider collider in CollidersInRadius)
            {
                Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(ExplosionForce, this.transform.position, ExplosionRadius * scale);
                }
                EnemyBruteScript sc = collider.gameObject.GetComponent<EnemyBruteScript>();
                if (sc != null)
                {
                    sc.DamageHandler(ExplosionDmg);
                }




            }
            PanOut();
        }


    }

    public void Evaporating()
    {
        if (isEvaporating && evaporating > 0)
        {
            if (Input.GetAxis("Jump") != 0)
            {
                GameManager.instance.cameraOffsetObj.GetComponent<BuildCameraObj>().PanOut(cameraObj.transform.position, cameraObj.transform.rotation);
                Destroy(this.gameObject);
            }
            evaporating -= Time.deltaTime;
        }
        else if (evaporating < 0) 
        {

            GameManager.instance.cameraOffsetObj.GetComponent<BuildCameraObj>().PanOut(cameraObj.transform.position, cameraObj.transform.rotation);
            Destroy(this.gameObject);
        }

        if (GameManager.instance.GameState == GameManager.GameStates.Building)
        {
            GameManager.instance.cameraOffsetObj.GetComponent<BuildCameraObj>().PanOut(cameraObj.transform.position, cameraObj.transform.rotation);
            Destroy(this.gameObject);
        }

    }
    void PanOut()
    {


        StartCoroutine(Transition());

    }
    IEnumerator Transition()
    {
        float t = 0.0f;
        Vector3 startingPos = cameraObj.transform.position;
        Quaternion startingRot = cameraObj.transform.rotation;
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale / 1f);


            cameraObj.transform.position = Vector3.Lerp(startingPos, DeathCamera.transform.position, t);
            //cameraObj.transform.rotation = Quaternion.Lerp(startingRot, DeathCamera.transform.rotation, t);
            yield return 0;

        }



    }
}

