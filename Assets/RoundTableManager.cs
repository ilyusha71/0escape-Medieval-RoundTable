/**************************************************************************************** 
 * Wakaka Studio 2017
 * Author: iLYuSha Dawa-mumu Wakaka Kocmocovich Kocmocki KocmocA
 * Project:: 0escape Medieval - Round Table
 * Tools: Unity 5/C# + Arduino/C++
 * Last Updated: 2017/10/21
 ****************************************************************************************/
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public enum Knight
{
    Arthur = 0,
    Lancelot = 1,
    Ector = 2,
    Galahad = 3,
    Tristan = 4,
}
public class RoundTableManager : MonoBehaviour
{
    [Header("Basic")]
    public GameObject[] knights;
    public Transform[] cylinders;
    public Transform[] nodes;
    private GameObject[] seats = new GameObject[5];
    [Header("Testing")]
    public GameObject Observatory;
    public Text rotText;
    public Text recordtText;
    private float startTime;
    private float recordTime;
    int resetTimes;
    [Header("Standard")]
    public GameObject pentagram;
    public Transform star;
    public GameObject table;
    public Transform goal;
    private bool ready;
    private bool roll;
    private int laserPoint;





    void Awake()
    {
        if (ArduinoController.instance == null)
        {
            PlayerPrefs.SetInt("lastScene", SceneManager.GetActiveScene().buildIndex + 100);
            SceneManager.LoadScene("Arduino Controller");
        }

        // 定義圓桌騎士的初始位置
        seats[(int)Knight.Arthur] = null;
        seats[(int)Knight.Lancelot] = knights[(int)Knight.Tristan];
        seats[(int)Knight.Ector] = knights[(int)Knight.Galahad];
        seats[(int)Knight.Galahad] = knights[(int)Knight.Arthur];
        seats[(int)Knight.Tristan] = knights[(int)Knight.Lancelot];

        pentagram.SetActive(false);
        ready = true;

        recordTime = PlayerPrefs.GetInt("recordTime");
        if (recordTime == 0)
            recordTime = 3600;
        recordtText.text = "Record: " + recordTime;

    }

    IEnumerator End()
    {
        DOTween.To(x => pentagram.GetComponent<CanvasGroup>().alpha = x, 1, 0, 3);
        yield return new WaitForSeconds(3.0f);
        star.gameObject.SetActive(false);
        table.SetActive(true);
        table.transform.DOMove(goal.transform.position, 2.97f);
        yield return new WaitForSeconds(3.0f);
        DOTween.To(x => table.GetComponent<CanvasGroup>().alpha = x, 1, 0, 2);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Keypad7))
            resetTimes++;
        if (Input.GetKeyUp(KeyCode.Keypad7))
            resetTimes = 0;
        if (resetTimes > 150)
        {
            if (ArduinoController.instance.connectAruidnoCompleted)
                ArduinoController.arduinoSerialPort.Write("R");
            ArduinoController.instance.msgBox.AddNewMsg("遊戲重新啟動");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(End());
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Observatory.SetActive(ArduinoController.instance.panelSetting.activeSelf);
        }
        //ArduinoController.instance.msgBox.AddNewMsg("dasdasd");

        rotText.text = string.Format("{0:F2}", pentagram.transform.localEulerAngles.z);
        if (Input.GetKey(KeyCode.J))
            pentagram.transform.rotation *= Quaternion.Euler(0, 0, 0.5f);
        if (Input.GetKey(KeyCode.K))
            pentagram.transform.rotation *= Quaternion.Euler(0, 0, -0.5f);
        if (Input.GetKey(KeyCode.R))
            pentagram.SetActive(true);

        if (!roll)
        {
            // 激活
            if (ArduinoController.command == "Active" || Input.GetKey(KeyCode.L))
                laserPoint++;

            if (laserPoint > 30 && laserPoint < 10000)
            {
                if (startTime == 0)
                {
                    startTime = Time.time;
                    if (ArduinoController.instance.connectAruidnoCompleted)
                        ArduinoController.arduinoSerialPort.Write("A");
                    ArduinoController.instance.msgBox.AddNewMsg("雷射陣開啟");
                }
                if (Time.time - startTime > 7.0f)
                {
                    roll = true;
                    startTime = Time.time;
                    if (ArduinoController.instance.connectAruidnoCompleted)
                        ArduinoController.arduinoSerialPort.Write("S");
                    ArduinoController.instance.msgBox.AddNewMsg("遊戲開始");
                    pentagram.SetActive(true);
                    laserPoint += 100000;
                }
            }
        }
        else
        {
            star.rotation = Quaternion.Euler(0, 0, Time.time * 60 * 1);
            // 結束
            if (seats[(int)Knight.Arthur] == knights[(int)Knight.Arthur] &&
                seats[(int)Knight.Lancelot] == knights[(int)Knight.Lancelot] &&
                seats[(int)Knight.Galahad] == knights[(int)Knight.Galahad] &&
                seats[(int)Knight.Tristan] == knights[(int)Knight.Tristan])
            {
                if (ArduinoController.instance.connectAruidnoCompleted)
                    ArduinoController.arduinoSerialPort.WriteLine("E");
                ArduinoController.instance.msgBox.AddNewMsg("遊戲結束");
                float endTime = Time.time - startTime;
                if (endTime < recordTime)
                    PlayerPrefs.SetInt("recordTime", (int)endTime);
                roll = false;
                StartCoroutine(End());
            }

            /* 單點觸發轉動 */
            Rotate();

            /* 多點觸發移動 */
            if (ready)
            {
                Move();
            }
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    void Rotate()
    {
        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1) ||
            ArduinoController.command == "ARTHUR")
            cylinders[(int)Knight.Arthur].rotation *= Quaternion.Euler(0, 0, -37 * Time.deltaTime);

        if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2) ||
            ArduinoController.command == "LANCELOT")
            cylinders[(int)Knight.Lancelot].rotation *= Quaternion.Euler(0, 0, 37 * Time.deltaTime);

        if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3) ||
            ArduinoController.command == "ECTOR")
            cylinders[(int)Knight.Ector].rotation *= Quaternion.Euler(0, 0, -37 * Time.deltaTime);

        if (Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Keypad4) ||
            ArduinoController.command == "GALAHAD")
            cylinders[(int)Knight.Galahad].rotation *= Quaternion.Euler(0, 0, -37 * Time.deltaTime);

        if (Input.GetKey(KeyCode.Alpha5) || Input.GetKey(KeyCode.Keypad5) ||
            ArduinoController.command == "TRISTAN")
            cylinders[(int)Knight.Tristan].rotation *= Quaternion.Euler(0, 0, 37 * Time.deltaTime);
    }
    void Move()
    {
        /* 鄰邊移動 */
        if (Input.GetKey(KeyCode.Keypad1) && Input.GetKey(KeyCode.Keypad2) || ArduinoController.command == "1")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur] };
            MovingCheck(0, 1, knightsPermitMove);
        }
        else if (Input.GetKey(KeyCode.Keypad2) && Input.GetKey(KeyCode.Keypad3) || ArduinoController.command == "2")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur], knights[(int)Knight.Lancelot], knights[(int)Knight.Tristan] };
            MovingCheck(1, 2, knightsPermitMove);
        }
        else if (Input.GetKey(KeyCode.Keypad3) && Input.GetKey(KeyCode.Keypad4) || ArduinoController.command == "3")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur], knights[(int)Knight.Lancelot], knights[(int)Knight.Tristan] };
            MovingCheck(2, 3, knightsPermitMove);
        }
        else if (Input.GetKey(KeyCode.Keypad4) && Input.GetKey(KeyCode.Keypad5) || ArduinoController.command == "4")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur], knights[(int)Knight.Lancelot], knights[(int)Knight.Tristan] };
            MovingCheck(3, 4, knightsPermitMove);
        }
        else if (Input.GetKey(KeyCode.Keypad5) && Input.GetKey(KeyCode.Keypad1) || ArduinoController.command == "5")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur] };
            MovingCheck(4, 0, knightsPermitMove);
        }
        /* 對角移動 */
        else if (Input.GetKey(KeyCode.Keypad1) && Input.GetKey(KeyCode.Keypad3) || ArduinoController.command == "6")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur] };
            MovingCheck(0, 2, knightsPermitMove);
        }
        else if (Input.GetKey(KeyCode.Keypad3) && Input.GetKey(KeyCode.Keypad5) || ArduinoController.command == "7")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur], knights[(int)Knight.Lancelot], knights[(int)Knight.Galahad] };
            MovingCheck(2, 4, knightsPermitMove);
        }
        else if (Input.GetKey(KeyCode.Keypad5) && Input.GetKey(KeyCode.Keypad2) || ArduinoController.command == "8")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur], knights[(int)Knight.Lancelot], knights[(int)Knight.Galahad] };
            MovingCheck(4, 1, knightsPermitMove);
        }
        else if (Input.GetKey(KeyCode.Keypad2) && Input.GetKey(KeyCode.Keypad4) || ArduinoController.command == "9")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur], knights[(int)Knight.Lancelot], knights[(int)Knight.Galahad] };
            MovingCheck(1, 3, knightsPermitMove);
        }
        else if (Input.GetKey(KeyCode.Keypad4) && Input.GetKey(KeyCode.Keypad1) || ArduinoController.command == "10")
        {
            GameObject[] knightsPermitMove = { knights[(int)Knight.Arthur] };
            MovingCheck(3, 0, knightsPermitMove);
        }
    }
    void MovingCheck(int node1, int node2, GameObject[] knightsPermitMove)
    {
        if (seats[node1] == null)
        {
            for (int i = 0; i < knightsPermitMove.Length; i++)
            {
                if (seats[node2] == knightsPermitMove[i])
                {
                    StartCoroutine(MoveKnight(seats[node2], node2, node1));
                }
            }
        }
        else if (seats[node2] == null)
        {
            for (int i = 0; i < knightsPermitMove.Length; i++)
            {
                if (seats[node1] == knightsPermitMove[i])
                {
                    StartCoroutine(MoveKnight(seats[node1], node1, node2));
                }
            }
        }
    }
    IEnumerator MoveKnight(GameObject knight, int from, int to)
    {
        ready = false;
        roll = false;

        Vector3 local = knight.transform.localPosition;
        knight.SetActive(false);
        knight.transform.SetParent(nodes[to].transform);
        knight.transform.localPosition = local;
        knight.transform.localRotation = Quaternion.identity;
        seats[from] = null;

        star.DOKill();
        star.position = nodes[from].position;
        star.rotation = nodes[from].rotation;
        star.DOScale(0, 0.73f).SetEase(Ease.InExpo);
        star.DOMove(nodes[to].position, 0.73f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.73f);
        seats[to] = knight;
        knight.SetActive(true);
        StartCoroutine(Buffer());
    }
    IEnumerator Buffer()
    {
        star.localScale = Vector3.zero;
        star.position = nodes[5].position;
        roll = true;
        star.DOScale(1, 1.0f);
        yield return new WaitForSeconds(1.0f);
        ready = true;
    }


}
