using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiskFlicker : MonoBehaviour
{
    public enum DiskStates {idle, warmup, moving, flicked};
    Rigidbody rb;
    Camera mainCamera;
    GameManager gameManager;
    DiskStates diskState = DiskStates.idle;
    Image powerBar;

    bool flicking = false;
    bool flicked = false;
    bool canFlip = false;

    float cushion = .2f;
    float power = 0;
    float maxPower = 75f;
    float minPower = 10f;
    float powerFactor = 30f;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        Canvas MainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        powerBar = MainCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        powerBar.transform.localScale = new Vector3(minPower / maxPower, 1, 1);
    }

    private void Start()
    {        

    }

    void Update()
    {
        if (gameManager && gameManager.GetCanFlick())
        {
            if (diskState == DiskStates.idle && Input.GetKey(KeyCode.Mouse0))
            {
                diskState = DiskStates.warmup;
            }
            if (diskState == DiskStates.warmup && Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (power > minPower)
                {
                    if (power > maxPower)
                    {
                        power = maxPower;
                    }
                    diskState = DiskStates.moving;
                    FlickDisk();
                    StartCoroutine(CountdownBarrier());
                }
                else
                {
                    diskState = DiskStates.idle;
                    power = 0;
                }
            }
            if (diskState == DiskStates.warmup)
            {
                power += Time.deltaTime * powerFactor;
                if (power > minPower || power < maxPower)
                {
                    powerBar.transform.localScale = new Vector3(power / maxPower, 1, 1);
                }
            }
        }
        if (transform.position.magnitude > 50)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (diskState != DiskStates.flicked && canFlip && diskState == DiskStates.moving && rb.velocity.magnitude < .1f)
        {
            rb.velocity = Vector3.zero;
            diskState = DiskStates.flicked;
        }
    }

    void FlickDisk()
    {
        Vector3 direction = Vector3.Normalize(new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z) - transform.position);        
        rb.AddForce(direction * -Mathf.Pow(power, 1.8f));
    }

    public DiskStates GetDiskState()
    {
        return diskState;
    }

    IEnumerator CountdownBarrier()
    {
        yield return new WaitForSeconds(cushion);
        canFlip = true;
    }
}
