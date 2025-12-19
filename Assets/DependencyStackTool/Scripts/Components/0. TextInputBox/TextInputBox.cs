
// [Summary] (By Wessel)
//
// This is a prototype script for typing text.
//

using System.Collections;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

public class TextInputBox : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private int _maxChars = 24;
    [SerializeField] private float _typingResetDelay = 0.5f;
    [SerializeField] private float _caretBlinkDelay = 0.5f;
    [SerializeField] private float _caretToTextOffset = 20f;
    [SerializeField] private string _emptyText = "SaveName..";
    [SerializeField] private bool _isFocussedOnEnable = true;
    [SerializeField] private bool _isSpaceAllowed = true;

    [Header("Outside Checking")]
    [SerializeField] private RectTransform _insideRectTransform;

    [Header("Text Caret")]
    [SerializeField] private RectTransform _caretRectTransform;

    [Header("Text Output")]
    [SerializeField] private TMP_Text _outputText;

    [Header("UI Input")]
    [SerializeField] private Button _focusButton;

    // Currents
    private int _caretIndex = 0;
    private string _typedText = "";

    // States
    [SerializeField] private bool _isTyping = false;
    [SerializeField] private bool _isBlinking = false;

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered = false;


    // ----------------- Functions -----------------


    #region Gettter Functions

    public string GetTypedText() => _typedText;

    #endregion

    #region Setter Functions

    public void SetTypedText(string newInputText)
    {
        _typedText = newInputText;
        ShowTypedText();
    }

    #endregion

    #region OnEnable Functions

    private void OnEnable()
    {
        FocusEnabler();
    }

    private void RegisterInputHandlerRelated()
    {
        // Safety check.
        if (_isRegistered)
            return;
        _isRegistered = true;

        // Safely register input handler related things.
        _inputHandler.OnCharTyped += CharTypedResult;
        _inputHandler.OnAnyMouseStarted += OutsideCheck;
    }

    #endregion

    #region OnDisable Functions

    private void OnDisable()
    {
        UnfocusTriggered();
    }

    private void UnregisterInputHandlerRelated()
    {
        // Safety check.
        if (!_isRegistered)
            return;
        _isRegistered = false;

        // Safely unregister input handler related things.
        _inputHandler.OnCharTyped -= CharTypedResult;
        _inputHandler.OnAnyMouseStarted -= OutsideCheck;
    }

    #endregion

    #region Focus Functions

    public void FocusEnabler()
    {
        if (_isFocussedOnEnable)
            FocusTriggered();
        else
            UnfocusTriggered();
    }

    public void FocusTriggered()
    {
        Debug.Log("FocusTriggered");

        RegisterInputHandlerRelated();
        UpdateCaretPosition();
        StopAllCoroutines();

        _isTyping = false;
        _isBlinking = true;

        if (_caretRectTransform.gameObject.activeSelf == false)
            _caretRectTransform.gameObject.SetActive(true);

        StartCoroutine(CaretBlink());
    }

    private void UnfocusTriggered()
    {
        Debug.Log("UnfocusTriggered");

        UnregisterInputHandlerRelated();
        StopAllCoroutines();

        _isTyping = false;
        _isBlinking = false;

        if (_caretRectTransform.gameObject.activeSelf == true)
            _caretRectTransform.gameObject.SetActive(false);
    }

    #endregion

    #region Typing Functions

    private void CharTypedResult(char inputChar)
    {
        //Debug.Log($"Input Received: {inputChar}");

        HandleTyping(inputChar);
    }

    private void HandleTyping(char inputChar)
    {
        if (inputChar == '\b') // Backspace
        {
            if (_typedText.Length > 0)
            {
                _typedText = _typedText.Substring(0, _typedText.Length - 1);
                ShowTypedText();
                TypedTriggered();
            }
        }
        else if (inputChar == '\u007F') // Delete
        {
            _typedText = "";
            ShowTypedText();
            TypedTriggered();
        }
        else if (inputChar == '\n' || inputChar == '\r') // Enter / Return
        {
            UnfocusTriggered();
        }
        else if (char.IsControl(inputChar)) // Ignore all other control characters (Escape, Tab, etc.)
        {
            // Do nothing
        }
        else if (inputChar == ' ')
        {
            if (_isSpaceAllowed)
                AllowedCharacter(inputChar);

            // Else do nothing
        }
        else // Normal printable character
        {
            AllowedCharacter(inputChar);
        }
    }

    private void AllowedCharacter(char inputChar)
    {
        if (_typedText.Length < _maxChars)
        {
            _typedText += inputChar;
            ShowTypedText();
            TypedTriggered();
        }
    }

    private void TypedTriggered()
    {
        UpdateCaretPosition();
        StopAllCoroutines();

        _isBlinking = false;
        _isTyping = true;

        if (_caretRectTransform.gameObject.activeSelf == false)
            _caretRectTransform.gameObject.SetActive(true);

        StartCoroutine(ResetTyping());
    }

    private void ShowTypedText()
    {
        if (_typedText.Length == 0)
        {
            _outputText.fontStyle = FontStyles.Italic;
            _outputText.text = _emptyText;
        }

        else
        {
            _outputText.fontStyle = FontStyles.Normal;
            _outputText.text = _typedText;
        }

    }

    #endregion

    #region Caret Functions

    private void UpdateCaretPosition()
    {
        _caretIndex = _typedText.Length;
        Debug.Log($"Updating Caret Position 1: {_caretIndex}");

        _outputText.ForceMeshUpdate();
        var textInfo = _outputText.textInfo;
        int charCount = textInfo.characterCount;

        Vector2 localPos;


        if (_caretIndex == 0)
        {
            Debug.Log($"Updating Caret Position 2: {_caretIndex}");

            // Start of text
            localPos = new Vector2(_caretToTextOffset, 0); // X=0, Y=baseline will be 0
        }
        else
        {
            Debug.Log($"Updating Caret Position 3: {_caretIndex}");

            var charInfo = textInfo.characterInfo[_caretIndex - 1];
            var line = textInfo.lineInfo[charInfo.lineNumber];

            // Position caret after character, aligned to baseline
            float x = charInfo.xAdvance;
            //float x = caretIndex * 20f; // Approximate character width
            float y = 0;

            localPos = new Vector2(x, y);
        }

        _caretRectTransform.anchoredPosition = localPos;
    }

    private IEnumerator CaretBlink()
    {
        while (!_isTyping && _isBlinking)
        {
            yield return new WaitForSeconds(_caretBlinkDelay);
            _caretRectTransform.gameObject.SetActive(!_caretRectTransform.gameObject.activeSelf);
        }
    }

    private IEnumerator ResetTyping()
    {
        if (_isTyping)
        {
            _isBlinking = false;
            yield return new WaitForSeconds(_typingResetDelay);
            _isTyping = false;

            if (!_isBlinking)
            {
                _isBlinking = true;
                StartCoroutine(CaretBlink());
            }
        }
    }

    #endregion

    // Reworking below with EventSystem!

    #region OutsideCheck Functions

    private void OutsideCheck()
    {
        Vector2 position = _inputHandler.GetMousePosition;
        if (!RectTransformUtility.RectangleContainsScreenPoint(_insideRectTransform, position))
            UnfocusTriggered();
        else
            FocusTriggered();
    }

    #endregion

}
