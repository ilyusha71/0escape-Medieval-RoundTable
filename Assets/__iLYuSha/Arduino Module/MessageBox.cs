using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    /* Console */
    public Text msgBox;
    private List<string> msgList = new List<string>();

    public void AddNewMsg(string msg)
    {
        //Debug.Log("///////////////////////////////////////////////////////");
        msgBox.text = "";
        if (msgList.Count == 30)
            msgList.RemoveAt(0);
        msgList.Add(msg);

        int x = msgList.Count;

        for (int i = 0; i < msgList.Count; i++)
        {
            if (x != msgList.Count)
                Debug.LogError("weaadasdasd");
            if (msgList[i] != "")
                msgBox.text += ("\n" + i + ": " + msgList[i]);
            else if (msgList[i] == null)
                msgBox.text += ("\n" + i + ": null");
            else
                msgBox.text += ("\n" + i + ": 空");
            //Debug.Log(i + ":" + msgList[i]);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Backspace))
        {
            AddNewMsg(Random.Range(0, 100000000).ToString()); 
        }
    }
}
