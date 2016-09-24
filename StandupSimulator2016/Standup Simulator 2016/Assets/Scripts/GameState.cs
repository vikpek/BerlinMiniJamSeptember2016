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

    public static GameState Instance { get; private set; }

    //----------------------------------------------------------
    public int Persons = 5;
    public float GameSeconds = 60 * 5;
    public float PersonDistance = 5;
    public GameObject PersonParent;
    public CountdownArmAnimator pCountDownArm;
    public List<GameObject> PersonPrefabs = new List<GameObject>();

    [Space(20)]
    public State CurrentState;
    public float GameSecondsElapsed;
    public Person[] CurrentPersons;
    public Person CurrentSpeakingPerson;

    //----------------------------------------------------------
    public Dictionary<Department, Dictionary<Department, float[]>> DeparmentQualities = new Dictionary<Department, Dictionary<Department, float[]>>();

    //----------------------------------------------------------
    private List<Department> pPersonSetup;
    private Random pRandom;

    //----------------------------------------------------------
    private void Awake ()
    {
        GameState.Instance = this;
        this.CurrentState = State.WaitToStart;
    }

    //----------------------------------------------------------
    void Start()
    {
        this.InitGame();

        this.StartGame();
    }

    //----------------------------------------------------------
    [ContextMenu("StartGame")]
    public void StartGame()
    {
        this.GameSecondsElapsed = 0;
        if (this.pCountDownArm != null)
        {
            this.pCountDownArm.InitateCountDown((int)this.GameSeconds);
        }
        this.CurrentState = State.Running;
    }

    //----------------------------------------------------------
    [ContextMenu("InitGame")]
    void InitGame()
    {
        this.pPersonSetup = new List<Department>();
        for (int i = 0; i < this.Persons; ++i)
        {
            if (i < (int)Department.Num)
            {
                this.pPersonSetup.Add((Department)i);
            }
            else
            {
                this.pPersonSetup.Add((Department)Random.Range((int)Department.Art, (int)Department.Num));
            }
        }
        Shuffle(this.pPersonSetup);

        this.CreatePersons();
    }

    //----------------------------------------------------------
    public static void Shuffle<T>(IList<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    //----------------------------------------------------------
    //[ContextMenu("CreatePersons")]
    void CreatePersons()
    {
        this.DestroyPersons();

        if (this.pPersonSetup == null)
            this.InitGame();

        this.CurrentPersons = new Person[this.Persons];
        for (int i = 0; i < this.Persons; ++i)
        {
            var eDepartment = this.pPersonSetup[i];

            var pPrefab = this.PersonPrefabs[(int)eDepartment];
            if (pPrefab != null)
            {
                var pPersonObject = GameObject.Instantiate(pPrefab, this.PersonParent.transform) as GameObject;
#if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo(pPersonObject, "Created Person");
#endif
                this.CurrentPersons[i] = pPersonObject.GetComponent<Person>();
            }
        }
        this.PositionPersons();

#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(this, "Person Setup");
#endif
    }

    //----------------------------------------------------------
    [ContextMenu("DestroyPersons")]
    void DestroyPersons()
    {
        if (this.CurrentPersons != null)
        {
            for (int i = 0; i < this.CurrentPersons.Length; ++i)
            {
                if (this.CurrentPersons[i] != null)
                {
                    if (Application.isPlaying)
                    {
                        GameObject.Destroy(this.CurrentPersons[i].gameObject);
                    }
                    else
                    {
#if UNITY_EDITOR
                        UnityEditor.Undo.DestroyObjectImmediate(this.CurrentPersons[i].gameObject);
#endif
                    }
                }
            }
        }
        this.CurrentPersons = null;

#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(this, "Person Setup");
#endif
    }

    //----------------------------------------------------------
    void PositionPersons()
    {
        var fAngleStep = Mathf.PI / (float)(this.Persons - 1);
        var fCurrentAngle = 0f;
        for (int i = 0; i < this.Persons; ++i, fCurrentAngle += fAngleStep)
        {
            var fDegAngle = Mathf.Rad2Deg * -fCurrentAngle;
            this.CurrentPersons[i].transform.localPosition = Quaternion.Euler(new Vector3(0, 0, fDegAngle)) * (Vector3.left * this.PersonDistance);
        }
    }

    //----------------------------------------------------------
    private void Update ()
    {
        if (this.CurrentState == State.Running)
        {
            this.GameSecondsElapsed += Time.unscaledDeltaTime;

            if (this.GameSecondsElapsed >= this.GameSeconds)
            {
                this.FinishGame();
            }
        }
	}

    //----------------------------------------------------------
    private void FinishGame()
    {
        this.CurrentState = State.ShowHighscore;
    }

    //----------------------------------------------------------
}
