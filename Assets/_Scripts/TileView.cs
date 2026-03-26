using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileView : MonoBehaviour, IPointerDownHandler
{
  [SerializeField] private Image iconImage;
  [SerializeField] private Image highlight;

  public int Row { get; private set; }
  public int Col { get; private set; }
  public int IconId { get; private set; }

  private BoardManager boardManager;
  public void Init(BoardManager boardManager, int row, int col, int iconId, Sprite sprite)
  {
    this.boardManager = boardManager;
    EnsureRaycastTarget();
    Row = row;
    Col = col;
    SetIcon(iconId, sprite);
    SetSelected(false);
  }

  public void SetIcon(int iconId, Sprite sprite)
  {
    IconId = iconId;
    iconImage.sprite = sprite;
    iconImage.enabled = iconId != 0;
  }

  public void SetSelected(bool isSelected)
  {
    if (highlight != null)
    {
      highlight.gameObject.SetActive(true);
      highlight.enabled = isSelected;
    }
  }

  public void SetEmpty()
  {
    IconId = 0;
    iconImage.sprite = null;
    iconImage.enabled = false;
    SetSelected(false);
  }

  public Vector3 GetWorldPos()
  {
    return transform.position;
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    if (IconId == 0 || boardManager == null)
      return;

    boardManager.OnTileClicked(this);
  }

  private void EnsureRaycastTarget()
  {
    Image rootImage = GetComponent<Image>();
    if (rootImage == null)
    {
      rootImage = gameObject.AddComponent<Image>();
      rootImage.color = new Color(1f, 1f, 1f, 0f);
    }

    rootImage.raycastTarget = true;

    if (iconImage != null)
      iconImage.raycastTarget = false;

    if (highlight != null)
      highlight.raycastTarget = false;
  }

}
