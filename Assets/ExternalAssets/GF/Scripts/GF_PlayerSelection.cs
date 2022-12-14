using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class Selection_Elements
{
    public GameObject LoadingScreen;
    public Slider FillBar;
    public TMP_Text value;
    [Header("Player Attributes")]
    public Image Speed_Bar;
    public Image Handling_Bar;
    public Image Acceleration_Bar;
    public Text InfoText;

    [Header("UI Buttons")]
    public Button PlayBtn;
    public GameObject NextBtn;
    public GameObject PrevBtn;
}

[System.Serializable]
public class PlayerAttributes{
    public string Name;
    [Tooltip("Text to display when this Player is locked.")]
    [Multiline]
    public string Info;
    public GameObject PlayerObject;
    [Range(0, 100)]
    public int Speed;
    [Range(0, 100)]
    public int Handling;
    [Range(0, 100)]
    public int Acceleration;
    public bool Locked;
    [Tooltip("Enter Level Number which must be unlocked to get this Player.")]
    public int LevelRequired;

}

public class GF_PlayerSelection : MonoBehaviour {

    [Header("Scene Selection")]
    public Scenes PreviousScene;
    public Scenes NextScene;

    [Header("UI Elements")]
    public Selection_Elements Selection_UI;

    [Header("Player Attributes")]
    public PlayerAttributes[] Players;



    [Header("Ad Sequence ID")]
    public int SequenceID;
	public bool LoadingSequence;
	public int LoadingSequenceID;

    AsyncOperation async = null;
    private int current;

    void Start(){
		Time.timeScale = 1;
		AudioListener.pause = false;
        Selection_UI.FillBar.value = 0;
        //  Selection_UI.LoadingScreen.SetActive(false);
        StartCoroutine(startGame());
        if (!GameManager.Instance.Initialized) {
			InitializeGame();
		}
        GetPlayerInfo();
		ShowAds(SequenceID, "Player Selection");
    }

	void InitializeGame() {
		SaveData.Instance = new SaveData();
		GF_SaveLoad.LoadProgress();
		GameManager.Instance.Initialized = true;
	}

	void ShowAds(int id, string SceneName) {
		GF_AdsManager.ShowAdvertisement (id, SceneName);
	}

    void Update(){
        if (async != null)
        {
            async.allowSceneActivation = false;
            Selection_UI.FillBar.value += async.progress / 800;
            Selection_UI.value.text = (Mathf.Floor(Selection_UI.FillBar.value * 100)) + "%";
            if (Selection_UI.FillBar.value >= (Selection_UI.FillBar.maxValue))
                async.allowSceneActivation = true;
        }
        /*if (async != null)
        {
            Selection_UI.FillBar.value = async.progress;
            if (async.progress >= 0.9f)
            {
                Selection_UI.FillBar.value = 1.0f;
            }
        }*/
    }

    void GetPlayerInfo(){
        for (int i = 0; i < Players.Length; i++)
        {
            if (i == current)
            {
                Players[i].PlayerObject.SetActive(true);
            }
            else if (i != current)
            {
                Players[i].PlayerObject.SetActive(false);
            }
        }

        Selection_UI.Speed_Bar.fillAmount = Players[current].Speed / 100.0f;
        Selection_UI.Handling_Bar.fillAmount = Players[current].Handling / 100.0f;
        Selection_UI.Acceleration_Bar.fillAmount = Players[current].Acceleration / 100.0f;

        if (Players[current].LevelRequired > SaveData.Instance.Level)
        {
            Players[current].Locked = true;
            Selection_UI.InfoText.text = Players[current].Info;
            Selection_UI.PlayBtn.interactable = false;
        }
        else if (Players[current].LevelRequired <= SaveData.Instance.Level)
        {
            Players[current].Locked = false;
            Selection_UI.InfoText.text = "";
            Selection_UI.PlayBtn.interactable = true;
        }

        if (current == 0)
        {
            Selection_UI.PrevBtn.SetActive(false);
            Selection_UI.NextBtn.SetActive(true);
        }
        else if (current == Players.Length - 1)
        {
            Selection_UI.PrevBtn.SetActive(true);
            Selection_UI.NextBtn.SetActive(false);
        }
        else
        {
            Selection_UI.PrevBtn.SetActive(true);
            Selection_UI.NextBtn.SetActive(true);
        }
    }

    public void Previous(){
        current--;
        GetPlayerInfo();
    }

    public void Next(){
        current++;
        GetPlayerInfo();
    }

    public void PlayLevel(){
		GameManager.Instance.SessionStatus = 1;
        GameManager.Instance.CurrentPlayer = current+1;
		Selection_UI.LoadingScreen.SetActive(true);
        StartCoroutine(LevelStart());
    }

    IEnumerator startGame()
    {
        async = SceneManager.LoadSceneAsync(NextScene.ToString());
        yield return async;
    }

    IEnumerator LevelStart(){
		if (LoadingSequence){
			ShowAds(LoadingSequenceID, "Loading Screen");
			yield return new WaitForSeconds (3);
		}
		async = SceneManager.LoadSceneAsync(NextScene.ToString());
        yield return async;
    }

    public void BackBtn(){
        SceneManager.LoadScene(PreviousScene.ToString());
    }
}
