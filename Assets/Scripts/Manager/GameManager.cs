using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Elements")]
    [Range(2, 6)]
    [SerializeField]
    int _difficulty = 4;

    [SerializeField] Transform gameHolder;
    [SerializeField] Transform piecePrefab;

    [Header("UI Elements")]
    [SerializeField] List<Texture2D> _imageTextures;

    [SerializeField] Transform _levelSelectionPanel;
    [SerializeField] Image _levelSelectPrefab;
    public LayerMask piecesLayerMask;
    List<Transform> pieces;
    Vector2Int dimensions;
    float _height;
    float _width;
    [SerializeField] float offsetX;
    [SerializeField] private float offsetY;
    [SerializeField] private GameObject _player;
    float _distanceToForward = 2f;
    private Transform draggingPiece = null;
    private Vector3 offset;
    [SerializeField] GameObject reaction;
    private int piecesCorrect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reaction.SetActive(true);
        foreach (Texture2D texture in _imageTextures)
        {
            Image image = Instantiate(_levelSelectPrefab, _levelSelectionPanel);
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            //Assign button to action
            StartPuzzle(texture);
        }
    }

    void StartPuzzle(Texture2D _texture)
    {
        //setea con la rotación que tenga el player
        Vector3 rotation = _player.transform.rotation.eulerAngles;
        var quaternion = gameHolder.gameObject.transform.rotation;
        quaternion.eulerAngles = rotation;
        gameHolder.gameObject.transform.rotation = quaternion;
        gameHolder.gameObject.transform.position =
            new Vector3(_player.transform.position.x, gameHolder.transform.position.y, _player.transform.position.z) +
            _player.transform.forward * _distanceToForward;
        //Hide the UI
        _levelSelectionPanel.gameObject.SetActive(false);
        //We store the list of the transform
        pieces = new List<Transform>();
        //Calculate the size of each puzzles piece, based on a difficulty settings
        dimensions = GetDimensions(_texture, _difficulty);
        //Create the pieces of  the correct size with the correct texture
        CreatePuzzlePieces(_texture);
        //Place the piece randomly into visible area
        Scatter();
        UpdateBorder();
    }


    Vector2Int GetDimensions(Texture2D jigsawTexture, int difficulty)
    {
        Vector2Int dimensions = Vector2Int.zero;
        //Difficulty is the number of pieces on the smallest texture dimension.
        //This helpos ensure the pieces are as square as possible
        if (jigsawTexture.width < jigsawTexture.height)
        {
            dimensions.x = difficulty;
            dimensions.y = (difficulty * jigsawTexture.height) / jigsawTexture.width;
        }
        else
        {
            dimensions.x = (difficulty * jigsawTexture.width) / jigsawTexture.height;
            dimensions.y = difficulty;
        }
        return dimensions;

    }

    void CreatePuzzlePieces(Texture2D jigsawTexture)
    {
        _height = 1f / dimensions.y;
        float aspect = (float)jigsawTexture.width / jigsawTexture.height;
        _width = aspect / dimensions.x;

        for (int row = 0; row < dimensions.y; row++)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                //Create the piece in the right location on the right size.
                Transform piece = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity, gameHolder);
                piece.localPosition = Vector3.zero;
                piece.localRotation = Quaternion.Euler(Vector3.zero);

                piece.localScale = new Vector3(_width, _height, 1f);
                // We don't have to name them, but always useful for debugging.
                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);

                float width1 = 1f / dimensions.x;
                float height1 = 1f / dimensions.y;


                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));

                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = uv;
                //Update the textuire of the piece
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", jigsawTexture);

            }
        }

    }

    private void Scatter()
    {
        //Place each piece randomly in the visible area
        foreach (Transform piece in pieces)
        {
            float x = Random.Range(-offsetX, offsetX);
            float y = Random.Range(-offsetY, offsetY);
            piece.localPosition = new Vector3(x, y, 0);
        }
    }

    void UpdateBorder()
    {
        LineRenderer lineRenderer = gameHolder.GetComponent<LineRenderer>();
        //Calculate half sizes to simplify the code
        float halfWidth = (_width * dimensions.x) / 2f;
        float halfHeight = (_height * dimensions.y) / 2f;

        //We want the border to be behind the pieces
        float borderZ = 0f;

        //Set border vertices, starting top left, going clockwise
        lineRenderer.SetPosition(0, new Vector3(-halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(1, new Vector3(halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(2, new Vector3(halfWidth, -halfHeight, borderZ));
        lineRenderer.SetPosition(3, new Vector3(-halfWidth, -halfHeight, borderZ));

        //Set the thickness of border line
        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .1f;

        //Show the border line
        lineRenderer.enabled = true;
    }

    // Update is called once per frame

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = default;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20f, piecesLayerMask))
            {
                draggingPiece = hit.transform;
            }
        }

        if (draggingPiece && Input.GetMouseButtonUp(0))
        {
            SnapAndDisableIfCorrect();
            draggingPiece = null;
        }

        if (draggingPiece)
        {
            float distance = (gameHolder.transform.position - Camera.main.transform.position).magnitude;
            Vector3 newPosition =
                Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
            draggingPiece.position = newPosition;
        }
    }

    private void SnapAndDisableIfCorrect()
    {
        int pieceIndex = pieces.IndexOf(draggingPiece);
        int col = pieceIndex % dimensions.x;
        int row = pieceIndex / dimensions.x;

        Vector3 targetPosition = new((-_width * dimensions.x / 2) + (_width * col) + (_width / 2),
            (-_height * dimensions.y / 2) + (_height * row) + (_height / 2), 0f);

        if (Vector3.Distance(draggingPiece.localPosition, targetPosition) < (_width / 2))
        {
            draggingPiece.localPosition = targetPosition;
            draggingPiece.GetComponent<BoxCollider>().enabled = false;
            piecesCorrect++;
            if (piecesCorrect == pieces.Count)
            {
                GameController.Instance.GameWin();
            }
        }
    }
}
