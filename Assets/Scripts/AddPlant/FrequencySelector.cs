using System;
using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrequencySelector : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor;
    
    [SerializeField] private SimpleScrollSnap _numberScrollSnap;
    [SerializeField] private SimpleScrollSnap _textScrollSnap;
    [SerializeField] private TMP_Text[] _numbersText;
    [SerializeField] private TMP_Text[] _wordsText;
    
    private string _number;
    private string _text;

    public event Action<string> TimesNumberInputed;
    public event Action<string> TimesNameInputed; 

    private void OnEnable()
    {
        _numberScrollSnap.OnPanelCentered.AddListener(SetNumber);
        _textScrollSnap.OnPanelCentered.AddListener(SetText);
        
    }

    private void OnDisable()
    {
        _numberScrollSnap.OnPanelCentered.RemoveListener(SetNumber);
        _textScrollSnap.OnPanelCentered.RemoveListener(SetText);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        Reset();
        gameObject.SetActive(false);
    }

    private void SetNumber(int start, int end)
    {
        _number = _numbersText[start].text;
        SetColorForSelected(_numbersText, start);
        TimesNumberInputed?.Invoke(_number);
    }

    private void SetText(int start, int end)
    {
        _text = _wordsText[start].text;
        SetColorForSelected(_wordsText, start);
        TimesNameInputed?.Invoke(_text);
    }

    private void InitializeTextFields()
    {
        for (int i = 0; i < _numbersText.Length; i++)
        {
            _numbersText[i].text = i.ToString("00");
        }

        for (int i = 0; i < _wordsText.Length; i++)
        {
            _wordsText[i].text = i.ToString("00");
        }

        SetColorForSelected(_numbersText, 0);
        SetColorForSelected(_wordsText, 0);
    }

    private void SetColorForSelected(TMP_Text[] texts, int selectedIndex)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].color = i == selectedIndex ? _selectedColor : _unselectedColor;
        }
    }

    private void Reset()
    {
        _numberScrollSnap.GoToPanel(0);
        _textScrollSnap.GoToPanel(0);
        
        _number = string.Empty;
        _text = string.Empty;
    }
}