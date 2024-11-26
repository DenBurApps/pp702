using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OpenArticleScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _content;
    [SerializeField] private ArticleScreen _articleScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _articleScreen.PlaneOpened += EnableScreen;
    }

    private void OnDisable()
    {
        _articleScreen.PlaneOpened -= EnableScreen;
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    private void EnableScreen(ArticlePlane plane)
    {
        _screenVisabilityHandler.EnableScreen();
        _name.text = plane.Name.text;
        _image.sprite = plane.Sprite;
        _content.text = plane.Content.text;
    }
}
