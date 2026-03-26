using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
  [Header("Board Size")]
  [SerializeField] private int playableRows = 8;
  [SerializeField] private int playableCols = 12;

  [Header("References")]
  [SerializeField] private TileFactory tileFactory;
  [SerializeField] private Sprite[] doraemonSprites;
  [SerializeField] private LineDrawer lineDrawer;
  [SerializeField] private GridLayoutGroup boardGrid;


  [Header("Levels")]
  [SerializeField] private LevelConfig[] levels;
  [SerializeField] private int maxShuffleAttempts = 20;

  private int currentLevelIndex = 0;
  private LevelConfig currentLevel;
  private BoardRule currentRule = BoardRule.None;

  private BoardData boardData;
  private TileView[,] tileViews;

  private TileView firstSelected;
  private TileView secondSelected;

  private PathFinder pathFinder;
  private Canvas boardCanvas;

  private void Start()
  {
    if (tileFactory == null)
    {
      return;
    }

    if (doraemonSprites == null || doraemonSprites.Length == 0)
    {
      return;
    }

    pathFinder = new PathFinder();
    StartLevel(0);
  }


  private void CreateViews()
  {
    int rows = playableRows + 2;
    int cols = playableCols + 2;

    tileViews = new TileView[rows, cols];

    for (int r = 1; r <= playableRows; r++)
    {
      for (int c = 1; c <= playableCols; c++)
      {
        int iconId = boardData.Cells[r, c];
        Sprite sprite = doraemonSprites[iconId - 1];

        TileView tile = tileFactory.Create(this, r, c, iconId, sprite);
        if (tile == null)
          return;

        tileViews[r, c] = tile;
      }
    }
  }

  public void OnTileClicked(TileView clicked)
  {
    if (firstSelected == null)
    {
      firstSelected = clicked;
      firstSelected.SetSelected(true);
      return;
    }

    if (clicked == firstSelected)
    {
      firstSelected.SetSelected(false);
      firstSelected = null;
      return;
    }

    secondSelected = clicked;
    secondSelected.SetSelected(true);

    StartCoroutine(TryMatch());
  }

  private IEnumerator TryMatch()
  {
    if (firstSelected == null || secondSelected == null)
    {
      ResetSelection();
      yield break;
    }

    List<Vector2Int> path = pathFinder.FindShortestPath(
    boardData,
    firstSelected.Row, firstSelected.Col,
    secondSelected.Row, secondSelected.Col
);


    if (path != null)
    {
      if (lineDrawer != null)
      {
        List<Vector3> points = ConvertPathToWorldPoints(path);
        lineDrawer.DrawLine(points);

        yield return new WaitForSeconds(lineDrawer.ShowTime);
      }

      RemovePair(firstSelected, secondSelected);

      if (boardData.IsCleared())
      {
        GoToNextLevel();
      }
      else if (!HasAnyAvailableMatch())
      {
        ShuffleBoardUntilMatch();
      }
    }

    ResetSelection();
  }


  private void RemovePair(TileView a, TileView b)
  {
    boardData.ClearCell(a.Row, a.Col);
    boardData.ClearCell(b.Row, b.Col);

    boardData.ApplyRule(currentRule);
    RefreshViewsFromBoard();

  }

  private void RefreshViewsFromBoard()
  {
    for (int r = 1; r <= playableRows; r++)
    {
      for (int c = 1; c <= playableCols; c++)
      {
        TileView tile = tileViews[r, c];
        int iconId = boardData.Cells[r, c];

        if (iconId == 0)
        {
          tile.SetEmpty();
        }
        else
        {
          tile.SetIcon(iconId, doraemonSprites[iconId - 1]);
        }
      }
    }
  }

  private void EnsureAvailableMatch()
  {
    if (boardData == null || boardData.IsCleared())
      return;

    if (!HasAnyAvailableMatch())
      ShuffleBoardUntilMatch();
  }

  private void ShuffleBoardUntilMatch()
  {
    if (boardData == null || boardData.IsCleared())
      return;

    for (int attempt = 0; attempt < maxShuffleAttempts; attempt++)
    {
      boardData.ShuffleRemainingTiles();
      RefreshViewsFromBoard();

      if (HasAnyAvailableMatch())
      {
        return;
      }
    }

  }


  private void ResetSelection()
  {
    if (firstSelected != null) firstSelected.SetSelected(false);
    if (secondSelected != null) secondSelected.SetSelected(false);

    firstSelected = null;
    secondSelected = null;
  }

  private bool IsBoardCleared()
  {
    return boardData != null && boardData.IsCleared();
  }

  private List<Vector3> ConvertPathToWorldPoints(List<Vector2Int> path)
  {
    List<Vector3> points = new(path.Count);

    foreach (Vector2Int cell in path)
    {
      points.Add(GetCellWorldPosition(cell.x, cell.y));
    }

    return points;
  }

  private Vector3 GetCellWorldPosition(int row, int col)
  {
    TileView existingTile = GetTileViewSafe(row, col);
    if (existingTile != null)
      return ConvertUIToLineWorld(existingTile.GetWorldPos());

    Vector3 anchor = GetAnchorTileWorldPosition();
    Vector3 horizontalStep = GetHorizontalStep();
    Vector3 verticalStep = GetVerticalStep();

    int rowOffset = row - 1;
    int colOffset = col - 1;

    Vector3 uiPoint = anchor + horizontalStep * colOffset + verticalStep * rowOffset;
    return ConvertUIToLineWorld(uiPoint);
  }

  private TileView GetTileViewSafe(int row, int col)
  {
    if (tileViews == null)
      return null;

    if (row < 0 || row >= tileViews.GetLength(0) || col < 0 || col >= tileViews.GetLength(1))
      return null;

    return tileViews[row, col];
  }

  private Vector3 GetAnchorTileWorldPosition()
  {
    TileView anchorTile = GetTileViewSafe(1, 1);
    return anchorTile != null ? anchorTile.GetWorldPos() : transform.position;
  }

  private Vector3 GetHorizontalStep()
  {
    TileView leftTile = GetTileViewSafe(1, 1);
    TileView rightTile = GetTileViewSafe(1, 2);

    if (leftTile != null && rightTile != null)
      return rightTile.GetWorldPos() - leftTile.GetWorldPos();

    return Vector3.right;
  }

  private Vector3 GetVerticalStep()
  {
    TileView topTile = GetTileViewSafe(1, 1);
    TileView bottomTile = GetTileViewSafe(2, 1);

    if (topTile != null && bottomTile != null)
      return bottomTile.GetWorldPos() - topTile.GetWorldPos();

    return Vector3.down;
  }

  private Vector3 ConvertUIToLineWorld(Vector3 uiPoint)
  {
    if (lineDrawer == null)
      return uiPoint;

    Camera uiCamera = GetUICamera();
    Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, uiPoint);

    Camera renderCamera = Camera.main;
    if (renderCamera == null)
      return uiPoint;

    float distanceToCamera = Mathf.Abs(lineDrawer.transform.position.z - renderCamera.transform.position.z);
    Vector3 worldPoint = renderCamera.ScreenToWorldPoint(new Vector3(screenPoint.x,
                                                                     screenPoint.y,
                                                                     distanceToCamera));
    worldPoint.z = lineDrawer.transform.position.z;
    return worldPoint;
  }

  private Canvas FindBoardCanvas()
  {
    TileView firstTile = GetTileViewSafe(1, 1);
    if (firstTile != null)
    {
      Canvas tileCanvas = firstTile.GetComponentInParent<Canvas>();
      if (tileCanvas != null)
        return tileCanvas.rootCanvas;
    }

    Canvas anyCanvas = FindFirstObjectByType<Canvas>();
    return anyCanvas != null ? anyCanvas.rootCanvas : null;
  }

  private Camera GetUICamera()
  {
    if (boardCanvas == null)
      return null;

    return boardCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : boardCanvas.worldCamera;
  }

  private void StartLevel(int levelIndex)
  {
    if (levels == null || levels.Length == 0)
    {
      return;
    }

    if (levelIndex < 0 || levelIndex >= levels.Length)
    {
      return;
    }

    currentLevelIndex = levelIndex;
    currentLevel = levels[currentLevelIndex];
    currentRule = currentLevel.rule;

    boardData = new BoardData();
    boardData.Generate(currentLevel.rows, currentLevel.cols, currentLevel.iconTypeCount);

    playableRows = currentLevel.rows;
    playableCols = currentLevel.cols;

    UpdateBoardLayout();
    ClearOldViews();
    CreateViews();
    boardCanvas = FindBoardCanvas();
    EnsureAvailableMatch();
  }

  private void GoToNextLevel()
  {
    int nextLevelIndex = currentLevelIndex + 1;

    if (nextLevelIndex >= levels.Length)
    {
      return;
    }

    StartLevel(nextLevelIndex);
  }

  private void ClearOldViews()
  {
    if (tileViews == null)
      return;

    for (int r = 0; r < tileViews.GetLength(0); r++)
    {
      for (int c = 0; c < tileViews.GetLength(1); c++)
      {
        if (tileViews[r, c] != null)
        {
          Destroy(tileViews[r, c].gameObject);
        }
      }
    }

    tileViews = null;
  }

  private void UpdateBoardLayout()
  {
    if (boardGrid == null)
      return;

    boardGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    boardGrid.constraintCount = playableCols;
  }


  private bool HasAnyAvailableMatch()
  {
    if (boardData == null || boardData.Cells == null || pathFinder == null)
      return false;

    for (int r1 = 1; r1 <= playableRows; r1++)
    {
      for (int c1 = 1; c1 <= playableCols; c1++)
      {
        int iconId = boardData.Cells[r1, c1];
        if (iconId == 0)
          continue;

        for (int r2 = r1; r2 <= playableRows; r2++)
        {
          int startCol = (r2 == r1) ? c1 + 1 : 1;

          for (int c2 = startCol; c2 <= playableCols; c2++)
          {
            if (boardData.Cells[r2, c2] != iconId)
              continue;

            if (pathFinder.CanConnect(boardData, r1, c1, r2, c2))
              return true;
          }
        }
      }
    }

    return false;
  }
}
