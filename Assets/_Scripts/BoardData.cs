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
      return;
    }

    if (iconTypeCount <= 0)
    {
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

  public void ApplyRule(BoardRule rule)
  {
    switch (rule)
    {
      case BoardRule.CollapseDown:
        CollapseDown();
        break;

      case BoardRule.CollapseUp:
        CollapseUp();
        break;

      case BoardRule.ShiftLeft:
        ShiftLeft();
        break;
      case BoardRule.ShiftRight:
        ShiftRight();
        break;
    }
  }

  public void ShuffleRemainingTiles()
  {
    if (Cells == null)
      return;

    List<int> remainingValues = new();

    for (int r = 1; r <= PlayableRows; r++)
    {
      for (int c = 1; c <= PlayableCols; c++)
      {
        if (Cells[r, c] != 0)
          remainingValues.Add(Cells[r, c]);
      }
    }

    if (remainingValues.Count <= 1)
      return;

    Shuffle(remainingValues);

    int index = 0;
    for (int r = 1; r <= PlayableRows; r++)
    {
      for (int c = 1; c <= PlayableCols; c++)
      {
        if (Cells[r, c] != 0)
        {
          Cells[r, c] = remainingValues[index];
          index++;
        }
      }
    }
  }

  private void CollapseDown()
  {
    for (int c = 1; c <= PlayableCols; c++)
    {
      int writeRow = PlayableRows;

      for (int r = PlayableRows; r >= 1; r--)
      {
        if (Cells[r, c] != 0)
        {
          Cells[writeRow, c] = Cells[r, c];

          if (writeRow != r)
            Cells[r, c] = 0;

          writeRow--;
        }
      }

      for (int r = writeRow; r >= 1; r--)
      {
        Cells[r, c] = 0;
      }
    }
  }

  private void CollapseUp()
  {
    for (int c = 1; c <= PlayableCols; c++)
    {
      int writeRow = 1;

      for (int r = 1; r <= PlayableRows; r++)
      {
        if (Cells[r, c] != 0)
        {
          Cells[writeRow, c] = Cells[r, c];

          if (writeRow != r)
            Cells[r, c] = 0;

          writeRow++;
        }
      }

      for (int r = writeRow; r <= PlayableRows; r++)
      {
        Cells[r, c] = 0;
      }
    }
  }

  private void ShiftLeft()
  {
    for (int r = 1; r <= PlayableRows; r++)
    {
      int writeCol = 1;

      for (int c = 1; c <= PlayableCols; c++)
      {
        if (Cells[r, c] != 0)
        {
          Cells[r, writeCol] = Cells[r, c];

          if (writeCol != c)
            Cells[r, c] = 0;

          writeCol++;
        }
      }

      for (int c = writeCol; c <= PlayableCols; c++)
      {
        Cells[r, c] = 0;
      }
    }
  }

  private void ShiftRight()
  {
    for (int r = 1; r <= PlayableRows; r++)
    {
      int writeCol = PlayableCols;

      for (int c = PlayableCols; c >= 1; c--)
      {
        if (Cells[r, c] != 0)
        {
          Cells[r, writeCol] = Cells[r, c];

          if (writeCol != c)
            Cells[r, c] = 0;

          writeCol--;
        }
      }

      for (int c = writeCol; c >= 1; c--)
      {
        Cells[r, c] = 0;
      }
    }
  }

}
