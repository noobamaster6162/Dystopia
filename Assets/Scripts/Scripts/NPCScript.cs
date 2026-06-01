
using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class NPCScript : MonoBehaviour
{

    [SerializeField] private GameObject npc;
    [SerializeField] private TextMeshProUGUI npcDialogue;
    [SerializeField] private float npcHealth;
    [SerializeField] private string dialogue;

    private bool _isPlayerNear;
    private bool _showText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.eKey.wasPressedThisFrame && _isPlayerNear) _showText = true;
        else _showText = false;

        if (_showText)
        {
            ShowDialogue(dialogue);
        }

        else
        {
            HideDialogue();
        }

    }

    private void ShowDialogue(string str)
    {
        npcDialogue.SetText(str);
        npcDialogue.enabled = true;
    }

    private void HideDialogue()
    {
        npcDialogue.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isPlayerNear = false;
        HideDialogue();
    }
}
