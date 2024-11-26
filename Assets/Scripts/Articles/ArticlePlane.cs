using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArticlePlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private Image _image;

    public TMP_Text Name => _nameText;
    public TMP_Text Content => _contentText;
    public Sprite Sprite => _image.sprite;

    public event Action<ArticlePlane> Opened;

    public void OpenArticle()
    {
        Opened?.Invoke(this);
    }
}
