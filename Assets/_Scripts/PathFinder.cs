using UnityEngine;
using System.Collections.Generic;

public class PathFinder
{
  public bool CanConnect(int[,] board, int r1, int c1, int r2, int c2)
  {
    return FindPath(board, r1, c1, r2, c2) != null;
  }

  public List<Vector2Int> FindPath(int[,] board, int r1, int c1, int r2, int c2)
  {
    if (r1 == r2 && c1 == c2) return null;
    if (board[r1, c1] != board[r2, c2]) return null;
    if (board[r1, c1] == 0 || board[r2, c2] == 0) return null;

    if (CheckStraight(board, r1, c1, r2, c2))
      return BuildPath(new Vector2Int(r1, c1), new Vector2Int(r2, c2));

    if (CheckOneTurn(board, r1, c1, r2, c2, out Vector2Int corner))
      return BuildPath(new Vector2Int(r1, c1), corner, new Vector2Int(r2, c2));

    if (CheckTwoTurn(board, r1, c1, r2, c2, out Vector2Int corner1, out Vector2Int corner2))
      return BuildPath(new Vector2Int(r1, c1), corner1, corner2, new Vector2Int(r2, c2));
    return null;
  }

  private bool CheckStraight(int[,] board, int r1, int c1, int r2, int c2)
  {
    if (r1 == r2)
      return ClearRow(board, r1, c1, c2);
    if (c1 == c2)
      return ClearCol(board, c1, r1, r2);

    return false;
  }
  private bool CheckOneTurn(int[,] board, int r1, int c1, int r2, int c2, out Vector2Int corner)
  {
    corner = default;
    if (board[r1, c2] == 0)
    {
      if (ClearRow(board, r1, c1, c2) && ClearCol(board, c2, r1, r2))
      {
        corner = new Vector2Int(r1, c2);
        return true;
      }
    }

    if (board[r2, c1] == 0)
    {
      if (ClearCol(board, c1, r1, r2) && ClearRow(board, r2, c1, c2))
      {
        corner = new Vector2Int(r2, c1);
        return true;
      }
    }
    return false;
  }

  private bool CheckTwoTurn(int[,] board, int r1, int c1, int r2, int c2, out Vector2Int corner1, out Vector2Int corner2)
  {
    corner1 = default;
    corner2 = default;

    int rows = board.GetLength(0);
    int cols = board.GetLength(1);
    for (int c = c1 - 1; c >= 0; c--)
    {
      if (board[r1, c] != 0) break;

      if (CheckOneTurn(board, r1, c, r2, c2, out corner2))
      {
        corner1 = new Vector2Int(r1, c);
        return true;
      }
    }

    for (int c = c1 + 1; c < cols; c++)
    {
      if (board[r1, c] != 0) break;

      if (CheckOneTurn(board, r1, c, r2, c2, out corner2))
      {
        corner1 = new Vector2Int(r1, c);
        return true;
      }
    }

    for (int r = r1 - 1; r >= 0; r--)
    {
      if (board[r, c1] != 0) break;

      if (CheckOneTurn(board, r, c1, r2, c2, out corner2))
      {
        corner1 = new Vector2Int(r, c1);
        return true;
      }
    }

    for (int r = r1 + 1; r < rows; r++)
    {
      if (board[r, c1] != 0) break;
      if (CheckOneTurn(board, r, c1, r2, c2, out corner2))
      {
        corner1 = new Vector2Int(r, c1);
        return true;
      }
    }

    return false;
  }
  private bool ClearRow(int[,] board, int row, int c1, int c2)
  {
    int min = Mathf.Min(c1, c2) + 1;
    int max = Mathf.Max(c1, c2);

    for (int c = min; c < max; c++)
    {
      if (board[row, c] != 0) return false;
    }

    return true;
  }

  private bool ClearCol(int[,] board, int col, int r1, int r2)
  {
    int min = Mathf.Min(r1, r2) + 1;
    int max = Mathf.Max(r1, r2);

    for (int r = min; r < max; r++)
    {
      if (board[r, col] != 0)
        return false;
    }

    return true;
  }

  private List<Vector2Int> BuildPath(params Vector2Int[] points)
  {
    List<Vector2Int> path = new();

    foreach (var point in points)
    {
      if (path.Count == 0 || path[path.Count - 1] != point)
      {
        path.Add(point);
      }
    }

    return path;
  }

}
