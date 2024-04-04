// Licensed under CPOL: https://en.wikipedia.org/wiki/Code_Project_Open_License
// 
// Author: Paulo Francisco Zemek. August, 03, 2023.

using NUnit.Framework;
using System.Numerics;

namespace Pfz.Math.Tests;

public sealed class BigDecimalParseTests
{
	[Test]
	public void TryParseFailsWithEmptyString()
	{
		Assert.IsFalse(BigDecimal.TryParse("", out var _));
	}

	[Test]
	public void TryParseFailsWithInvalidCharacter()
	{
		Assert.IsFalse(BigDecimal.TryParse("123a", out var _));
	}

	[Test]
	public void TryParseSucceeds()
	{
		Assert.IsTrue(BigDecimal.TryParse("012.345000", out var result));
		Assert.That(result, Is.EqualTo(new BigDecimal(12345, 3)));
	}

	[Test]
	public void ParseFailsWithNullInput()
	{
		string? nullValue = null;
		Assert.Throws<ArgumentException>(() => BigDecimal.Parse(nullValue!));
	}

	[Test]
	public void ParseFailsWithEmptyString()
	{
		Assert.Throws<ArgumentException>(() => BigDecimal.Parse(""));
	}

	[Test]
	public void ParseFailsWithAlmostValidValue()
	{
		Assert.Throws<ArgumentException>(() => BigDecimal.Parse("123a567"));
	}

	[Test]
	public void ParseFailsWithTwoDots()
	{
		Assert.Throws<ArgumentException>(() => BigDecimal.Parse("123.567.980"));
	}

	[Test]
	public void ParseWorksWithUInt64MaxValueAsString()
	{
		var maxValue = UInt64.MaxValue;
		var str = maxValue.ToString();
		var bigDecimal = BigDecimal.Parse(str);
		var bigDecimalStr = bigDecimal.ToString();

		Assert.That(bigDecimalStr, Is.EqualTo(str));
	}

	[Test]
	public void ParseWorksWithFloatPoint()
	{
		var parsedValue = BigDecimal.Parse("00012345.67890");
		var directValue = new BigDecimal(1234567890, 5);
		var directValue2 = new BigDecimal(123456789, 4);

		Assert.That(parsedValue, Is.EqualTo(directValue));
		Assert.That(directValue2, Is.EqualTo(directValue));
	}

	[Test]
	public void ParseGiant57058WithLotsOfUnnecessaryZeroes()
	{
		var parsedValue = BigDecimal.Parse("0000000000000000000000000000000000000000057.05800000000000000000");
		Assert.That(parsedValue.DigitCount, Is.EqualTo(new BigInteger(5)));
		Assert.That(parsedValue.ToString(), Is.EqualTo("57.058"));
	}


	[Test]
	public void WhoSupportsMore()
	{
		decimal m = 1m;
		decimal mUInt64Max = UInt64.MaxValue;

		var bd = BigDecimal.One;
		var bdUInt64Max = new BigDecimal(UInt64.MaxValue);

		bool isDecimalSupported = true;
		bool isBigDecimalSupported = true;

		// TODO: Notice that if you reduce the 1000 value to 1, the test will fail
		// that's because both decimal and BigDecimal will reach to the same results with
		// no overflow.
		for (int i=0; i<1000; i++)
		{
			bool bigDecimalSupported = true;

			try
			{
				m *= mUInt64Max;
			}
			catch
			{
				isDecimalSupported = false;
				break;
			}

			try
			{
				bd *= bdUInt64Max;
			}
			catch
			{
				bigDecimalSupported = false;
				break;
			}

			if (!isDecimalSupported || !bigDecimalSupported)
				throw new Exception("Error at digit " + i);

			Assert.IsTrue(bigDecimalSupported);
			Assert.That(bd.ToString(), Is.EqualTo(m.ToString()));
		}

		Assert.IsTrue(isBigDecimalSupported);
		Assert.IsFalse(isDecimalSupported);
	}

	[Test]
	public static void JustDot3()
	{
		var parsedValue = BigDecimal.Parse(".3");
		var expectedValue = new BigDecimal(3, 1);
		Assert.That(parsedValue, Is.EqualTo(expectedValue));
		Assert.That(parsedValue.ToString(), Is.EqualTo("0.3"));
	}

	[Test]
	public static void ParseNegativeNumber()
	{
		var parsedValue = BigDecimal.Parse("-.5");
		var expectedValue = new BigDecimal(-5, 1);
		Assert.That(parsedValue, Is.EqualTo(expectedValue));
	}
}

