/// <summary>
/// Environment类型枚举
/// 定义不同的运行模式
/// </summary>
    public enum EnvironmentType
{
    Game,       // 游戏运行时（完整的Manager组合）
    Editor,     // 编辑器工具（只包含编辑器需要的Manager）
    Minimal,    // 最小架构（单元测试用，只包含必要的Manager）
    Custom      // 自定义（预留扩展）
}