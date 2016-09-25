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
    public float SecondsToUpdatePlayerState = 1f;
    public float SecondsToAddScore = 1f;
    public bool RotateAllPersonsToTalkingPlayer = false;

    [Space(5)]
    public CountdownArmAnimator pCountDownArm;
    public List<GameObject> PersonPrefabs = new List<GameObject>();
    public GameObject WaitingUi;
    public GameObject RunningUi;
    public GameObject FinishedUi;
    public ProgressBarController TalkingFinishProgress;

    [Space(20)]
    public float Score;
    public State CurrentState;
    public float GameSecondsElapsed;
    public Person[] CurrentPersons;
    public int CurrentPersonSpeakingIndex = 0;

    //----------------------------------------------------------
    public Person CurrentPersonSpeaking
    {
        get
        {
            if (this.CurrentPersonSpeakingIndex < 0 ||
                this.CurrentPersonSpeakingIndex >= this.CurrentPersons.Length)
                return null;

            return this.CurrentPersons[this.CurrentPersonSpeakingIndex];
        }
    }

    //----------------------------------------------------------
    public Dictionary<Department, Dictionary<Department, float[]>> DepartmentQualities = new Dictionary<Department, Dictionary<Department, float[]>>();

    //----------------------------------------------------------
    private List<Department> pPersonSetup;

    //----------------------------------------------------------
    private void Awake ()
    {
        GameState.Instance = this;
        this.CurrentState = State.WaitToStart;

        this.WaitingUi.SetActive(false);
        this.RunningUi.SetActive(false);
        this.FinishedUi.SetActive(false);
    }

    //----------------------------------------------------------
    void Start()
    {
        this.InitGame();
    }

    //----------------------------------------------------------
    [ContextMenu("StartGame")]
    public void StartGame()
    {
        this.GameSecondsElapsed = 0;
        this.Score = 0;
        if (this.pCountDownArm != null)
        {
            this.pCountDownArm.InitateCountDown((int)this.GameSeconds);
        }
        this.CurrentPersonSpeakingIndex = 0;
        this.CurrentPersonSpeaking.Talk();

        for (int i=0;i<this.CurrentPersons.Length;++i)
        {
            if (i != this.CurrentPersonSpeakingIndex)
            {
                this.CurrentPersons[i].StartListeningState();
            }
        }

        this.CurrentState = State.Running;

        this.WaitingUi.SetActive(false);
        this.RunningUi.SetActive(true);
        this.FinishedUi.SetActive(false);
    }

    //----------------------------------------------------------
    [ContextMenu("InitGame")]
    void InitGame()
    {
        this.InitDepartmentQualities();
        this.InitPersonDepartments();

        this.CreatePersons();
        this.RotateAllPersonsToPerson(null);

        this.CurrentState = State.WaitToStart;

        this.WaitingUi.SetActive(true);
        this.RunningUi.SetActive(false);
        this.FinishedUi.SetActive(false);
    }

    //----------------------------------------------------------
    void InitPersonDepartments()
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
    }

    //----------------------------------------------------------
    void InitDepartmentQualities()
    {
        this.InitDepartmentQuality(Department.Art, Department.Art, 1f, 1f);
        this.InitDepartmentQuality(Department.Art, Department.Code, 0.4f, 0.6f);
        this.InitDepartmentQuality(Department.Art, Department.GameDesign, 0.6f, 0.8f);
        this.InitDepartmentQuality(Department.Art, Department.Operations, 0.3f, 0.5f);

        this.InitDepartmentQuality(Department.Code, Department.Art, 0.4f, 0.6f);
        this.InitDepartmentQuality(Department.Code, Department.Code, 1f, 1f);
        this.InitDepartmentQuality(Department.Code, Department.GameDesign, 0.7f, 0.9f);
        this.InitDepartmentQuality(Department.Code, Department.Operations, 0.2f, 0.4f);

        this.InitDepartmentQuality(Department.GameDesign, Department.Art, 0.6f, 0.8f);
        this.InitDepartmentQuality(Department.GameDesign, Department.Code, 0.7f, 0.9f);
        this.InitDepartmentQuality(Department.GameDesign, Department.GameDesign, 1f, 1f);
        this.InitDepartmentQuality(Department.GameDesign, Department.Operations, 0.5f, 0.7f);

        this.InitDepartmentQuality(Department.Operations, Department.Art, 0.3f, 0.5f);
        this.InitDepartmentQuality(Department.Operations, Department.Code, 0.2f, 0.4f);
        this.InitDepartmentQuality(Department.Operations, Department.GameDesign, 0.5f, 0.7f);
        this.InitDepartmentQuality(Department.Operations, Department.Operations, 1f, 1f);
    }

    //----------------------------------------------------------
    void InitDepartmentQuality(Department eFrom, Department eTo, float fFrom, float fTo)
    {
        if (!this.DepartmentQualities.ContainsKey(eFrom))
        {
            this.DepartmentQualities.Add(eFrom, null);
        }
        if (this.DepartmentQualities[eFrom] == null)
        {
            this.DepartmentQualities[eFrom] = new Dictionary<Department, float[]>();
        }
        if (!this.DepartmentQualities[eFrom].ContainsKey(eTo))
        {
            this.DepartmentQualities[eFrom].Add(eTo, null);
        }
        if (this.DepartmentQualities[eFrom][eTo] == null)
        {
            this.DepartmentQualities[eFrom][eTo] = new float[2];
        }
        this.DepartmentQualities[eFrom][eTo][0] = fFrom;
        this.DepartmentQualities[eFrom][eTo][1] = fTo;
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
            this.CurrentPersons[i].transform.localPosition = Quaternion.AngleAxis(fDegAngle, Vector3.forward) * (Vector3.left * this.PersonDistance);
        }
    }

    //----------------------------------------------------------
    public void RotateAllPersonsToPerson(Person pPerson)
    {
        var vWorldPositionToRotateTo = Vector3.zero;
        if (pPerson != null)
        {
            vWorldPositionToRotateTo = pPerson.transform.position;
        }
        else
        {
            vWorldPositionToRotateTo = this.PersonParent.transform.position;
        }

        for (int i = 0; i < this.Persons; ++i)
        {
            var vRotateTo = vWorldPositionToRotateTo;
            var pPersonToRotate = this.CurrentPersons[i];
            
            // rotate the talking person to the lead (you)
            if (pPersonToRotate == pPerson)
            {
                vRotateTo = this.PersonParent.transform.position;
            }

            // rotate the person to to targetposition
            var fAngle = AngleBetweenVector3(pPersonToRotate.transform.position, vRotateTo);
            this.CurrentPersons[i].ModelToRotate.transform.rotation = Quaternion.AngleAxis(fAngle, Vector3.forward) * Quaternion.Euler(-90,0,0);
        }
    }

    //----------------------------------------------------------
    private float AngleBetweenVector3(Vector3 vec1, Vector3 vec2)
    {
        Vector3 diference = vec2 - vec1;
        float sign = (vec2.x < vec1.x) ? 1.0f : -1.0f;
        return Vector3.Angle(Vector3.up, diference) * sign;
    }

    //----------------------------------------------------------
    private float timer = 0;
    private void Update ()
    {
        if (this.CurrentState == State.Running)
        {
            this.GameSecondsElapsed += Time.unscaledDeltaTime;

            if (this.GameSecondsElapsed >= this.GameSeconds)
            {
                this.FinishGame();
            }
            else
            {
                timer += Time.deltaTime;

                if (timer >= SecondsToAddScore)
                {
                    timer = 0;
                    this.GetScoreOfAllListeningPlayers();
                }

                TalkingFinishProgress.foregroundImage.fillAmount = this.CurrentPersonSpeaking.GetTalkingProgress();
            }
        }
        else
        {
            timer = 0;
        }
	}

    //----------------------------------------------------------
    public void GoToNextPerson()
    {
        if (this.CurrentPersonSpeaking != null)
        {
            this.CurrentPersonSpeaking.StopTalk();
        }

        // next person
        this.CurrentPersonSpeakingIndex++;

        if (this.CurrentPersonSpeakingIndex >= this.Persons)
        {
            this.FinishGame();
        }
        else
        {
            this.CurrentPersonSpeaking.Talk();
        }
    }

    //----------------------------------------------------------
    private void GetScoreOfAllListeningPlayers()
    {
        if (this.CurrentPersonSpeaking.HasTalkingTimeEnded())
            return;

        for (int i = 0; i < this.CurrentPersons.Length; ++i)
        {
            if (i != this.CurrentPersonSpeakingIndex)
            {
                var pPerson = this.CurrentPersons[i];
                this.Score += pPerson.GetScorePerSecond();
            }
        }
    }

    //----------------------------------------------------------
    private void FinishGame()
    {
        this.CurrentState = State.ShowHighscore;

        this.WaitingUi.SetActive(false);
        this.RunningUi.SetActive(false);
        this.FinishedUi.SetActive(true);

        this.pCountDownArm.StopAllCoroutines();

        for (int i = 0; i < this.CurrentPersons.Length; ++i)
        {
            this.CurrentPersons[i].SetToCelebrating();
        }
        this.RotateAllPersonsToPerson(null);
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
}
