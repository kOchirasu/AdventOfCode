using NUnit.Framework;

namespace UtilExtensions.Tests;

public class MathUtilsTests {
    [Test]
    public void LcmTest() {
        Assert.AreEqual(0, MathUtils.Lcm());
        Assert.AreEqual(5 * 5, MathUtils.Lcm(25));
        Assert.AreEqual(2 * 3, MathUtils.Lcm(2, 3));
        Assert.AreEqual(2 * 2 * 3 * 5, MathUtils.Lcm(2, 3, 4, 5));
    }

    [Test]
    public void GcdTest() {
        Assert.AreEqual(0, MathUtils.Gcd());
        Assert.AreEqual(25, MathUtils.Gcd(25));
        Assert.AreEqual(18, MathUtils.Gcd(461952, 116298));
        Assert.AreEqual(30, MathUtils.Gcd(81 * 30, 2 * 30, 13 * 30));
    }
}
