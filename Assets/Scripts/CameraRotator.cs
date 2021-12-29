using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] GameObject originalStartingPoint;

    float timeToTake = 2f;
    float angle;
    float distance = 25f;
    float startMousePosX;

    GameObject focusPoint;
    DiskFlicker diskFlicker;
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Start()
    {        
        startMousePosX = Input.mousePosition.x;
        if (focusPoint == null)
        {
            focusPoint = originalStartingPoint;
        }
        angle = Mathf.PI / 2;
    }

    void Update()
    {
        float currentMousePos = Input.mousePosition.x;
        if (gameManager.GetCanFlick() && diskFlicker != null && diskFlicker.GetDiskState() != DiskFlicker.DiskStates.moving)
        {
            gameObject.transform.LookAt(focusPoint.transform.position);
            if (currentMousePos < startMousePosX - 100)
            {
                angle -= Mathf.Pow(Mathf.Abs(currentMousePos - startMousePosX + 100) * .0001f, 2);
            }
            if (currentMousePos > startMousePosX + 100)
            {
                angle += Mathf.Pow(Mathf.Abs(currentMousePos - startMousePosX - 100) * .0001f, 2);
            }
            float xPos = focusPoint.transform.position.x - distance * Mathf.Sin(angle);
            float ypos = transform.position.y;
            float zPos = focusPoint.transform.position.z - distance * Mathf.Cos(angle);
            transform.position = new Vector3(xPos, ypos, zPos);
            gameObject.transform.LookAt(focusPoint.transform.position);
            
        }
    }

    public void ChangeFocus(GameObject newFocusPoint, Vector3 cameraLocation, float angle)
    {
        diskFlicker = null;
        focusPoint = newFocusPoint;
        diskFlicker = newFocusPoint.GetComponent<DiskFlicker>();
        if (Vector3.Distance(cameraLocation, transform.position) > 25)
        {
            StartCoroutine(RotateCamera(cameraLocation, angle));
        }      
    }

    IEnumerator RotateCamera(Vector3 cameraLocation, float angle)
    {
        gameManager.SetCanFlick(false);
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(focusPoint.transform.position - transform.position, Vector3.up);
        float timeTaken = 0;
        while (2 * timeTaken < timeToTake)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, 2 * Mathf.Sin(Mathf.PI / 2 * timeTaken / timeToTake));
            timeTaken += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = endRotation;
        yield return new WaitForEndOfFrame();
        StartCoroutine(MoveCamera(cameraLocation, angle));
        
    }

    IEnumerator MoveCamera(Vector3 cameraLocation, float angle)
    {
        gameManager.SetCanFlick(false);
        this.angle = Mathf.PI * angle / 2;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = cameraLocation;
        float timeTaken = 0;
        while (timeTaken < timeToTake)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.Sin(Mathf.PI / 2 * timeTaken / timeToTake));
            gameObject.transform.LookAt(focusPoint.transform.position);
            timeTaken += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = cameraLocation;
        yield return new WaitForEndOfFrame();

        gameManager.SetCanFlick(true);
    }
}
