namespace PL
{
    internal class KeyPressEventArgs
    {
        public int KeyChar { get; internal set; }
        public bool Handled { get; internal set; }
    }
}