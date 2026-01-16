using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main : MonoBehaviour
{
    public GameObject refToBread1, refToBread2, refToRat, refToRatPic, refToCup,  refToToesterSwitch,refToPlug, refToPlugSwitch,refToToesterTimer,refToToesterUI,refToToesterRail;
    //camera gameObjects
    public GameObject toesterSlot1_1, toesterSlot2_1, toesterSlot1_2, toesterSlot2_2,toesterSlot1_0,toesterSlot2_0, refToCamera_RatScatch, refToCamera_Fuse,refToMainCamera, refToUI,refToCamera_Cup,refToCamera_Rat,refToRatOnSandwich, camera_warning;
    public GameObject refToCanvas;
    public GameObject refToToesterLight;

    public Material breadMaterial; 
    public Material ratMaterial;

    private Renderer bread1Renderer;
    private Renderer bread2Renderer;
    private Renderer ratRenderer;


    public Material PointShader;

    //toester timer
    public int timer = 0;
    public float floatTimer = 0;
    public bool isTiming = false;
    int toesterTimer = 90;


    bool bread1Spot1 = false;
    bool bread2Spot1 = false;
    bool bread12Spot1 = false;
    bool bread12Spot2 = false;
    bool ratSpot1 = false;
    bool ratSpot2 = false;
    bool fuseClicked = false;
    bool cameraClosed = false;
    bool fuseClosin = false;
    bool ratOccupied = false;
    bool breadOccupied = false;
    bool bakedAndOnToester = false;
    public bool breadBaked = false;
    public bool ratBaked = false;
    bool breadReady = false;
    bool ratReady = false;
    public bool ratSandwichDone = false;
    bool warningRead = false;

    private GameObject lastHoveredObject = null;
private Material lastOriginalMaterial = null;


    public ParticleSystem smoke;

    public GameObject Canvas;
    public GameObject refToPrecaution;
    void Start()
    {
        Screen.SetResolution(1280, 720, false); // 16:9 해상도, 창모드
        Application.targetFrameRate = 60;
        refToCanvas.SetActive(false);
        refToToesterLight.SetActive(false);

        if (refToBread1 != null)
            bread1Renderer = refToBread1.GetComponent<Renderer>();
        if (refToBread2 != null)
            bread2Renderer = refToBread2.GetComponent<Renderer>();

        if (refToRat != null)
            ratRenderer = refToRat.GetComponent<Renderer>();

        
        Camera.main.transform.position = camera_warning.transform.position;

        if (smoke.isPlaying)
        {
            smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    void Update()
    {
        toester_Timer();       
        HandleInput();
        toesterUI();
        Warning();
        HandleHover();

        if(Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
          

    }

    void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            refToPrecaution.SetActive(false);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                ratInToester(clickedObject);
                breadInToester(clickedObject);
                fuseSwitch(clickedObject);
                closeUP(clickedObject);
                ratSandwich(clickedObject);
            }
        }
    }
    void ratInToester(GameObject clickedObject)
    {
        //쥐가 토스터 상단에 위치
        if (clickedObject == refToRat && !breadOccupied&&!ratBaked&&!isTiming)
        {
            rat_UP();
            ratSpot1 = true;
            ratOccupied = true;
        }
        //쥐가 토스터 하단에 위치
        if (ratSpot1 && clickedObject==refToToesterSwitch && fuseClicked && !breadOccupied&&!ratBaked)
        {
            ratOccupied = true;
            refToRat.transform.position = new Vector3(-0.89f, 1.8f, 1.48f);
            refToRat.transform.rotation = Quaternion.Euler(-23.21f, 118.512f, -4.358f);
            ratSpot1 = false;
            ratSpot2 = true;

            //타이머 작동
            toester_Timer();
            isTiming = true;

            //쥐가 튀어나오는건 타이머에서 구현함      
        }
        if (ratBaked && clickedObject == refToRat)
        {
            bakedAndOnToester = false;
            ratOccupied = false;
            rat_GROUND();
            ratReady = true;    
        }
    }
    void rat_UP()
    {
        refToRat.transform.position = new Vector3(-0.74f, 2.54f, 1.35f);
        refToRat.transform.rotation = Quaternion.Euler(-23.21f, 118.512f, -4.358f);
    }
    void rat_GROUND()
    {
        refToRat.transform.position = new Vector3(0.45f, 0.28f, -2.86f);
        refToRat.transform.rotation = Quaternion.Euler(2.347f, 87.139f, -89.74f);
    }

    void breadInToester(GameObject clickedObject)
    {
        if(clickedObject==refToBread1&& !ratOccupied&&!breadBaked&&!isTiming)
        {
            refToBread1.transform.position = toesterSlot1_1.transform.position;
            refToBread1.transform.rotation = toesterSlot1_1.transform.rotation;
            bread1Spot1 = true;
            breadOccupied = true;
        }
        if (!ratOccupied && clickedObject == refToBread2&&!breadBaked && !isTiming)
        {
            refToBread2.transform.position = toesterSlot2_1.transform.position;
            refToBread2.transform.rotation = toesterSlot2_1.transform.rotation;
            bread2Spot1 = true;
            breadOccupied = true;
        }

        if(bread1Spot1&&bread2Spot1)
        {
            bread12Spot1 = true;
        }
        if (bread12Spot1 && clickedObject == refToToesterSwitch&&!breadBaked && !isTiming&&fuseClicked)
        {
            breadOccupied = true;
            refToBread1.transform.position = toesterSlot1_2.transform.position;
            refToBread1.transform.rotation = toesterSlot1_2.transform.rotation;
            refToBread2.transform.position = toesterSlot2_2.transform.position;
            refToBread2.transform.rotation = toesterSlot2_2.transform.rotation;
            bread12Spot1 = false;
            bread12Spot2 = true;


            //타이머 작동
            toester_Timer();
            isTiming = true;
        }

            //빵이 튀어나오는건 타이머에서 구현함

            if(breadBaked&&(clickedObject==refToBread1||clickedObject==refToBread2)&&!ratSandwichDone)
            {
                bakedAndOnToester = false;
                breadOccupied = false;
                bread_PLATE();
                breadReady = true;
            }
        
    }
    void bread_UP()
    {
        refToBread1.transform.position = toesterSlot1_1.transform.position;
        refToBread1.transform.rotation = toesterSlot1_1.transform.rotation;
        refToBread2.transform.position = toesterSlot2_1.transform.position;
        refToBread2.transform.rotation = toesterSlot2_1.transform.rotation;
    }
    void bread_PLATE()
    {
        refToBread1.transform.position = toesterSlot1_0.transform.position;
        refToBread1.transform.rotation = toesterSlot1_0.transform.rotation;
        refToBread2.transform.position = toesterSlot2_0.transform.position;
        refToBread2.transform.rotation = toesterSlot2_0.transform.rotation;
    }




    void fuseSwitch(GameObject clickedObject)
    {
        //************plug*************
        if (!fuseClicked)
        {
            if (!cameraClosed && (clickedObject == refToPlug || clickedObject == refToPlugSwitch))
            {
                Camera.main.transform.position = refToCamera_Fuse.transform.position;
                Camera.main.transform.rotation = refToCamera_Fuse.transform.rotation;

                cameraClosed = true;
                fuseClicked = false;
                fuseClosin = true;
            }
            else if (cameraClosed && fuseClosin && clickedObject == refToPlugSwitch)
            {
                fuseClicked = true;
            }
            if (fuseClicked == true)
            {
                refToPlugSwitch.transform.rotation = Quaternion.Euler(19, 90, -90);
            }
            else if (fuseClicked == false)
            {
                refToPlugSwitch.transform.rotation = Quaternion.Euler(-17, 90, -90);
            }
        }
        if (cameraClosed == true)
        {
            refToCanvas.SetActive(true);
        }
        if (cameraClosed == false)
        {
            refToCanvas.SetActive(false);

            Camera.main.transform.position = refToMainCamera.transform.position;
            Camera.main.transform.rotation = refToMainCamera.transform.rotation;
        }

    }
    void closeUP(GameObject clickedObject)
    {
        // RAT PIC
        

        if (clickedObject == refToRatPic)
        {
            cameraClosed = true;
            Camera.main.transform.position = refToCamera_RatScatch.transform.position;
            Camera.main.transform.rotation = refToCamera_RatScatch.transform.rotation;
        }

        //UAL CUP

        if (clickedObject == refToCup)
        {
            cameraClosed = true;
            Camera.main.transform.position = refToCamera_Cup.transform.position;
            Camera.main.transform.rotation = refToCamera_Cup.transform.rotation;
        }

        //UI
        if (clickedObject == refToUI)
        {
            Camera.main.transform.position = refToMainCamera.transform.position;
            cameraClosed = false;
        }


        if (cameraClosed == true)
        {
            refToCanvas.SetActive(true);
        }
        if (cameraClosed == false)
        {
            refToCanvas.SetActive(false);

            Camera.main.transform.position = new Vector3(7.03f, 4.2f, -0.2f);
            Camera.main.transform.rotation = Quaternion.Euler(383.8f, -90, 0.819f);
            //Vector3(7.03100204,4.20599937,-0.208000064)
            //383.864//-90//0.819
        }
    }

    //*****클릭 안들어가는 함수*****
    void toester_Timer()
    {
        if (isTiming)
        {
            // 연기 ON
            if (!smoke.isPlaying)
            {
                smoke.Play();
            }

            refToToesterSwitch.transform.position = new Vector3(0.99f, 1.27f, 0.7f);
            refToToesterTimer.transform.rotation = Quaternion.Euler(-1.19f, 121f, toesterTimer);
            toesterTimer += 2;
            refToToesterLight.SetActive(true);
            refToToesterRail.transform.position = new Vector3(-0.548f, 0.86f, 1.28f);

            floatTimer += Time.deltaTime;
            if (floatTimer >= 1f)
            {
                timer++;
                floatTimer = 0;
            }

            if (timer >= 5)
            {
                // 연기 "자연스럽게 사라지도록"
                if (smoke.isPlaying)
                {
                    smoke.Stop(false); // <- 여기 핵심!
                }

                isTiming = false;
                refToToesterSwitch.transform.position = new Vector3(0.99f, 1.7f, 0.72f);
                timer = 0;
                refToToesterLight.SetActive(false);
                refToToesterRail.transform.position = new Vector3(-0.548f, 1.97f, 1.28f);

                if (ratSpot2)
                {
                    rat_UP();
                    ratSpot2 = false;
                    ratSpot1 = true;
                    ratBaked = true;
                    bakedAndOnToester = true;
                    ratRenderer.material = ratMaterial;
                }

                if (bread12Spot2)
                {
                    bread_UP();
                    bread12Spot2 = false;
                    bread12Spot1 = true;
                    breadBaked = true;
                    bakedAndOnToester = true;
                    bread1Renderer.material = breadMaterial;
                    bread2Renderer.material = breadMaterial;
                }
            }
        }
    }

    void toesterUI()
    {
        Renderer renderer = refToToesterUI.GetComponent<Renderer>();

        if (fuseClicked&&!isTiming )
        {
            if (renderer != null)
            {
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.color = new Color(195f, 10f, 0f); //초록색
            }
        }

        else if (isTiming)
        {
            if (renderer != null)
            {
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.color = new Color(195f, 0f, 0f);//빨간색
            }
        }

        if (!isTiming&&bakedAndOnToester)
        {
            if (renderer != null)
            {
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.color = new Color(0f, 195f, 0f);//초록색
            }
        }

        
    }

    void ratSandwich(GameObject clickedObject)
    {
        if (ratReady && breadReady)
        {
            
            if (clickedObject == refToRat)
            {
                ratSandwichDone = true;
                Canvas.SetActive(true);

                refToRat.transform.position = new Vector3(2.74f, 0.63f, -0.2f);
                refToRat.transform.rotation = Quaternion.Euler(1.298f, 137.5f, -88.027f);

                refToBread1.transform.position = new Vector3(2.84f, 0.93f, -0.078f);
                refToBread1.transform.rotation = Quaternion.Euler(271.627f, 37.214f, 78.655f);
            }
        }
    }

    void Warning()
    {
        if(Input.GetMouseButtonDown(0)&&!warningRead)
        {
            warningRead = true;
            camera_warning.SetActive(false);
            Camera.main.transform.position = new Vector3(7.03f, 4.2f, -0.2f);
            Camera.main.transform.rotation = Quaternion.Euler(383.8f, -90, 0.819f);
        }
    }


    void HandleHover()
    {
        Vector3 defaultCamPos = new Vector3(7.03f, 4.2f, -0.2f);
        float tolerance = 0.05f; // 오차 허용 범위

        // 카메라가 기본 위치가 아닐 경우 하이라이트 금지
        if (Vector3.Distance(Camera.main.transform.position, defaultCamPos) > tolerance)
        {
            ResetLastHovered();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            if (hoveredObject == refToBread1 || hoveredObject == refToBread2 || hoveredObject == refToRat ||
                hoveredObject == refToCup || hoveredObject == refToToesterSwitch || hoveredObject == refToPlug ||
                hoveredObject == refToPlugSwitch || hoveredObject == refToRatPic || hoveredObject == refToUI)
            {
                if (lastHoveredObject != hoveredObject)
                {
                    ResetLastHovered();

                    Renderer renderer = hoveredObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        lastHoveredObject = hoveredObject;
                        lastOriginalMaterial = renderer.material;
                        renderer.material = PointShader;
                    }
                }
            }
            else
            {
                ResetLastHovered();
            }
        }
        else
        {
            ResetLastHovered();
        }
    }


    void ResetLastHovered()
    {
        if (lastHoveredObject != null && lastOriginalMaterial != null)
        {
            Renderer renderer = lastHoveredObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = lastOriginalMaterial;
            }
            lastHoveredObject = null;
            lastOriginalMaterial = null;
        }
    }
}