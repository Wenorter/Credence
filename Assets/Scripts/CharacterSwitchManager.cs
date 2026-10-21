using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitchManager : MonoBehaviour
{
    public const int NoneIndex = -1;

    [Serializable]
    public struct CharacterEntry
    {
        public string name;
        public List<MonoBehaviour> behaviours;
        public List<GameObject> gameObjects;
    }

    [SerializeField] private InputActionReference switchAction;
    [SerializeField] private List<CharacterEntry> characters = new List<CharacterEntry>();

    [Tooltip("Character enabled on Awake. -1 = start with every character off.")]
    [SerializeField] private int startIndex;

    private int _currentIndex = NoneIndex;

    public int CurrentIndex => _currentIndex;
    public int Count => characters.Count;
    public IReadOnlyList<CharacterEntry> Characters => characters;

    public string CurrentName => _currentIndex >= 0 && _currentIndex < characters.Count ? characters[_currentIndex].name : string.Empty;

    private void Awake()
    {
        TurnOffAll();

        if (characters.Count == 0 || startIndex < 0) return;

        SwitchTo(Mathf.Clamp(startIndex, 0, characters.Count - 1));
    }

    private void OnEnable()
    {
        if (!switchAction || switchAction.action == null) return;

        switchAction.action.performed += OnSwitchPerformed;
        switchAction.action.Enable();
    }

    private void OnDisable()
    {
        if (!switchAction || switchAction.action == null) return;

        switchAction.action.performed -= OnSwitchPerformed;
        switchAction.action.Disable();
    }

    private void OnSwitchPerformed(InputAction.CallbackContext context)
    {
        SwitchToNext();
    }

    public void SwitchToNext()
    {
        if (characters.Count == 0) return;

        int nextIndex = (_currentIndex + 1) % characters.Count;
        if (nextIndex == _currentIndex) return;

        SwitchTo(nextIndex);
    }

    public void SwitchTo(int index)
    {
        if (index < 0 || index >= characters.Count)
        {
            TurnOffAll();
            return;
        }

        for (int i = 0; i < characters.Count; i++)
        {
            if (i != index) SetCharacterEnabled(i, false);
        }

        _currentIndex = index;
        SetCharacterEnabled(index, true);
    }

    public void TurnOffAll()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            SetCharacterEnabled(i, false);
        }

        _currentIndex = NoneIndex;
    }

    public bool IsCharacterEnabled(int index)
    {
        if (index < 0 || index >= characters.Count) return false;

        CharacterEntry character = characters[index];
        bool hasAny = false;

        if (character.gameObjects != null)
        {
            foreach (GameObject obj in character.gameObjects)
            {
                if (!obj || transform.IsChildOf(obj.transform)) continue;
                hasAny = true;
                if (!obj.activeSelf) return false;
            }
        }

        if (character.behaviours != null)
        {
            foreach (MonoBehaviour behaviour in character.behaviours)
            {
                if (!behaviour || behaviour == this) continue;
                hasAny = true;
                if (!behaviour.enabled) return false;
            }
        }

        return hasAny;
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

    private void SetBehavioursEnabled(List<MonoBehaviour> behaviours, bool isEnabled)
    {
        if (behaviours == null) return;

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (!behaviour || behaviour == this) continue;

            behaviour.enabled = isEnabled;
        }
    }

    private void SetGameObjectsActive(List<GameObject> gameObjects, bool isActive)
    {
        if (gameObjects == null) return;

        foreach (GameObject obj in gameObjects)
        {
            if (!obj) continue;

            if (transform.IsChildOf(obj.transform)) continue;

            obj.SetActive(isActive);
        }
    }
}