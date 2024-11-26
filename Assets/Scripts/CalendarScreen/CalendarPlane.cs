using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalendarPlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _types;
    [SerializeField] private PhotosController _photosController;
    
    public bool IsActive { get; private set; }

    public void Enable(string name, string types, byte[] image)
    {
        gameObject.SetActive(true);
        _name.text = name;
        _types.text = types;

        if (image != null)
        {
            _photosController.SetPhotos(image);
        }
        else
        {
            _photosController.ResetPhotos();
        }
        
        IsActive = true;
    }

    public void Disable()
    {
        _name.text = string.Empty;
        _types.text = string.Empty;
        gameObject.SetActive(false);
        IsActive = false;
    }
    
}
