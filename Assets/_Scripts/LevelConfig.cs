using UnityEngine;

[CreateAssetMenu(menuName = "DoraMatch/Level Config")]
public class LevelConfig : ScriptableObject
{
    public int rows = 8;
    public int cols = 12;
    public int iconTypeCount = 6;
    public BoardRule rule = BoardRule.None;
}
