using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject redToken;
    [SerializeField] GameObject blackToken;

    CameraRotator cameraRotator;
    GameObject currentToken;
    DiskFlicker diskFlicker;

    Vector3[] tokenLocations = { new Vector3(0, -.11f, -15.5f), new Vector3(-15.5f, -.11f, 0), new Vector3(0, -.11f, 15.5f), new Vector3(15.5f, -.11f, 0) };
    Vector3[] cameraLocations = { new Vector3(0, 12f, -40.5f), new Vector3(-40.5f, 12f, 0), new Vector3(0, 12f, 40.5f), new Vector3(40.5f, 12f, 0)};

    int currentTurn = 1;

    bool canFlick = true;

    private void Awake()
    {
        cameraRotator = Camera.main.GetComponent<CameraRotator>();
    }

    void Start()
    {        
        SpawnToken(redToken);
    }

    void Update()
    {
        if (diskFlicker != null)
        {
            if (diskFlicker.GetDiskState() == DiskFlicker.DiskStates.flicked)
            {
                currentToken = null;
                diskFlicker = null;
                if (currentTurn < 9)
                {
                    if (currentTurn % 2 == 1)
                    {
                        SpawnToken(redToken);
                    }
                    else
                    {
                        SpawnToken(blackToken);
                    }
                }
            }
        }
    }

    void SpawnToken(GameObject colorToken)
    {
        currentToken = null;
        currentToken = Instantiate(colorToken, tokenLocations[currentTurn%4], Quaternion.Euler(-90, 0, 0), null);
        print("Token at " + tokenLocations[currentTurn % 4]);
        diskFlicker = currentToken.GetComponent<DiskFlicker>();
        cameraRotator.ChangeFocus(currentToken, cameraLocations[currentTurn%4], currentTurn%4);
        currentTurn++;
    }

    public bool GetCanFlick()
    {
        return canFlick;
    }

    public void SetCanFlick(bool canFlickIn)
    {
        canFlick = canFlickIn;
    }
}
