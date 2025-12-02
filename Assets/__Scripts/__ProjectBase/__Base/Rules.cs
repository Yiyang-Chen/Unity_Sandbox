public interface ICanInit
{
    bool Initialized { get; set; }
    void Init();
    void Shutdown();
}

public interface IBelongToEnvironment
{
    IEnvironment GetEnvironment();
}

public interface ICanSetEnvironment
{
    void SetEnvironment(IEnvironment environment);
}

public interface ICanGetSystem : IBelongToEnvironment
{
}

public static class CanGetSystemExtension
{
    public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
        self.GetEnvironment().GetSystem<T>();
}

public interface IHasController
{
    public void RegisterController(IController controller);
    IController GetController();
}
