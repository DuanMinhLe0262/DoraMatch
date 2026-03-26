using System.Collections.Generic;
using UnityEngine;

public class BoardData
{
  public int PlayableRows { get; private set; }
  public int PlayableCols { get; private set; }
  public int[,] Cells { get; private set; }

  public void Generate(int rows, int cols, int iconTypeCount)
  {
    PlayableRows = rows;
    PlayableCols = cols;

    int playableCellCount = rows * cols;
    if (playableCellCount % 2 != 0)
    {
      Debug.LogError("So o choi phai la so chan");
      return;
    }

    if (iconTypeCount <= 0)
    {
      Debug.LogError("iconTypeCount phai lon hon 0");
      return;
    }

    int totalRows = rows + 2;
    int totalCols = cols + 2;
    Cells = new int[totalRows, totalCols];

    List<int> values = new();
    int pairCount = playableCellCount / 2;

    for (int i = 0; i < pairCount; i++)
    {
      int iconId = (i % iconTypeCount) + 1;
      values.Add(iconId);
      values.Add(iconId);
    }

    Shuffle(values);

    int index = 0;

    for (int r = 1; r <= rows; r++)
    {
      for (int c = 1; c <= cols; c++)
      {
        Cells[r, c] = values[index];
        index++;
      }
    }
  }

  public void ClearCell(int row, int col)
  {
    if (Cells == null) return;
    if (row < 0 || row >= Cells.GetLength(0)) return;
    if (col < 0 || col >= Cells.GetLength(1)) return;

    Cells[row, col] = 0;
  }

  public bool IsCleared()
  {
    if (Cells == null)
      return false;

    for (int r = 1; r <= PlayableRows; r++)
    {
      for (int c = 1; c <= PlayableCols; c++)
      {
        if (Cells[r, c] != 0)
          return false;
      }
    }

    return true;
  }

  private void Shuffle(List<int> values)
  {
    for (int i = 0; i < values.Count; i++)
    {
      int rand = Random.Range(i, values.Count);
      (values[i], values[rand]) = (values[rand], values[i]);
    }
  }
}
