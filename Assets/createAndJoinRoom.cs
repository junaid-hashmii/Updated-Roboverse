using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class createAndJoinRoom : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;

    public GameObject PlayerObj;
    public void createRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
        Debug.Log("create");
    }

    public void JoinRoom()
    {

        PhotonNetwork.JoinRoom(joinInput.text);
    }
    public override void OnJoinedRoom()
    {
        // PhotonNetwork.LoadLevel("GamePlay");
        
        PhotonNetwork.Instantiate(PlayerObj.name, Vector3.zero, Quaternion.identity);
    }
    

}