using UnityEngine;

public class TileFactory : MonoBehaviour
{
  [SerializeField] private TileView tilePrefab;
  [SerializeField] private Transform parent;

  public TileView Create(BoardManager boardManager, int row, int col, int iconId, Sprite sprite)
  {
    if (tilePrefab == null)
    {
      return null;
    }

    TileView tile = parent != null
      ? Instantiate(tilePrefab, parent)
      : Instantiate(tilePrefab);

    tile.Init(boardManager, row, col, iconId, sprite);
    return tile;
  }
}
