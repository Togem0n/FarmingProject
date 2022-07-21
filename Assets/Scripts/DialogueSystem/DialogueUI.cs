using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryBar;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private GameObject dialogueBox;

    public bool IsOpen { get; private set; }

    private TypewritterEffect typewritterEffect;
    private ResponseHandler responseHandler;
    private void Start()
    {
        typewritterEffect = GetComponent<TypewritterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        CloseDialogueBox();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;

        dialogueBox.SetActive(true);

        inventoryBar.SetActive(false);

        Player.Instance.DisablePlayerInput();

        if (TimelineManager.Instance.timelineToPlay != null)
        {
            TimelineManager.Instance.PauseTimeline();
        }

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for(int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            yield return typewritterEffect.Run(dialogue, textLabel);

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    private void CloseDialogueBox()
    {

        if (TimelineManager.Instance.timelineToPlay != null)
        {
            TimelineManager.Instance.ResumeTimeline();
        }
        else
        {
            Player.Instance.EnablePlayerInput();
        }

        IsOpen = false;
        dialogueBox.SetActive(false);
        inventoryBar.SetActive(true);
        textLabel.text = string.Empty;
    }
}
