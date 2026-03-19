using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Texture2D> _imageTextures;
    [SerializeField] private Transform _levelSelectionPanel;
    [SerializeField] private Image _levelSelectPrefab;
    [SerializeField] private GameObject reaction;
    [SerializeField] private PuzzleGenerator _generator;

    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        reaction.SetActive(true);
        foreach (Texture2D texture in _imageTextures)
        {
            Image image = Instantiate(_levelSelectPrefab, _levelSelectionPanel);
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            _levelSelectionPanel.gameObject.SetActive(false);
            _generator.GeneratePuzzle(texture);
        }
    }
}

