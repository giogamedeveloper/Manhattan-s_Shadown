using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    [SerializeField] private Transform gameHolder;
    [SerializeField] private Transform piecePrefab;
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;
    [SerializeField] private int difficulty = 4;
    [SerializeField] private GameObject _player;

    private float _height;
    private float _width;
    private float _distanceToForward = 2f;

    public List<Transform> Pieces { get; private set; }
    public Vector2Int Dimensions { get; private set; }
    public float Width => _width;
    public float Height => _height;
    public Transform GameHolder => gameHolder;

    public void GeneratePuzzle(Texture2D texture)
    {
        Vector3 rotation = _player.transform.rotation.eulerAngles;
        var quaternion = gameHolder.gameObject.transform.rotation;
        quaternion.eulerAngles = rotation;
        gameHolder.gameObject.transform.rotation = quaternion;
        gameHolder.gameObject.transform.position =
            new Vector3(_player.transform.position.x, gameHolder.transform.position.y, _player.transform.position.z) +
            _player.transform.forward * _distanceToForward;

        Pieces = new List<Transform>();
        Dimensions = GetDimensions(texture, difficulty);
        CreatePuzzlePieces(texture);
        Scatter();
        UpdateBorder();
    }

    private Vector2Int GetDimensions(Texture2D texture, int diff)
    {
        Vector2Int dimensions = Vector2Int.zero;
        if (texture.width < texture.height)
        {
            dimensions.x = diff;
            dimensions.y = (diff * texture.height) / texture.width;
        }
        else
        {
            dimensions.x = (diff * texture.width) / texture.height;
            dimensions.y = diff;
        }
        return dimensions;
    }

    private void CreatePuzzlePieces(Texture2D texture)
    {
        _height = 1f / Dimensions.y;
        float aspect = (float)texture.width / texture.height;
        _width = aspect / Dimensions.x;

        for (int row = 0; row < Dimensions.y; row++)
        {
            for (int col = 0; col < Dimensions.x; col++)
            {
                Transform piece = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity, gameHolder);
                piece.localPosition = Vector3.zero;
                piece.localRotation = Quaternion.Euler(Vector3.zero);
                piece.localScale = new Vector3(_width, _height, 1f);
                piece.name = $"Piece {(row * Dimensions.x) + col}";
                Pieces.Add(piece);

                float width1 = 1f / Dimensions.x;
                float height1 = 1f / Dimensions.y;
                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));

                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = uv;
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
            }
        }
    }

    private void Scatter()
    {
        foreach (Transform piece in Pieces)
        {
            float x = Random.Range(-offsetX, offsetX);
            float y = Random.Range(-offsetY, offsetY);
            piece.localPosition = new Vector3(x, y, 0);
        }
    }

    private void UpdateBorder()
    {
        LineRenderer lineRenderer = gameHolder.GetComponent<LineRenderer>();
        float halfWidth = (_width * Dimensions.x) / 2f;
        float halfHeight = (_height * Dimensions.y) / 2f;
        lineRenderer.SetPosition(0, new Vector3(-halfWidth, halfHeight, 0f));
        lineRenderer.SetPosition(1, new Vector3(halfWidth, halfHeight, 0f));
        lineRenderer.SetPosition(2, new Vector3(halfWidth, -halfHeight, 0f));
        lineRenderer.SetPosition(3, new Vector3(-halfWidth, -halfHeight, 0f));
        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .1f;
        lineRenderer.enabled = true;
    }
}