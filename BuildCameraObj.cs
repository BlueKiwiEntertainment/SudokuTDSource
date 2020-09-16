using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCameraObj : MonoBehaviour
{

    Vector3 InitialPosition;
    Quaternion InitialRotation;
    public float PanDuration;
    public GameObject cameraObj;
    bool CameraActive = true;
    public int CameraSpeed;
    float xModifier;
    float yModifier;
    public float MinX;
    public float MaxX;
    public float MinZ;
    public float MaxZ;

    public void Awake()
    {
        InitialPosition = this.gameObject.transform.position;
        InitialRotation = this.gameObject.transform.rotation;

    }


    private void Update()
    {
        if (CameraActive)

        {
            CameraController();
        }
    }

    public void PanOut(Vector3 position, Quaternion rotation)
    {
        CameraActive = true;
        cameraObj.SetActive(true);
        this.gameObject.transform.position = position;
        this.gameObject.transform.rotation = rotation;
        StartCoroutine(Transition());
        
    }
    IEnumerator Transition()
    {
        float t = 0.0f;
        Vector3 startingPos = this.gameObject.transform.position;
        Quaternion startingRot = this.gameObject.transform.rotation;
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale / PanDuration);


            transform.position = Vector3.Lerp(startingPos, InitialPosition, t);
            transform.rotation = Quaternion.Lerp(startingRot, InitialRotation, t);
            yield return 0;
          
        }
        if (t >= 0)
        {
            if (GameManager.instance.GameState == GameManager.GameStates.RunningAround)
            {
                GameManager.instance.SelectingSudoku();
            }
        }

    }

    public void PanIn(Vector3 position, Quaternion rotation)
    {
        CameraActive = false;
        GameManager.instance.RunningStarter();
        InitialPosition = this.gameObject.transform.position;
        InitialRotation = this.gameObject.transform.rotation;
        StartCoroutine(TransitionIn(position, rotation));
        
    }

    IEnumerator TransitionIn(Vector3 finalPosition, Quaternion finalRotation)
    {
        Debug.Log("CoroutineStart");
        float t = 0.0f;
        Vector3 startingPos = this.gameObject.transform.position;
        Quaternion startingRot = this.gameObject.transform.rotation;
        while (t < 1.0f)
        {
            
            t += Time.deltaTime * (Time.timeScale / PanDuration);


            transform.position = Vector3.Lerp(startingPos, finalPosition, t);
            transform.rotation = Quaternion.Lerp(startingRot, finalRotation, t);
            yield return 0;
        }
        if (t >= 1.0f)
        {
            GameManager.instance.RunningSudoku();
            cameraObj.SetActive(false);
            
        }
          
        
        
    }
    void CameraController()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            xModifier += Input.GetAxis("Horizontal") * CameraSpeed;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            yModifier += Input.GetAxis("Vertical") * CameraSpeed;
        }

        float x = Mathf.Clamp((this.gameObject.transform.position.x + xModifier), MinX, MaxX);
        xModifier = 0;
        float y = this.gameObject.transform.position.y;

        float z = Mathf.Clamp((this.gameObject.transform.position.z + yModifier), MinZ, MaxZ);
        yModifier = 0;

        this.gameObject.transform.position = new Vector3(x, y, z);
    }

}


