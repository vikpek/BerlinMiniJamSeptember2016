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

    //----------------------------------------------------------
    [Space(20)]
    public GameState.Department Department = GameState.Department.Invalid;
    public State CurrentState = State.Invalid;
    public ListeningState CurrentListeningState = ListeningState.Invalid;
    public TalkingState CurrentTalkingState = TalkingState.Invalid;

    public float ScorePerSecond;

    //----------------------------------------------------------
    public Dictionary<GameState.Department, float> DeparmentQualities = new Dictionary<GameState.Department, float>();

    //----------------------------------------------------------
    private GameState pGameState;

    private PlayerController pPlayer;

    //----------------------------------------------------------
    private void Start ()
    {
        this.StartListeningState();
        this.pPlayer = PlayerController.i;
        this.ScorePerSecond = 1;
        this.pGameState = GameState.Instance;
    }

    private void InitDepartments()
    {
    }

    //----------------------------------------------------------
    public float GetScorePerSecond()
    {
        return ScorePerSecond * 10;
    }

    //----------------------------------------------------------
    // Every 0.5

    private float timer = 0;
    private float maxTime = 0.5f;
    private void Update ()
    {
        timer += Time.deltaTime;

        if (timer > maxTime)
        {
            timer = 0;

            if (UnityEngine.Random.value > 0.5)
            {
                // Asleep
                pGameState.DepartmentQualities[Department].
            }
            else
            {
                // Confuse
                
            }
        }
	}

    //----------------------------------------------------------
    public void Talk()
    {
        this.StartListeningState();
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
    }

    //----------------------------------------------------------
    private void StartListeningState()
    {
        this.CurrentState = State.Listening;
        this.CurrentListeningState = ListeningState.Understands;
    }

    //----------------------------------------------------------
    public bool GetExplain()
    {
        if (this.CurrentListeningState == ListeningState.Confused)
        {
            this.CurrentListeningState = ListeningState.Understands;
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
