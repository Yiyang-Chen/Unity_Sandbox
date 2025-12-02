// 数据类，但是不会做强制的MVC划分
public interface IData { }

// 静态数据，不会在运行时改变,只允许在游戏外读取
public interface IStaticData : IData { }
// 表格数据
public interface ITableData : IStaticData { }
// 其他配置数据
public interface IConfigData : IStaticData { }

// 运行时模型，会在运行时改变
public interface IRuntimeModel : IData { }
