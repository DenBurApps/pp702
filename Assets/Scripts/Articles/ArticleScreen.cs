using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class ArticleScreen : MonoBehaviour
{
    [SerializeField] private List<ArticlePlane> _articlePlanes;
    [SerializeField] private Settings _settings;
    [SerializeField] private OpenArticleScreen _openArticleScreen;

    public event Action<ArticlePlane> PlaneOpened;
    public event Action BackClicked;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _settings.ArticlesOpened += EnableScreen;
        _openArticleScreen.BackClicked += EnableScreen;

        foreach (var plane in _articlePlanes)
        {
            plane.Opened += OnArticleOpened;
        }
    }

    private void OnDisable()
    {
        _settings.ArticlesOpened -= EnableScreen;
        _openArticleScreen.BackClicked -= EnableScreen;

        foreach (var plane in _articlePlanes)
        {
            plane.Opened -= OnArticleOpened;
        }
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

    private void EnableScreen()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    private void OnArticleOpened(ArticlePlane plane)
    {
        PlaneOpened?.Invoke(plane);
        _screenVisabilityHandler.DisableScreen();
    }
}
