using System;

namespace MTS.Editor
{
    /// <summary>
    /// TODO: update summary, add TestName property
    /// </summary>
    public class ParamNotFoundException : EditorException
    {
        public string ParamName { get; private set; }

        public string TestName { get; private set; }

        public ParamNotFoundException(string paramName)
            : base("")
        {
            ParamName = paramName;
        }
    }
}
