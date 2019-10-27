namespace WC3Stats
{
    public interface IPlayerHandler<T>
    {
        bool Accepts(byte[] bytes);
        T Handle(byte[] bytes);
    }
}