using UnityEngine;
using System.Collections.Generic;

public class Person : MonoBehaviour
{
    //----------------------------------------------------------
    public enum State
    {
        Invalid,
        Listening,
        Talking
    }

    public enum ListeningState
    {
        Invalid,
        Understands,    // 
        Asleep,         // don't receive information
        Confused,       // receive information reduced
    }

    public enum TalkingState
    {
        Invalid,
        Idle,
        Slow,
        Fast,
    }

    //----------------------------------------------------------
    public GameObject IconSleep;
    public GameObject IconConfuse;
    public GameObject IconCurrentlyTaking;

    //----------------------------------------------------------
    [Space(20)]
    public GameState.Department Department = GameState.Department.Invalid;
    public State CurrentState = State.Invalid;
    public ListeningState CurrentListeningState = ListeningState.Invalid;
    public TalkingState CurrentTalkingState = TalkingState.Invalid;

    public float ScorePerSecond;

    public float TalkingTimeElapsed;
    public float MaxTimeForScore;

    //----------------------------------------------------------
    public Dictionary<GameState.Department, float> DepartmentQualities = new Dictionary<GameState.Department, float>();

    //----------------------------------------------------------
    private GameState pGameState;
    private PlayerController pPlayer;

    //----------------------------------------------------------
    private void Start ()
    {
        this.pGameState = GameState.Instance;

        this.InitDepartments();

        this.StartListeningState();
        this.pPlayer = PlayerController.i;
        this.ScorePerSecond = 1;
        this.IconSleep.SetActive(false);
        this.IconConfuse.SetActive(false);
        this.IconCurrentlyTaking.SetActive(false);
    }

    //----------------------------------------------------------
    private void InitDepartments()
    {
        this.DepartmentQualities.Clear();

        var pQualities = GameState.Instance.DepartmentQualities[this.Department];
        for (int i = 0; i < (int)GameState.Department.Num; ++i)
        {
            float[] aRange;
            var eDepartmentTo = (GameState.Department)i;
            if (pQualities.TryGetValue(eDepartmentTo, out aRange))
            {
                var fValue = Random.Range(aRange[0], aRange[1]);
                this.DepartmentQualities.Add(eDepartmentTo, fValue);

                Debug.Log("Person: To Department: " + eDepartmentTo.ToString() + ": " + fValue, this);
            }
            else
            {
                this.DepartmentQualities.Add(eDepartmentTo, 1f);
                Debug.LogError("From: " + this.Department.ToString() + " To: " + eDepartmentTo.ToString() + " did not have a range setup!" , this);
            }
        }

    }

    //----------------------------------------------------------
    public float GetScorePerSecond()
    {
        if (this.TalkingTimeElapsed >= this.MaxTimeForScore)
            return 0;

        return ScorePerSecond * 10;
    }

    //----------------------------------------------------------
    // Every 0.5
    private float timer = 0;
    private float maxTime = 0.5f;
    private void Update ()
    {
        timer += Time.unscaledDeltaTime;

        if (timer > maxTime && 
            this.CurrentState == State.Listening &&
            this.CurrentListeningState == ListeningState.Understands)
        {
            timer = 0;

            var pTalkingPerson = GameState.Instance.CurrentPersonSpeaking;
            var eTalkingDepartment = pTalkingPerson.Department;
            if (UnityEngine.Random.value > 0.5)
            {
                // Asleep
                var fQualityThreshold = this.DepartmentQualities[eTalkingDepartment];
                if (UnityEngine.Random.value > fQualityThreshold)
                {
                    this.GoToSleep();
                }
            }
            else
            {
                // Confuse
                var fQualityThreshold = this.DepartmentQualities[eTalkingDepartment];
                if (UnityEngine.Random.value > fQualityThreshold)
                {
                    this.GoToConfuse();
                }
            }
        }

        if (this.CurrentState == State.Talking)
        {
            this.TalkingTimeElapsed += Time.unscaledDeltaTime;
        }
	}

    //----------------------------------------------------------
    public void Talk()
    {
        this.StartTalkState();
    }

    //----------------------------------------------------------
    public void StopTalk()
    {
        this.StartListeningState();
    }

    //----------------------------------------------------------
    private void StartTalkState()
    {
        this.CurrentState = State.Talking;
        this.CurrentTalkingState = TalkingState.Idle;

        this.IconSleep.SetActive(false);
        this.IconConfuse.SetActive(false);
        this.IconCurrentlyTaking.SetActive(true);

        this.TalkingTimeElapsed = 0;
        this.MaxTimeForScore = Random.Range(9f, 13f);
    }

    //----------------------------------------------------------
    private void StartListeningState()
    {
        this.CurrentState = State.Listening;
        this.CurrentListeningState = ListeningState.Understands;
    }

    //----------------------------------------------------------
    private void GoToSleep()
    {
        this.CurrentListeningState = ListeningState.Asleep;

        this.ScorePerSecond = 0;

        this.IconSleep.SetActive(true);
        this.IconConfuse.SetActive(false);
        this.IconCurrentlyTaking.SetActive(false);
    }

    //----------------------------------------------------------
    private void GoToConfuse()
    {
        this.CurrentListeningState = ListeningState.Asleep;

        this.ScorePerSecond = 0.5f;

        this.IconSleep.SetActive(false);
        this.IconConfuse.SetActive(true);
        this.IconCurrentlyTaking.SetActive(false);
    }

    //----------------------------------------------------------
    public bool GetExplain()
    {
        if (this.CurrentListeningState == ListeningState.Confused)
        {
            this.CurrentListeningState = ListeningState.Understands;

            this.ScorePerSecond = 1f;

            this.IconSleep.SetActive(false);
            this.IconConfuse.SetActive(false);
            this.IconCurrentlyTaking.SetActive(false);

            return true;
        }
        return false;
    }

    //----------------------------------------------------------
    public bool GetWakeup()
    {
        if (this.CurrentListeningState == ListeningState.Asleep)
        {
            this.CurrentListeningState = ListeningState.Understands;

            this.ScorePerSecond = 1f;

            this.IconSleep.SetActive(false);
            this.IconConfuse.SetActive(false);
            this.IconCurrentlyTaking.SetActive(false);

            return true;
        }
        return false;
    }

    //----------------------------------------------------------
    public void OnMouseDown()
    {
        bool activated = false;

        switch (pPlayer.playerState)
        {
            case PlayerController.PlayerState.Idle:
                break;
            case PlayerController.PlayerState.ActiveExplain:
                activated = GetExplain();
                break;
            case PlayerController.PlayerState.ActiveWakeup:
                activated = GetWakeup();
                break;
            default:
                break;
        }

        if (activated)
        {
            pPlayer.BackToIdle();
        }

    }

}
