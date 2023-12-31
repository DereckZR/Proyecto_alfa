using UnityEngine;

public enum chessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}
public class ChessPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public chessPieceType type;
    private Vector3 desiredPosition;
    private Vector3 desiredScale;

    private void Awake() {
        desiredScale = new Vector3 (20, 20, transform.localScale.z);
    }
    private void Update() {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime *  10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime *  10);
    }

    public virtual void Setposition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if(force)
            transform.position = desiredPosition;
    }
    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if(force)
            transform.localScale = desiredScale;
    }
}
