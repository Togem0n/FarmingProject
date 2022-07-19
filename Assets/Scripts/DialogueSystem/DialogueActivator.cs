using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] public bool needPlayerInput = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent<Player>(out Player player))
        {
            player.Interactable = this;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent<Player>(out Player player))
        {
            player.Interactable = null;
        }
    }

    public void Interact(Player player)
    {
        if(TryGetComponent(out DialogueResponseEvent responseEvent))
        {
            player.DialogueUI.AddResponseEvents(responseEvent.Events);
        }

        player.DialogueUI.ShowDialogue(dialogueObject);
    }

    public bool needClick()
    {
        return needPlayerInput;
    }
}
