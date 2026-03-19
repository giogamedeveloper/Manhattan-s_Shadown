using UnityEngine;

public class PuzzleInput : MonoBehaviour
{
    [SerializeField] private LayerMask piecesLayerMask;
    [SerializeField] private PuzzleGenerator _generator;
    [SerializeField] private PuzzleChecker _checker;

    private Transform _draggingPiece;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, piecesLayerMask))
                _draggingPiece = hit.transform;
        }

        if (_draggingPiece && Input.GetMouseButtonUp(0))
        {
            _checker.SnapAndDisableIfCorrect(_draggingPiece);
            _draggingPiece = null;
        }

        if (_draggingPiece)
        {
            float distance = (_generator.GameHolder.position - _mainCamera.transform.position).magnitude;
            _draggingPiece.position = _mainCamera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
        }
    }
}
