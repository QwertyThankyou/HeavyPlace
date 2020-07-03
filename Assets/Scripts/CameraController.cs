using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Скорость стен")]
    public float speedWall = 1f;
    
    [Header("Номер стартовой комнаты")]
    public int room;                   // Номер текущей комнаты
    [Header("Колличество комнат на сцене")]
    public int countRoom;              // Колличество комнат на сцене
    
    private List<GameObject> _walls;   // Лист со стенами
    private List<GameObject> _rooms;   // Лист с комнатами
    
    private CameraOrbit _cam;          // Камера
    
    private float _startPos;           // Позиция начала касания
    private float _endPos;             // Позиция конца касания

    private int _angle = 1;            // Текущий угол камеры
    private int _bufAngle;             // Сохраняет прошлое значение _angle
    
    private bool _swapLim = true;      // Можно ли делать свайп
    private float _swapLimiter = 0f;   // Ограничивает "время" для следующего свайпа
    
    void Start()
    {
        _cam = GetComponent<CameraOrbit>();
        
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
        StartCoroutine(Changer( SwapW1(), SwapW2(), SwapWB()));

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
    
    private void SwitchRoom(int roomSwitch)           // Смена активной комнаты
    {
        foreach (GameObject i in _rooms)
        {
            if (i.name != "Room" + roomSwitch) i.SetActive(false);
            else i.SetActive(true);
        }

        foreach (GameObject wall in _walls)
        {
            for (int i = 1; i <= countRoom; i++)
            {
               if (i != roomSwitch) wall.transform.Find("room" + i).gameObject.SetActive(false);
               else wall.transform.Find("room" + i).gameObject.SetActive(true); 
            }
        }
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
                {
                    _cam.MoveHorizontal(true);
                    if (_angle != 4) _angle++;
                    else _angle = 1;
                }
                    
                else 
                { 
                    _cam.MoveHorizontal(false);
                    if (_angle != 1) _angle--;
                    else _angle = 4;
                }
            }
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

    IEnumerator Changer(Transform wallOne, Transform wallTwo, Transform wallBack)       // Меняет position и rotation у камеры и поднимает стены
    {
        wallOne.position = new Vector3(wallOne.position.x, Mathf.Lerp(wallOne.position.y, 15f, Time.deltaTime * speedWall), wallOne.position.z);
        wallTwo.position = new Vector3(wallTwo.position.x, Mathf.Lerp(wallTwo.position.y, 15f, Time.deltaTime * speedWall), wallTwo.position.z);
        
        wallBack.position = new Vector3(wallBack.position.x, Mathf.Lerp(wallBack.position.y, 2.629908f, Time.deltaTime * speedWall), wallBack.position.z);
        yield return null;
    }

    private void OnDisable()
    {
        _walls.Clear();
        _rooms.Clear();
    }
}