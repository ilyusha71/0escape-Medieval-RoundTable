/**************************************************************************************** 
 * Wakaka Studio 2017
 * Author: iLYuSha Dawa-mumu Wakaka Kocmocovich Kocmocki KocmocA
 * Project:: 0escape Medieval - Round Table
 * Tools: Unity 5/C# + Arduino/C++
 * Last Updated: 2017/07/29
 ****************************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class WakakaGalaxy : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }
    public GameObject Observatory;
    //private int pentagramOrder; // 五芒星棋盤10條線的編號
    private string[] roundTableKnight; // 圓柱目前所屬的騎士
    public GameObject roundTableThrone;
    public GameObject[] roundTableTristana;
    public GameObject[] roundTableGalahad;
    public GameObject[] roundTableArthur;
    public GameObject[] roundTableLancelot;
    public Transform[] roundTable;
    public Transform star;
    public Transform[] starNode;
    public GameObject table;
    public Transform goal;

    private bool ready;
    private bool roll;

    public GameObject pentagram;
    private int laserPoint;

    public Text rotText;

    private float startTime;
    private float recordTime;


    void Awake()
    {
        if (ArduinoController.instance == null)
        {
            PlayerPrefs.SetInt("lastScene", SceneManager.GetActiveScene().buildIndex+100);
            SceneManager.LoadScene("Arduino Controller");
        }

        roundTableKnight = new string[5];
        roundTableKnight[0] = "";

        roundTableKnight[1] = "Tristan";
        roundTableKnight[2] = "Galahad";
        roundTableKnight[3] = "Arthur";
        roundTableKnight[4] = "Lancelot";
        pentagram.SetActive(false);
        ready = true;
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

            if (laserPoint > 30 && laserPoint<10000)
            {
                if (startTime == 0)
                {
                    startTime = Time.time;
                    ArduinoController.arduinoSerialPort.Write("A");
                    ArduinoController.instance.msgBox.AddNewMsg("雷射陣開啟");
                }
                if (Time.time - startTime > 7.0f)
                {
                    roll = true;
                    startTime = Time.time;
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
            if (roundTableKnight[0] == "Arthur" &&
                roundTableKnight[1] == "Lancelot" &&
                roundTableKnight[3] == "Galahad" &&
                roundTableKnight[4] == "Tristan")
            {
                ArduinoController.arduinoSerialPort.WriteLine("E");
                ArduinoController.instance.msgBox.AddNewMsg("遊戲結束");
                Debug.Log("END");
                float endTime = Time.time - startTime;
                if (endTime < PlayerPrefs.GetFloat("recordTime"))
                    PlayerPrefs.SetFloat("recordTime", endTime);
                roll = false;
                StartCoroutine(End());

            }

            /* 觸發旋轉 */
            if (Input.GetKey(KeyCode.Keypad1) || ArduinoController.command == "ARTHUR")
                roundTable[0].rotation *= Quaternion.Euler(0, 0, -37 * Time.deltaTime);
            if (Input.GetKey(KeyCode.Keypad2) || ArduinoController.command == "LANCELOT")
                roundTable[1].rotation *= Quaternion.Euler(0, 0, 37 * Time.deltaTime);
            if (Input.GetKey(KeyCode.Keypad3) || ArduinoController.command == "ECTOR")
                roundTable[2].rotation *= Quaternion.Euler(0, 0, -37 * Time.deltaTime);
            if (Input.GetKey(KeyCode.Keypad4) || ArduinoController.command == "GALAHAD")
                roundTable[3].rotation *= Quaternion.Euler(0, 0, -37 * Time.deltaTime);
            if (Input.GetKey(KeyCode.Keypad5) || ArduinoController.command == "TRISTAN")
                roundTable[4].rotation *= Quaternion.Euler(0, 0, 37 * Time.deltaTime);

            /* 觸發移動 */
            if (ready)
            {
                if (Input.GetKey(KeyCode.Keypad1) && Input.GetKey(KeyCode.Keypad2) || ArduinoController.command == "1")
                {
                    if (roundTableKnight[0] == "Arthur" && roundTableKnight[1] == "")
                        StartCoroutine(MoveKnight(roundTableKnight[0], 0, 1));
                    else if (roundTableKnight[0] == "" && roundTableKnight[1] == "Arthur")
                        StartCoroutine(MoveKnight(roundTableKnight[1], 1, 0));
                }
                else if (Input.GetKey(KeyCode.Keypad2) && Input.GetKey(KeyCode.Keypad3) || ArduinoController.command == "2")
                {
                    if (roundTableKnight[1] != "" && roundTableKnight[2] == "")
                    {
                        if (roundTableKnight[1] == "Arthur" || roundTableKnight[1] == "Tristan" || roundTableKnight[1] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[1], 1, 2));
                    }
                    else if (roundTableKnight[1] == "" && roundTableKnight[2] != "")
                    {
                        if (roundTableKnight[2] == "Arthur" || roundTableKnight[2] == "Tristan" || roundTableKnight[2] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[2], 2, 1));
                    }
                }
                else if (Input.GetKey(KeyCode.Keypad3) && Input.GetKey(KeyCode.Keypad4) || ArduinoController.command == "3")
                {
                    if (roundTableKnight[2] != "" && roundTableKnight[3] == "")
                    {
                        if (roundTableKnight[2] == "Arthur" || roundTableKnight[2] == "Tristan" || roundTableKnight[2] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[2], 2, 3));
                    }
                    else if (roundTableKnight[2] == "" && roundTableKnight[3] != "")
                    {
                        if (roundTableKnight[3] == "Arthur" || roundTableKnight[3] == "Tristan" || roundTableKnight[3] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[3], 3, 2));
                    }
                }
                else if (Input.GetKey(KeyCode.Keypad4) && Input.GetKey(KeyCode.Keypad5) || ArduinoController.command == "4")
                {
                    if (roundTableKnight[3] != "" && roundTableKnight[4] == "")
                    {
                        if (roundTableKnight[3] == "Arthur" || roundTableKnight[3] == "Tristan" || roundTableKnight[3] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[3], 3, 4));
                    }
                    else if (roundTableKnight[3] == "" && roundTableKnight[4] != "")
                    {
                        if (roundTableKnight[4] == "Arthur" || roundTableKnight[4] == "Tristan" || roundTableKnight[4] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[4], 4, 3));
                    }
                }
                else if (Input.GetKey(KeyCode.Keypad5) && Input.GetKey(KeyCode.Keypad1) || ArduinoController.command == "5")
                {
                    if (roundTableKnight[4] == "Arthur" && roundTableKnight[0] == "")
                        StartCoroutine(MoveKnight(roundTableKnight[4], 4, 0));
                    else if (roundTableKnight[4] == "" && roundTableKnight[0] == "Arthur")
                        StartCoroutine(MoveKnight(roundTableKnight[0], 0, 4));
                }
                else if (Input.GetKey(KeyCode.Keypad1) && Input.GetKey(KeyCode.Keypad3) || ArduinoController.command == "6")
                {
                    if (roundTableKnight[0] == "Arthur" && roundTableKnight[2] == "")
                        StartCoroutine(MoveKnight(roundTableKnight[0], 0, 2));
                    else if (roundTableKnight[0] == "" && roundTableKnight[2] == "Arthur")
                        StartCoroutine(MoveKnight(roundTableKnight[2], 2, 0));
                }
                else if (Input.GetKey(KeyCode.Keypad3) && Input.GetKey(KeyCode.Keypad5) || ArduinoController.command == "7")
                {
                    if (roundTableKnight[2] != "" && roundTableKnight[4] == "")
                    {
                        if (roundTableKnight[2] == "Arthur" || roundTableKnight[2] == "Galahad" || roundTableKnight[2] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[2], 2, 4));
                    }
                    else if (roundTableKnight[2] == "" && roundTableKnight[4] != "")
                    {
                        if (roundTableKnight[4] == "Arthur" || roundTableKnight[4] == "Galahad" || roundTableKnight[4] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[4], 4, 2));
                    }
                }
                else if (Input.GetKey(KeyCode.Keypad5) && Input.GetKey(KeyCode.Keypad2) || ArduinoController.command == "8")
                {
                    if (roundTableKnight[4] != "" && roundTableKnight[1] == "")
                    {
                        if (roundTableKnight[4] == "Arthur" || roundTableKnight[4] == "Galahad" || roundTableKnight[4] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[4], 4, 1));
                    }
                    else if (roundTableKnight[4] == "" && roundTableKnight[1] != "")
                    {
                        if (roundTableKnight[1] == "Arthur" || roundTableKnight[1] == "Galahad" || roundTableKnight[1] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[1], 1, 4));
                    }
                }
                else if (Input.GetKey(KeyCode.Keypad2) && Input.GetKey(KeyCode.Keypad4) || ArduinoController.command == "9")
                {
                    if (roundTableKnight[1] != "" && roundTableKnight[3] == "")
                    {
                        if (roundTableKnight[1] == "Arthur" || roundTableKnight[1] == "Galahad" || roundTableKnight[1] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[1], 1, 3));
                    }
                    else if (roundTableKnight[1] == "" && roundTableKnight[3] != "")
                    {
                        if (roundTableKnight[3] == "Arthur" || roundTableKnight[3] == "Galahad" || roundTableKnight[3] == "Lancelot")
                            StartCoroutine(MoveKnight(roundTableKnight[3], 3, 1));
                    }
                }
                else if (Input.GetKey(KeyCode.Keypad4) && Input.GetKey(KeyCode.Keypad1) || ArduinoController.command == "10")
                {
                    if (roundTableKnight[3] == "Arthur" && roundTableKnight[0] == "")
                        StartCoroutine(MoveKnight(roundTableKnight[3], 3, 0));
                    else if (roundTableKnight[3] == "" && roundTableKnight[0] == "Arthur")
                        StartCoroutine(MoveKnight(roundTableKnight[0], 0, 3));
                }
            }
        }
    }

    IEnumerator MoveKnight(string knight, int from, int to)
    {
        ready = false;
        roll = false;

        if (knight == "Tristan")
        {
            roundTableTristana[from].SetActive(false);
            roundTableKnight[from] = "";
            star.DOKill();
            star.position = starNode[from].position;
            star.rotation = starNode[from].rotation;
            star.DOScale(0, 0.73f).SetEase(Ease.InExpo);
            star.DOMove(starNode[to].position, 0.73f).SetEase(Ease.InQuad);
            yield return new WaitForSeconds(0.73f);
            roundTableTristana[to].SetActive(true);
            roundTableKnight[to] = "Tristan";
        }
        else if (knight == "Galahad")
        {
            roundTableGalahad[from].SetActive(false);
            roundTableKnight[from] = "";
            star.DOKill();
            star.position = starNode[from].position;
            star.rotation = starNode[from].rotation;
            star.DOScale(0, 0.73f).SetEase(Ease.InExpo);
            star.DOMove(starNode[to].position, 0.73f).SetEase(Ease.InQuad);
            yield return new WaitForSeconds(0.73f);
            roundTableGalahad[to].SetActive(true);
            roundTableKnight[to] = "Galahad";
        }
        else if (knight == "Arthur")
        {
            roundTableArthur[from].SetActive(false);
            roundTableKnight[from] = "";
            if (to == 0)
                roundTableThrone.SetActive(false);
            star.DOKill();
            star.position = starNode[from].position;
            star.rotation = starNode[from].rotation;
            star.DOScale(0, 0.73f).SetEase(Ease.InExpo);
            star.DOMove(starNode[to].position, 0.73f).SetEase(Ease.InQuad);
            yield return new WaitForSeconds(0.73f);
            if (from == 0)
                roundTableThrone.SetActive(true);
            roundTableArthur[to].SetActive(true);
            roundTableKnight[to] = "Arthur";
        }
        else if (knight == "Lancelot")
        {
            roundTableLancelot[from].SetActive(false);
            roundTableKnight[from] = "";
            star.DOKill();
            star.position = starNode[from].position;
            star.rotation = starNode[from].rotation;
            star.DOScale(0, 0.73f).SetEase(Ease.InExpo);
            star.DOMove(starNode[to].position, 0.73f).SetEase(Ease.InQuad);
            yield return new WaitForSeconds(0.73f);
            roundTableLancelot[to].SetActive(true);
            roundTableKnight[to] = "Lancelot";
        }
        StartCoroutine(Buffer());
    }

    IEnumerator Buffer()
    {
        star.localScale = Vector3.zero;
        star.position = starNode[5].position;
        roll = true;
        star.DOScale(1, 1.0f);
        yield return new WaitForSeconds(1.0f);
        ready = true;
    }
}