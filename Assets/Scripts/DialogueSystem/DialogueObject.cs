using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Dialogue", fileName = "Dialogue")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;

    public string Name => name;

    public string[] Dialogue => dialogue;

    public bool HasResponses => Responses != null && Responses.Length > 0;

    public Response[] Responses => responses;
}
