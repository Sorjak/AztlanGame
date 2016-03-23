using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEditor;

[ExecuteInEditMode]
public class GameController : MonoBehaviour {

    public List<GameObject> playerList;
    public GameObject playerObject;

	// Use this for initialization
	void Start () {
        playerList = new List<GameObject>();
        CreatePlayers();
	}
	
	// Update is called once per frame
	void LateUpdate () 
    {
        foreach (GameObject go in playerList)
        {
            Player p = go.GetComponent<Player>();
            if (p.currentPrayer > 1.0f)
            {
                //Debug.Log(p.name + " is fully charged!");
            }
        }
	}

    public void CreatePlayers()
    {
        var numDevices = InputManager.Devices.Count;

        for (int i = 0; i < numDevices; i++)
        {
            InputDevice device = InputManager.Devices[i];
            if (device != null)
            {
                SpawnPlayer(i, device);
                Debug.Log("Creating player " + i + " and assigning it " + device.Name + i);
            }

        }
    }

    public void SpawnPlayer(int id, InputDevice device)
    {
        var playerInstance = Instantiate<GameObject>(playerObject);
        playerInstance.GetComponent<PlayerInput>().SetUpController(device ?? InputManager.ActiveDevice);
        playerInstance.name = "Player " + id;

        playerList.Add(playerInstance);
    } 
}

[CustomEditor(typeof(GameController))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameController myScript = (GameController)target;
        if (GUILayout.Button("Spawn Player"))
        {
            myScript.SpawnPlayer(0, null);
        }
    }
}
