using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MTS.IO;
using MTS.Editor;

namespace MTS.Tester
{
    //class TestChannel : IDigitalInput
    //{
    //    private TestValue testParam;

    //    #region IDigitalInput Members

    //    public bool Value
    //    {
    //        get { return testParam.Enabled; }
    //    }

    //    public void SetValue(bool value)
    //    {
    //        testParam.Enabled = value;
    //    }

    //    #endregion

    //    #region IChannel Members

    //    public string Id
    //    {
    //        get;
    //        set;
    //    }

    //    public string Name
    //    {
    //        get;
    //        set;
    //    }

    //    public string Description
    //    {
    //        get;
    //        set;
    //    }

    //    public byte[] ValueBytes
    //    {
    //        get;
    //        set;
    //    }

    //    public int Size
    //    {
    //        get;
    //        set;
    //    }

    //    public event ChannelChangedEventHandler ValueChanged;

    //    #endregion

    //    public TestChannel(TestValue testParam)
    //    {
    //        this.testParam = testParam;
    //        Id = string.Format("Is{0}Enabled", testParam.ValueId);
    //        Name = string.Format("Is {0} Enabled", testParam.Name);
    //        Description = string.Format("Special channel for test {0} enabled value", testParam.Name);
    //    }
    //}
}
