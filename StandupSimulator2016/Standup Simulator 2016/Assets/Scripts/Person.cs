using UnityEngine;
using System.Collections.Generic;

public class Person : MonoBehaviour
{
    //----------------------------------------------------------
    public enum State
    {
        Invalid,
        Idle,           // no game is running, just idling
        Listening,
        Talking,
        Celebrating,
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
    public GameObject ModelToRotate;
    public Animator ModelToAnimate;

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
    private PlayerController pPlayer;

    //----------------------------------------------------------
    private void Start ()
    {
        this.InitDepartments();

        this.pPlayer = PlayerController.i;
        this.ScorePerSecond = 1;

        this.Reset();

        this.IconConfuse.transform.localRotation = Quaternion.identity;
        this.IconSleep.transform.localRotation = Quaternion.identity;
        this.IconCurrentlyTaking.transform.localRotation = Quaternion.identity;
    }

    //----------------------------------------------------------
    public void Reset()
    {
        this.CurrentState = State.Idle;
        this.IconSleep.SetActive(false);
        this.IconConfuse.SetActive(false);
        this.IconCurrentlyTaking.SetActive(false);

        this.ModelToAnimate.SetBool("Talking", false);
        this.ModelToAnimate.SetBool("Confuse", false);
        this.ModelToAnimate.SetBool("Sleep", false);
        this.ModelToAnimate.SetBool("Celebrating", false);

        this.ModelToAnimate.SetFloat("RandomSpeed", Random.Range(0.9f, 1.1f));
        this.ModelToAnimate.SetFloat("CycleOffset", Random.Range(0f, 1f));
    }

    //----------------------------------------------------------
    public void SetToCelebrating()
    {
        this.CurrentState = State.Celebrating;
        this.IconSleep.SetActive(false);
        this.IconConfuse.SetActive(false);
        this.IconCurrentlyTaking.SetActive(false);

        this.ModelToAnimate.SetBool("Talking", false);
        this.ModelToAnimate.SetBool("Confuse", false);
        this.ModelToAnimate.SetBool("Sleep", false);
        this.ModelToAnimate.SetBool("Celebrating", true);
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
            }
            else
            {
                this.DepartmentQualities.Add(eDepartmentTo, 1f);
                Debug.LogError("From: " + this.Department.ToString() + " To: " + eDepartmentTo.ToString() + " did not have a range setup!" , this);
            }
        }

    }

    //----------------------------------------------------------
    public bool HasTalkingTimeEnded()
    {
        return (this.TalkingTimeElapsed >= this.MaxTimeForScore);
    }

    //----------------------------------------------------------
    public float GetTalkingProgress()
    {
        return Mathf.Min(1f, (this.TalkingTimeElapsed / this.MaxTimeForScore));
    }

    //----------------------------------------------------------
    public float GetScorePerSecond()
    {
        return ScorePerSecond * 10;
    }

    //----------------------------------------------------------
    // Every 0.5
    private float timer = 0;
    private void Update ()
    {
        timer += Time.unscaledDeltaTime;

        if (this.CurrentState == State.Listening &&
            this.CurrentListeningState == ListeningState.Understands)
        {
            if (timer >= GameState.Instance.SecondsToUpdatePlayerState)
            {
                timer = 0;

                var pTalkingPerson = GameState.Instance.CurrentPersonSpeaking;
                var eTalkingDepartment = pTalkingPerson.Department;
                if (UnityEngine.Random.value > 0.5f)
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
        }
        else
        {
            timer = 0;
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

        this.ModelToAnimate.SetBool("Talking", true);
        this.ModelToAnimate.SetBool("Confuse", false);
        this.ModelToAnimate.SetBool("Sleep", false);
        this.ModelToAnimate.SetBool("Celebrating", false);

        this.TalkingTimeElapsed = 0;
        this.MaxTimeForScore = Random.Range(9f, 13f);

        if (GameState.Instance.RotateAllPersonsToTalkingPlayer)
        {
            GameState.Instance.RotateAllPersonsToPerson(this);
        }
    }

    //----------------------------------------------------------
    public void StartListeningState()
    {
        this.CurrentState = State.Listening;
        this.CurrentListeningState = ListeningState.Understands;

        this.IconSleep.SetActive(false);
        this.IconConfuse.SetActive(false);
        this.IconCurrentlyTaking.SetActive(false);

        this.ModelToAnimate.SetBool("Talking", false);
        this.ModelToAnimate.SetBool("Confuse", false);
        this.ModelToAnimate.SetBool("Sleep", false);
        this.ModelToAnimate.SetBool("Celebrating", false);
    }

    //----------------------------------------------------------
    private void GoToSleep()
    {
        this.CurrentListeningState = ListeningState.Asleep;

        this.ScorePerSecond = 0;

        this.IconSleep.SetActive(true);
        this.IconConfuse.SetActive(false);
        this.IconCurrentlyTaking.SetActive(false);

        this.ModelToAnimate.SetBool("Talking", false);
        this.ModelToAnimate.SetBool("Confuse", false);
        this.ModelToAnimate.SetBool("Sleep", true);
        this.ModelToAnimate.SetBool("Celebrating", false);
    }

    //----------------------------------------------------------
    private void GoToConfuse()
    {
        this.CurrentListeningState = ListeningState.Confused;

        this.ScorePerSecond = 0.5f;

        this.IconSleep.SetActive(false);
        this.IconConfuse.SetActive(true);
        this.IconCurrentlyTaking.SetActive(false);

        this.ModelToAnimate.SetBool("Talking", false);
        this.ModelToAnimate.SetBool("Confuse", true);
        this.ModelToAnimate.SetBool("Sleep", false);
        this.ModelToAnimate.SetBool("Celebrating", false);
    }

    //----------------------------------------------------------
    private void ReturnToUnderstands()
    {
        this.CurrentListeningState = ListeningState.Understands;

        this.ScorePerSecond = 1f;

        this.IconSleep.SetActive(false);
        this.IconConfuse.SetActive(false);
        this.IconCurrentlyTaking.SetActive(false);

        this.ModelToAnimate.SetBool("Talking", false);
        this.ModelToAnimate.SetBool("Confuse", false);
        this.ModelToAnimate.SetBool("Sleep", false);
        this.ModelToAnimate.SetBool("Celebrating", false);
    }

    //----------------------------------------------------------
    public bool GetExplain()
    {
        if (this.CurrentListeningState == ListeningState.Confused)
        {
            this.ReturnToUnderstands();
            return true;
        }
        return false;
    }

    //----------------------------------------------------------
    public bool GetWakeup()
    {
        if (this.CurrentListeningState == ListeningState.Asleep)
        {
            this.ReturnToUnderstands();
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

    //----------------------------------------------------------
}
