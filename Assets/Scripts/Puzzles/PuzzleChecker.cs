using UnityEngine;

public class PuzzleChecker : MonoBehaviour
{
    [SerializeField] private PuzzleGenerator _generator;
    private int _piecesCorrect;

    public void SnapAndDisableIfCorrect(Transform piece)
    {
        int pieceIndex = _generator.Pieces.IndexOf(piece);
        int col = pieceIndex % _generator.Dimensions.x;
        int row = pieceIndex / _generator.Dimensions.x;

        Vector3 targetPosition = new(
            (-_generator.Width * _generator.Dimensions.x / 2) + (_generator.Width * col) + (_generator.Width / 2),
            (-_generator.Height * _generator.Dimensions.y / 2) + (_generator.Height * row) + (_generator.Height / 2),
            0f);

        if (Vector3.Distance(piece.localPosition, targetPosition) < (_generator.Width / 2))
        {
            piece.localPosition = targetPosition;
            piece.GetComponent<BoxCollider>().enabled = false;
            _piecesCorrect++;
            if (_piecesCorrect == _generator.Pieces.Count)
                GameController.Instance.GameWin();
        }
    }
}
