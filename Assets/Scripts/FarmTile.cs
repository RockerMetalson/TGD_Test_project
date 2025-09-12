using UnityEngine;

public class FarmTile : MonoBehaviour
{
    public enum TileState { Grass, Plowed, Planted }
    public TileState currentState = TileState.Grass;

    public void Plow()
    {
        if (currentState == TileState.Grass)
        {
            currentState = TileState.Plowed;
            GetComponent<Renderer>().material.color = new Color(0.4f, 0.25f, 0.1f); // Brown
        }
    }
}
