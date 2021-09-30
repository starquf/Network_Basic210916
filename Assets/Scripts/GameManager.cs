using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private SocketModule tcp;

    [SerializeField]
    private InputField nickname;
    string myID;

    public GameObject prefabUnit;
    public GameObject mainChar;

    Dictionary<string, UnitControl> remoteUnits;
    Queue<string> commandQueue;

    private void Start()
    {
        tcp = GetComponent<SocketModule>();
        remoteUnits = new Dictionary<string, UnitControl>();
        commandQueue = new Queue<string>();
    }

    private void Update()
    {
        ProcessQueue();

        if (Input.GetMouseButtonDown(0))
        {
            // UI 눌렀을 때 움직이는 것을 방지하기 위함
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 targetPos;

                // orgPos = transform.position;
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // targetPos.z = orgPos.z;
                mainChar.GetComponent<UnitControl>().SetTargetPos(targetPos);

                string data = "#Move#" + targetPos.x + "," + targetPos.y;
                SendCommand(data);
            }
        }
    }

    private void ProcessQueue()
    {
        while (commandQueue.Count > 0)
        {
            string nextCommand = commandQueue.Dequeue();
            ProcessCommand(nextCommand);
        }
    }

    public void OnLogin()
    {
        string id = nickname.text;
        myID = id;

        if (id.Length > 0)
        {
            tcp.Login(id);
            mainChar.transform.position = Vector3.zero;
        }
    }

    public void OnLogOut()
    {
        tcp.LogOut();

        foreach (var remotePair in remoteUnits)
        {
            Destroy(remotePair.Value.gameObject);
        }

        remoteUnits.Clear();
    }

    public UnitControl AddUnit(string id)
    {
        UnitControl uc = null;

        if (!remoteUnits.ContainsKey(id))
        {
            GameObject newUnit = Instantiate(prefabUnit);
            uc = newUnit.GetComponent<UnitControl>();

            remoteUnits.Add(id, uc);
        }

        return uc;
    }

    public void SetMove(string id, string coordinates)
    {
        if (remoteUnits.ContainsKey(id))
        {
            UnitControl uc = remoteUnits[id];

            var strs = coordinates.Split(',');

            Vector3 pos = new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), 0);
            uc.SetTargetPos(pos);
        }
    }

    public void UserLeft(string id)
    {
        if (remoteUnits.ContainsKey(id))
        {
            UnitControl uc = remoteUnits[id];
            Destroy(uc.gameObject);
            remoteUnits.Remove(id);
        }
    }

    private void LoadHistory(string history)
    {
        var strs = history.Split(',');
        int max = strs.Length;

        for (int i = 0; i + 2 < max; i += 3)
        {
            string id = strs[i];

            if (myID.CompareTo(id) != 0)
            {
                UnitControl uc = AddUnit(id);

                if (uc != null)
                {
                    float x = float.Parse(strs[i + 1]);
                    float y = float.Parse(strs[i + 2]);
                    uc.transform.position = new Vector3(x, y, 0f);
                }
            }
        }
    }

    private void SendCommand(string cmd)
    {

    }

    private void ProcessCommand(string cmd)
    {
        
    }
}
