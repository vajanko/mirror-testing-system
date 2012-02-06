namespace MTS.IO
{
    /// <summary>
    /// Direction of mirror movement
    /// </summary>
    public enum MoveDirection
    {
        /// <summary>
        /// Move mirror up (north)
        /// </summary>
        Up,
        /// <summary>
        /// Move mirror down (south)
        /// </summary>
        Down,
        /// <summary>
        /// Move mirror left (west)
        /// </summary>
        Left,
        /// <summary>
        /// Move mirror right (east)
        /// </summary>
        Right,
        /// <summary>
        /// Do not move mirror
        /// </summary>
        None
    }

    /// <summary>
    /// Extension for <see cref="MoveDirection"/> enumerator. Allows distinguish between horizontal and
    /// vertical direction.
    /// </summary>
    public static class MoveDirectionExtension
    {
        /// <summary>
        /// Get value indicating whether given direction is vertical
        /// </summary>
        /// <param name="direction">Direction to check if it is vertical</param>
        /// <returns>True if direction is vertical</returns>
        public static bool IsVertical(this MoveDirection direction)
        {
            return (direction == MoveDirection.Up || direction == MoveDirection.Down);
        }
        /// <summary>
        /// Get value indicating whether given direction is horizontal
        /// </summary>
        /// <param name="direction">Direction to check if it is horizontal</param>
        /// <returns>True if direction is horizontal</returns>
        public static bool IsHorizontal(this MoveDirection direction)
        {
            return (direction == MoveDirection.Left || direction == MoveDirection.Right);
        }
    }
}
