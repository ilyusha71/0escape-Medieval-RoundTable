/**************************************************************************************** 
 * Wakaka Studio 2017
 * Author: iLYuSha Dawa-mumu Wakaka Kocmocovich Kocmocki KocmocA
 * Project: 0escape Medieval - Arduino Controller
 * Version: Arduino Module v1.200a
 * Tools: Unity 5/C# + Arduino/C++
 * Last Updated: 2017/07/29
 ****************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using System.Threading;

public class ArduinoController : MonoBehaviour
{
    public MessageBox msgBox;
    public static ArduinoController instance;
    public static SerialPort arduinoSerialPort;
    public static string command;
    public static bool readline;
    private Thread myThread;
    private bool connectAruidnoCompleted;
    private bool cancel;

    /* Arduino Setting */
    bool initializeCheck;
    public GameObject panelSetting;

    public GameObject groupPort;
    Toggle[] port;
    string serialPort = "COM9";
    public GameObject groupBaud;
    Toggle[] baud;
    int serialBaud = 9600;
    int[] valueBaud = new int[] {9600, 2400, 4800, 9600, 19200, 38400, 57600 };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this);

        int sceneIndex = PlayerPrefs.GetInt("lastScene");
        if (sceneIndex>=100)
            SceneManager.LoadScene(PlayerPrefs.GetInt("lastScene")-100);

        int com = PlayerPrefs.GetInt("lastPort");
        int rate = PlayerPrefs.GetInt("lastBaud");
        if (com != 0)
            initializeCheck = true;

        port =  groupPort.GetComponentsInChildren<Toggle>();
        port[com].isOn = true;
        serialPort = "COM" + com;
        baud = groupBaud.GetComponentsInChildren<Toggle>();
        baud[rate].isOn = true;
        serialBaud = valueBaud[rate];
    }

    public void ChangePort(int com)
    {
        if (port[com].isOn)
        {
            serialPort = "COM" + com;
            PlayerPrefs.SetInt("lastPort", com);
            msgBox.AddNewMsg("已重設Serial Port為" + serialPort);
        }
    }

    public void ChangeBaud(int rate)
    {
        if (baud[rate].isOn)
        {
            serialBaud = valueBaud[rate];
            PlayerPrefs.SetInt("lastBaud", rate);
            msgBox.AddNewMsg("已重設Serial Baud為" + serialBaud);
        }
    }

    void Start()
    {
        panelSetting.SetActive(!initializeCheck);
        Cursor.visible = !initializeCheck;

        if (initializeCheck)
            ConnectArduino();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            panelSetting.SetActive(!panelSetting.activeSelf);
            Cursor.visible = !Cursor.visible;
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
            msgBox.AddNewMsg("已刪除設定檔");
        }
    }
    private void LateUpdate()
    {
        if (readline)
        {
            readline = false;
            msgBox.AddNewMsg(command);
            command = "";
        }
    }

    #region Arduino
    public void ConnectArduino()
    {
        arduinoSerialPort = new SerialPort(serialPort, serialBaud);
        try
        {
            arduinoSerialPort.Open();
            connectAruidnoCompleted = true;
            myThread = new Thread(new ThreadStart(GetArduino));
            myThread.Start();

            arduinoSerialPort.WriteLine("R");
            msgBox.AddNewMsg("已開始接受訊號");
        }
        catch (System.Exception ex)
        {
            Debug.Log("Start Error : " + ex);
        }
    }
    private void GetArduino()
    {        
        while (myThread.IsAlive && !cancel)
        {
            if (arduinoSerialPort.IsOpen)
            {
                try
                {
                    command = arduinoSerialPort.ReadLine();
                    readline = true;
                    //Debug.Log(arduinoSerialPort.ReadLine());
                    //Console(arduinoSerialPort.ReadLine()); // Read the information
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Run Error : " + ex);
                }
            }
        }
    }

    public void Quit()
    {
        if (connectAruidnoCompleted)
        {
            if (myThread.IsAlive)
            {
                cancel = true;
                arduinoSerialPort.Close();
                Thread.Sleep(1000);
                myThread.Abort();
                Debug.Log("Thread isAlive ? " + myThread.IsAlive);
            }
            else
            {
                Debug.Log("Aborting thread failed");
            }
        }
    }
    void OnApplicationQuit()
    {
        Quit();
    }
    #endregion
}