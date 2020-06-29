using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Скорость камеры и стен")]
    [Tooltip("Скорость камеры")]
    public float speedCam = 1f;
    [Tooltip("Скорость стен")]
    public float speedWall = 1f;
    
    private Transform wallOne;         // Стены
    private Transform wallTwo;
    private Transform wallThree;
    private Transform wallFour;
    
    private Transform _cam;            // Камера
    private float _startPos;           // Позиция начала касания
    private float _endPos;             // Позиция конца касания

    private int _angle = 1;            // Текущий угол камеры
    private int _bufAngle;             // Сохраняет прошлое значение _angle
    
    private bool _swapLim = true;      // Можно ли делать свайп
    private float _swapLimiter = 0f;   // Ограничивает "время" для следующего свайпа

    private int _room;                 // Номер текущей комнаты

    private Vector3 _angle1T;          // T - transform у камеры
    private Vector3 _angle1R;          // R- rotation у камеры
    private Vector3 _angle2T;
    private Vector3 _angle2R;
    private Vector3 _angle3T;
    private Vector3 _angle3R;
    private Vector3 _angle4T;
    private Vector3 _angle4R;

    void Start()
    {
        _cam = GetComponent<Transform>();
        CameraSet();
        WallSet();

        _angle = 3;
        _bufAngle = 3;
        
        //GameObject.Find("Wall1").transform.Find("Room2").gameObject.SetActive(false);   // Отключение стен комнаты
    }
    
    void Update()
    {
        SwapA();
        StartCoroutine(Changer(SwapP(), SwapR(), SwapW1(), SwapW2()));
        StartCoroutine(WallBack(SwapWB()));

        if (_swapLim == false) _swapLimiter += 0.15f;    // Можно изменять 0.2f и 10f, можно подобрать идеальные значения, пока так
        if (_swapLimiter > 10f)
        {
            _swapLim = true;
            _swapLimiter = 0f;
        }
    } 

    private void WallSet()                      // Находит на сцене стены ("Wall" + N) и присваивает их transform переменным
    {
        wallOne = GameObject.Find("Wall" + 1).transform;
        wallTwo = GameObject.Find("Wall" + 2).transform;
        wallThree = GameObject.Find("Wall" + 3).transform;
        wallFour = GameObject.Find("Wall" + 4).transform;
        // искать стены в дочерних объектах room/wall
    }

    private void WallOff()                      // Может в будущем прикостылю
    {
        wallOne.gameObject.SetActive(false);
        wallTwo.gameObject.SetActive(false);
        wallThree.gameObject.SetActive(false);
        wallFour.gameObject.SetActive(false);
    }

    private void CameraSet()                    // Присваиваем переменным заранее выбранное положение в пространстве (по углам комнаты) 
    {
        _angle1T = new Vector3(5f, 6f, -6f);
        _angle1R = new Vector3(30f, -45f, 0f);

        _angle2T = new Vector3(-5f, 6f, -6f);
        _angle2R = new Vector3(30f, 45f, 0f);

        _angle3T = new Vector3(-5f, 6f, 6f);
        _angle3R = new Vector3(30f, 135f, 0f);

        _angle4T = new Vector3(5f, 6f, 6f);
        _angle4R = new Vector3(30f, -135f, 0f);
    }

    private void SwapA()                                                     // Смена angle при свапе
    {
        if (Input.GetMouseButtonDown(0)) _startPos = Input.mousePosition.x;
        else if (Input.GetMouseButtonUp(0) && _swapLim)
        {
            _swapLim = false;
            
            _endPos = Input.mousePosition.x;

            if (Math.Abs(_startPos - _endPos) > 40)
            {
                _bufAngle = _angle;

                if (_startPos < _endPos) 
                    if (_angle != 4) _angle++;
                    else _angle = 1;
                else if (_angle != 1) _angle--;
                     else _angle = 4;
            }
        }
    }

    private Vector3 SwapP()                                                 // Смена position
    {
        switch (_angle)                                                       
        { 
            case 1: 
                return _angle1T;
            case 2: 
                return _angle2T;
            case 3: 
                return _angle3T;
            case 4: 
                return _angle4T;
            default:
                return _angle1T;
        }
    }
    
    private Vector3 SwapR()                                                 // Смена rotation
    {
        switch (_angle)                                                      
        { 
            case 1: 
                return _angle1R;
            case 2: 
                return _angle2R;
            case 3: 
                return _angle3R;
            case 4: 
                return _angle4R;
            default:
                return _angle1R;
        }
    }
    
    private Transform SwapW1()                                                 // Возвращает правую стену
    {
        switch (_angle)                                                      
        { 
            case 1: 
                return wallFour;
            case 2: 
                return wallOne;
            case 3: 
                return wallTwo;
            case 4: 
                return wallThree;
            default:
                return wallFour;
        }
    }
    
    private Transform SwapW2()                                                 // Возвращает левую стену
    {
        switch (_angle)                                                      
        { 
            case 1: 
                return wallOne;
            case 2: 
                return wallTwo;
            case 3:
                return wallThree;
            case 4: 
                return wallFour;
            default:
                return wallOne;
        }
    }
    
    private Transform SwapWB()                                                 // Возвращает стену, которую нужно опустить
    {
        switch (_angle)                                                      
        { 
            case 1:
                if (_bufAngle == 4) return wallThree;
                else return wallTwo;
            case 2:
                if (_bufAngle == 1) return wallFour;
                else return wallThree;
            case 3:
                if (_bufAngle == 2) return wallOne;
                else return wallFour;
            case 4:
                if (_bufAngle == 3) return wallTwo;
                else return wallOne;
            default:
                return wallThree;
        }
    }

    IEnumerator Changer(Vector3 position, Vector3 rotation, Transform wallOne, Transform wallTwo)       // Меняет position и rotation у камеры и поднимает стены
    {
        _cam.position = new Vector3(
            Mathf.Lerp(_cam.position.x, position.x, Time.deltaTime * speedCam),
            _cam.position.y,
            Mathf.Lerp(_cam.position.z, position.z, Time.deltaTime * speedCam));
        _cam.rotation = Quaternion.Lerp(_cam.rotation, Quaternion.Euler(rotation), Time.deltaTime * speedCam);
        wallOne.position = new Vector3(wallOne.position.x, Mathf.Lerp(wallOne.position.y, 15f, Time.deltaTime * speedWall), wallOne.position.z);
        wallTwo.position = new Vector3(wallTwo.position.x, Mathf.Lerp(wallTwo.position.y, 15f, Time.deltaTime * speedWall), wallTwo.position.z);
        yield return null;
    }

    IEnumerator WallBack(Transform wall)            // Возвращает стену на своё место
    {
        wall.position = new Vector3(wall.position.x, Mathf.Lerp(wall.position.y, 2.629908f, Time.deltaTime * speedWall), wall.position.z);
        yield return null;
    }
}