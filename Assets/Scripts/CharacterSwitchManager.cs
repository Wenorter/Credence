using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitchManager : MonoBehaviour
{
    [Serializable]
    public struct CharacterEntry
    {
        public string name;
        public List<MonoBehaviour> behaviours;
        public List<GameObject> gameObjects;
    }

    [SerializeField] private InputActionReference switchAction;
    [SerializeField] private List<CharacterEntry> characters = new List<CharacterEntry>();
    [SerializeField] private int startIndex;

    private int _currentIndex = -1;

    public int CurrentIndex => _currentIndex;

    public string CurrentName =>
        _currentIndex >= 0 && _currentIndex < characters.Count
            ? characters[_currentIndex].name
            : string.Empty;

    private void Awake()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            SetCharacterEnabled(i, false);
        }

        if (characters.Count == 0)
            return;

        _currentIndex = Mathf.Clamp(startIndex, 0, characters.Count - 1);
        SetCharacterEnabled(_currentIndex, true);
    }

    private void OnEnable()
    {
        if (!switchAction || switchAction.action == null)
            return;

        switchAction.action.performed += OnSwitchPerformed;
        switchAction.action.Enable();
    }

    private void OnDisable()
    {
        if (switchAction == null || switchAction.action == null)
            return;

        switchAction.action.performed -= OnSwitchPerformed;
        switchAction.action.Disable();
    }

    private void OnSwitchPerformed(InputAction.CallbackContext context)
    {
        SwitchToNext();
    }

    public void SwitchToNext()
    {
        if (characters.Count <= 1)
            return;

        int nextIndex = (_currentIndex + 1) % characters.Count;

        if (_currentIndex >= 0)
        {
            SetCharacterEnabled(_currentIndex, false);
        }

        _currentIndex = nextIndex;
        SetCharacterEnabled(_currentIndex, true);
    }

    private void SetCharacterEnabled(int index, bool isEnabled)
    {
        CharacterEntry character = characters[index];

        if (!isEnabled)
        {
            SetBehavioursEnabled(character.behaviours, false);
            SetGameObjectsActive(character.gameObjects, false);
        }
        else
        {
            SetGameObjectsActive(character.gameObjects, true);
            SetBehavioursEnabled(character.behaviours, true);
        }
    }

    private void SetBehavioursEnabled(
        List<MonoBehaviour> behaviours,
        bool isEnabled)
    {
        if (behaviours == null)
            return;

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (!behaviour || behaviour == this)
                continue;

            behaviour.enabled = isEnabled;
        }
    }

    private void SetGameObjectsActive(List<GameObject> gameObjects, bool isActive)
    {
        if (gameObjects == null)
            return;

        foreach (GameObject obj in gameObjects)
        {
            if (!obj) continue;

            if (transform.IsChildOf(obj.transform)) continue;

            obj.SetActive(isActive);
        }
    }
}