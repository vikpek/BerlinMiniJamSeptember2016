using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public enum State
    {
        Invalid,
        WaitToStart,
        Running,
        ShowHighscore,
    }

    public enum Department
    {
        Art,
        Code,
        GameDesign,
        Operations,

        Invalid,
        Num = Invalid,
    }

    //----------------------------------------------------------
    public int Persons = 5;
    public float GameSeconds = 60 * 5;
    public Dictionary<Department, Dictionary<Department, float[]>> DeparmentQualities = new Dictionary<Department, Dictionary<Department, float[]>>();

    //----------------------------------------------------------
    public float GameSecondsElapsed { get; private set; }
    public Person[] CurrentPersons { get; private set; }
    public State CurrentState { get; private set; }

    //----------------------------------------------------------
    private void Awake ()
    {
        this.CurrentState = State.WaitToStart;
	}

    //----------------------------------------------------------
    public void StartGame()
    {
        this.GameSecondsElapsed = 0;
        this.CurrentState = State.Running;
    }

    //----------------------------------------------------------
    void CreatePersons()
    {
        this.DestroyPersons();

        this.CurrentPersons = new Person[this.Persons];
        for (int i = 0; i < this.Persons; ++i)
        {
            var pPrefab = Resources.Load("Person");
            if (pPrefab != null)
            {
                var pPersonObject = GameObject.Instantiate(pPrefab) as GameObject;
                this.CurrentPersons[i] = pPersonObject.GetComponent<Person>();
            }
        }
    }

    //----------------------------------------------------------
    void DestroyPersons()
    {
        if (this.CurrentPersons != null)
        {
            for (int i = 0; i < this.CurrentPersons.Length; ++i)
            {
                GameObject.Destroy(this.CurrentPersons[i].gameObject);
            }
        }
        this.CurrentPersons = null;
    }

    //----------------------------------------------------------
    void PositionPersons()
    {
        var fAngleStep = Mathf.PI / (float)this.Persons;

    }

    //----------------------------------------------------------
    private void Update ()
    {
        if (this.CurrentState == State.Running)
        {
            this.GameSeconds += Time.unscaledDeltaTime;
        }
	}

    //----------------------------------------------------------
}
