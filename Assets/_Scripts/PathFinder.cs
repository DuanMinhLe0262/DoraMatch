using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private struct Node
    {
        public int Row;
        public int Col;
        public int Dir;
        public int Turns;

        public Node(int row, int col, int dir, int turns)
        {
            Row = row;
            Col = col;
            Dir = dir;
            Turns = turns;
        }
    }

    private readonly Vector2Int[] directions =
    {
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1)
    };

    public bool CanConnect(BoardData board, int row1, int col1, int row2, int col2)
    {
        return FindShortestPath(board, row1, col1, row2, col2) != null;
    }

    public List<Vector2Int> FindShortestPath(BoardData board, int row1, int col1, int row2, int col2)
    {
        if (board == null || board.Cells == null) return null;
        if (row1 == row2 && col1 == col2) return null;
        if (!IsValid(board, row1, col1) || !IsValid(board, row2, col2)) return null;
        if (board.Cells[row1, col1] == 0 || board.Cells[row2, col2] == 0) return null;
        if (board.Cells[row1, col1] != board.Cells[row2, col2]) return null;

        int rows = board.PlayableRows + 2;
        int cols = board.PlayableCols + 2;
        const int Unreachable = int.MaxValue;

        Queue<Node> queue = new();
        int[,,] bestTurns = new int[rows, cols, 4];
        Node?[,,] parent = new Node?[rows, cols, 4];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                for (int d = 0; d < 4; d++)
                {
                    bestTurns[r, c, d] = Unreachable;
                }
            }
        }

        Vector2Int start = new(row1, col1);
        Vector2Int end = new(row2, col2);

        for (int dir = 0; dir < 4; dir++)
        {
            bestTurns[start.x, start.y, dir] = 0;
            queue.Enqueue(new Node(start.x, start.y, dir, 0));
        }

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            if (current.Turns > bestTurns[current.Row, current.Col, current.Dir])
                continue;

            int nx = current.Row + directions[current.Dir].x;
            int ny = current.Col + directions[current.Dir].y;

            while (IsValid(board, nx, ny))
            {
                bool isTarget = nx == end.x && ny == end.y;

                if (!isTarget && board.Cells[nx, ny] != 0)
                    break;

                if (current.Turns < bestTurns[nx, ny, current.Dir])
                {
                    bestTurns[nx, ny, current.Dir] = current.Turns;
                    parent[nx, ny, current.Dir] = current;

                    if (isTarget)
                    {
                        return ReconstructPath(
                            parent,
                            new Node(nx, ny, current.Dir, current.Turns),
                            start
                        );
                    }

                    queue.Enqueue(new Node(nx, ny, current.Dir, current.Turns));
                }

                nx += directions[current.Dir].x;
                ny += directions[current.Dir].y;
            }

            for (int newDir = 0; newDir < 4; newDir++)
            {
                if (newDir == current.Dir) continue;

                int newTurns = current.Turns + 1;
                if (newTurns > 2) continue;

                if (newTurns < bestTurns[current.Row, current.Col, newDir])
                {
                    bestTurns[current.Row, current.Col, newDir] = newTurns;
                    parent[current.Row, current.Col, newDir] = current;
                    queue.Enqueue(new Node(current.Row, current.Col, newDir, newTurns));
                }
            }
        }

        return null;
    }

    private List<Vector2Int> ReconstructPath(Node?[,,] parent, Node endNode, Vector2Int start)
    {
        List<Vector2Int> path = new();
        Node current = endNode;

        Vector2Int point = new(current.Row, current.Col);
        path.Add(point);

        while (current.Row != start.x || current.Col != start.y)
        {
            Node prev = parent[current.Row, current.Col, current.Dir].Value;
            current = prev;

            point = new Vector2Int(current.Row, current.Col);
            if (path[path.Count - 1] != point)
            {
                path.Add(point);
            }
        }

        path.Reverse();
        return CompressPath(path);
    }

    private List<Vector2Int> CompressPath(List<Vector2Int> fullPath)
    {
        if (fullPath.Count <= 2)
            return fullPath;

        List<Vector2Int> result = new();
        result.Add(fullPath[0]);

        for (int i = 1; i < fullPath.Count - 1; i++)
        {
            Vector2Int prev = fullPath[i - 1];
            Vector2Int current = fullPath[i];
            Vector2Int next = fullPath[i + 1];

            Vector2Int dir1 = current - prev;
            Vector2Int dir2 = next - current;

            if (dir1 != dir2)
                result.Add(current);
        }

        result.Add(fullPath[fullPath.Count - 1]);
        return result;
    }

    private bool IsValid(BoardData board, int row, int col)
    {
        return row >= 0
            && row < board.PlayableRows + 2
            && col >= 0
            && col < board.PlayableCols + 2;
    }
}
