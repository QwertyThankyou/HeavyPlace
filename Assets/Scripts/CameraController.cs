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
    
    [Header("Номер стартовой комнаты")]
    public int room;                   // Номер текущей комнаты
    [Header("Колличество комнат на сцене")]
    public int countRoom;              // Колличество комнат на сцене
    
    private List<GameObject> _walls;   // Лист со стенами
    private List<GameObject> _rooms;   // Лист с комнатами
    
    private Transform _cam;            // Камера
    private float _startPos;           // Позиция начала касания
    private float _endPos;             // Позиция конца касания

    private int _angle = 1;            // Текущий угол камеры
    private int _bufAngle;             // Сохраняет прошлое значение _angle
    
    private bool _swapLim = true;      // Можно ли делать свайп
    private float _swapLimiter = 0f;   // Ограничивает "время" для следующего свайпа
    
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
        
        _walls = new List<GameObject>();
        _rooms = new List<GameObject>();
        
        _angle = 3;               // Поставил троечки, чтобы не менять знаки в CameraSet
        _bufAngle = 3;
        
        ListFill();
        
        SwitchRoom(room);
    }
    
    void Update()
    {
        SwapA();
        StartCoroutine(Changer(SwapP(), SwapR(), SwapW1(), SwapW2()));
        StartCoroutine(WallBack(SwapWB()));

        if (_swapLim == false) _swapLimiter += 0.2f;    // Можно изменять 0.2f и 10f, можно подобрать идеальные значения, пока так
        if (_swapLimiter > 10f)
        {
            _swapLim = true;
            _swapLimiter = 0f;
        }
    }

    public void Button1()
    {
        SwitchRoom(1);
    }
    
    public void Button2()
    {
        SwitchRoom(2);
    }
    
    public void Button3()
    {
        SwitchRoom(3);
    }

    private void ListFill()                     // Заполнение листов  
    {
        for (int i = 1; i <= countRoom; i++)
        {
            _rooms.Add(GameObject.Find("Room" + i));
        }

        for (int i = 1; i <= 4; i++)
        {
            _walls.Add(GameObject.Find("Wall" + i));
        }
    }
    
    private void SwitchRoom(int room)           // Смена активной комнаты
    {
        foreach (GameObject i in _rooms)
        {
            if (i.name != "Room" + room) i.SetActive(false);
            else i.SetActive(true);
        }

        foreach (GameObject wall in _walls)
        {
            for (int i = 1; i <= countRoom; i++)
            {
               if (i != room) wall.transform.Find("room" + i).gameObject.SetActive(false);
               else wall.transform.Find("room" + i).gameObject.SetActive(true); 
            }
        }
    }
    
    private void CameraSet()                    // Присваиваем переменным заранее выбранное положение в пространстве (по углам комнаты) 
    {
        _angle1T = new Vector3(6f, 6f, -5f);
        _angle1R = new Vector3(30f, -45f, 0f);

        _angle2T = new Vector3(-6f, 6f, -5f);
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
                return _walls[3].transform;
            case 2: 
                return _walls[0].transform;
            case 3: 
                return _walls[1].transform;
            case 4: 
                return _walls[2].transform;
            default:
                return _walls[2].transform;
        }
    }
    
    private Transform SwapW2()                                                 // Возвращает левую стену
    {
        switch (_angle)                                                      
        { 
            case 1: 
                return _walls[0].transform;
            case 2: 
                return _walls[1].transform;
            case 3:
                return _walls[2].transform;
            case 4: 
                return _walls[3].transform;
            default:
                return _walls[0].transform;
        }
    }
    
    private Transform SwapWB()                                                 // Возвращает стену, которую нужно опустить
    {
        switch (_angle)                                                      
        { 
            case 1:
                if (_bufAngle == 4) return _walls[2].transform;
                else return _walls[1].transform;
            case 2:
                if (_bufAngle == 1) return _walls[3].transform;
                else return _walls[2].transform;
            case 3:
                if (_bufAngle == 2) return _walls[0].transform;
                else return _walls[3].transform;
            case 4:
                if (_bufAngle == 3) return _walls[1].transform;
                else return _walls[0].transform;
            default:
                return _walls[2].transform;
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