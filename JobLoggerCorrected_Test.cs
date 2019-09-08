using NUnit.Framework;
using System; 
using System.Linq; 
using System.Text;

[TestFixture]

public class JobLogger_Test
{
    private JobLogger objTest;

    [SetUp]
    public void Init()
    {
        objTest = new JobLogger(1, 1, 1, 1, 1, 1);

    }

    [Test]
    public void When_MessageType_Expect_BeAMessage()
    {
        int mType = objTest.DefineMessageType(1,0,0);
        Assert.AreEqual(1, mType);
    }

    [Test]
    public void When_MessageType_Expect_BeAWarning()
    {
        int mType = objTest.DefineMessageType(0, 1, 0);
        Assert.AreEqual(3, mType);
    }

    [Test]
    public void When_MessageType_Expect_BeAnError()
    {
        int mType = objTest.DefineMessageType(0, 0, 1);
        Assert.AreEqual(2, mType);
    }

    [Test]
    public void When_LogIntoDataBase_Expect_BeCorrect()
    {
        bool isSaved = objTest.LogInDataBase("text", 1);
        Assert.IsTrue(isSaved);
    }
} 
  