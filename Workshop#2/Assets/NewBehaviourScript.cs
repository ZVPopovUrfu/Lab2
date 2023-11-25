using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class NewBehaviourScript : MonoBehaviour
{
    public AudioClip quickAttack;
    public AudioClip powerfulAttack;
    public AudioClip critAttack;
    public AudioClip deadlyAttack;
    private AudioSource selectAudio;
    private List<Attack> dataSet = new List<Attack>();
    private bool statusStart = false;
    private int i = 1;
    private float critPower = 1.25f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GoogleSheets());
    }

    // Update is called once per frame
    void Update()
    {
        if (dataSet.Count <= i) return;
        if (dataSet[i].dead == 1 & statusStart == false & i != dataSet.Count)
        {
            StartCoroutine(PlaySelectAudio(deadlyAttack));
            Debug.Log("Смертельная атака");
        }

        if (dataSet[i].crit == 1 & statusStart == false & i != dataSet.Count)
        {
            StartCoroutine(PlaySelectAudio(critAttack));
            Debug.Log(dataSet[i].basicAttack * critPower + " Крит атака");
        }

        if (dataSet[i].basicAttack == 24 & statusStart == false & i != dataSet.Count)
        {
            StartCoroutine(PlaySelectAudio(quickAttack));
            Debug.Log(dataSet[i].basicAttack + " Быстрая аттака");
        }

        if (dataSet[i].basicAttack == 44 & statusStart == false & i != dataSet.Count)
        {
            StartCoroutine(PlaySelectAudio(powerfulAttack));
            Debug.Log(dataSet[i].basicAttack + " Мощная атака");
        }
    }

    IEnumerator GoogleSheets()
    {
        UnityWebRequest curentResp = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/1btlBkjaLx9OtJLb8JK0o73Wf0c_RtRDLtCd-qPdc6K8/values/Лист1?key=AIzaSyCtabUSp8PfgZ_D571iSW71DDh-o69wUrM");
        yield return curentResp.SendWebRequest();
        string rawResp = curentResp.downloadHandler.text;
        var rawJson = JSON.Parse(rawResp);
        foreach (var itemRawJson in rawJson["values"])
        {
            var parseJson = JSON.Parse(itemRawJson.ToString());
            var selectRow = parseJson[0].AsStringList;
            dataSet.Add(new Attack(int.Parse(selectRow[0]), int.Parse(selectRow[1]), int.Parse(selectRow[2])));
        }
    }

    IEnumerator PlaySelectAudio(AudioClip audioClip)
    {
        statusStart = true;
        selectAudio = GetComponent<AudioSource>();
        selectAudio.clip = audioClip;
        selectAudio.Play();
        yield return new WaitForSeconds(2);
        statusStart = false;
        i++;
    }
}

public class Attack
{
    public int basicAttack;
    public int crit;
    public int dead;

    public Attack(int basicAttack, int crit, int dead)
    {
        this.basicAttack = basicAttack;
        this.crit = crit;
        this.dead = dead;
    }
}
