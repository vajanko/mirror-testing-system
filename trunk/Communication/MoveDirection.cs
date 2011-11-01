namespace MTS.IO
{
    /// <summary>
    /// Direction of mirror movement
    /// </summary>
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    public static class MoveDirectionExtension
    {
        public static bool IsVertical(this MoveDirection direction)
        {
            return (direction == MoveDirection.Up || direction == MoveDirection.Down);
        }
        public static bool IsHorizontal(this MoveDirection direction)
        {
            return (direction == MoveDirection.Left || direction == MoveDirection.Right);
        }
    }
}
