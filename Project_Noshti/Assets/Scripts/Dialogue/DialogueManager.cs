using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public Text dialogueName;
    public Text dialogueText;

    public Animator dialogueBoxAnimator;

    public float timeBeforeAllowingNextStatement = 0.5f;
    public float timeBetweenDialogueBoxAndTextAppearance = 0.2f;
    public float timeBetweenNameAndTextAppearance = 0.2f;

    private PlayerActions playerActions;

    private Queue<Statement> statements = new Queue<Statement>();
    private Statement upcomingStatement;

    private bool dialogueIsOpen = false;
    private bool canGoToNextStatement = true;
    
    //Events
    public delegate void BoxImageAction();
    public event BoxImageAction OnBoxFullyVisible;
    public event BoxImageAction OnBoxInvisible;

    public delegate void DialogueStartAction();
    public event DialogueStartAction OnDialogueStart;
    public event DialogueStartAction OnDialogueEnd;

    public bool DialogueIsPlaying
    {
        get
        {
            return dialogueIsOpen;
        }
    }

    public Queue<Statement> StatementsQueue
    {
        get
        {
            return statements;
        }
    }

    private void Start()
    {
        statements = new Queue<Statement>();
        ClearAllText();
        playerActions = PlayerActions.CreateWithDefaultBindings();
    }

    private void Update()
    {
        //Logic for DisplayNextStatement();
    }

    private void ClearAllText()
    {
        dialogueName.text = "";
        dialogueText.text = "";
    }

    //To be called from an Animator Statemachinebehavior
    public void DialogueBoxIsInvisible()
    {
        if(OnBoxInvisible != null)
        {
            OnBoxInvisible();
        }
    }

    //To be called from an Animator Statemachinebehavior
    public void DialogueBoxIsVisible()
    {
        Invoke("DisplayNextStatement", timeBetweenDialogueBoxAndTextAppearance);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueIsOpen = true;
        dialogueBoxAnimator.SetBool("IsOpen", true);
        statements.Clear();

        if(OnDialogueStart != null)
        {
            OnDialogueStart();
        }

        foreach (Statement statement in dialogue.statements)
        {
            statements.Enqueue(statement);
        }
    }

    public void DisplayNextStatement()
    {
        DisplayNextStatement(timeBetweenNameAndTextAppearance);
    }

    public void DisplayNextStatement(float timeNameVisibleBeforeText)
    {
        if(canGoToNextStatement)
        {
            StopTyping();

            canGoToNextStatement = false;
            Invoke("AllowGoingToNextStatement", timeBeforeAllowingNextStatement);

            if (statements.Count == 0)
            {
                EndDialogue();
                return;
            }

            upcomingStatement = statements.Dequeue();

            StartCoroutine(TypeDialogueName(upcomingStatement.characterName));
            dialogueText.text = "";
            Invoke("TypeUpcomingText", timeNameVisibleBeforeText);
        }
    }

    public void StopTyping()
    {
        StopAllCoroutines();
    }

    private void AllowGoingToNextStatement()
    {
        canGoToNextStatement = true;
    }

    private void TypeUpcomingText()
    {
        StartCoroutine(TypeDialogueText(upcomingStatement));
    }

    IEnumerator TypeDialogueText(Statement statement)
    {
        dialogueText.text = "";

        foreach (char letter in statement.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }

        if(OnBoxFullyVisible != null)
        {
            OnBoxFullyVisible();
        }
    }

    IEnumerator TypeDialogueName(string name)
    {
        if(dialogueName.text != name)
        {
            dialogueName.text = "";

            foreach (char letter in name.ToCharArray())
            {
                dialogueName.text += letter;
                yield return null;
            }
        }
    }

    public void EndDialogue()
    {
        dialogueIsOpen = false;
        ClearAllText();
        Invoke("PlayClosingAnimation", timeBetweenDialogueBoxAndTextAppearance);
        if(OnDialogueEnd != null)
        {
            OnDialogueEnd();
        }
    }

    private void PlayClosingAnimation()
    {
        dialogueBoxAnimator.SetBool("IsOpen", false);
    }

    
}
