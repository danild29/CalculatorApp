// Licensed under CPOL: https://en.wikipedia.org/wiki/Code_Project_Open_License
// 
// Author: Paulo Francisco Zemek. August, 02, 2023.

using NUnit.Framework;
using System.Numerics;

namespace Pfz.Math.Tests;

public sealed class BigDecimalTests
{
	[Test]
	public void TestNegativeValue()
	{
		var bigDecimal = new BigDecimal(-1234);
		Assert.That(bigDecimal.DigitCount, Is.EqualTo(new BigInteger(4)));
	}

	[Test]
	public void TestManyDigitsDividend()
	{
		var dividend = new BigDecimal(1234567, 5);
		var divisor = BigDecimal.Two;
		var result = dividend / divisor;
		
		var decimalResult = 12.34567m / 2;
		var strResult = decimalResult.ToString();
		Assert.That(result, Is.EqualTo(BigDecimal.Parse(strResult)));
	}

	[Test]
	public void Divide_ZeroByZero_Returns1()
	{
		// To allow the rule that x/x is 1, a division of 0/0 also gives one.
		// That's on purpose.
		var result = BigDecimal.Zero / BigDecimal.Zero;
		Assert.That(result, Is.EqualTo(BigDecimal.One));
	}

	[Test]
	public void DivisionByZeroFails()
	{
		var zeroFive = new BigDecimal(5, 1);
		var zeroSix = new BigDecimal(6, 1);
		Assert.That(zeroSix, Is.Not.EqualTo(zeroFive));

		var zeroOne = new BigDecimal(1, 1);
		var otherZeroSix = zeroFive + zeroOne;
		Assert.That(otherZeroSix, Is.EqualTo(zeroSix));

		Assert.Throws<DivideByZeroException>(() => { var _ = zeroFive / BigDecimal.Zero; });
	}

	[Test]
	public void Value1_100_10000_AreTheSameIfTheSeparatorIsAtTheRightPlace()
	{
		var value1 = BigDecimal.One;
		var value100 = new BigDecimal(100, 2);
		var value10000 = new BigDecimal(10000, 4);

		Assert.That(value100, Is.EqualTo(value1));
		Assert.That(value100, Is.EqualTo(value10000));
	}

	[Test]
	public void PositivePlusNegativeEndsPositive_Int()
	{
		var value1 = new BigDecimal(100);
		var value2 = new BigDecimal(-40);
		var result = value1 + value2;

		Assert.That(result, Is.EqualTo(new BigDecimal(60)));
	}
	[Test]
	public void PositivePlusNegativeEndsNegative_Int()
	{
		var value1 = new BigDecimal(40);
		var value2 = new BigDecimal(-100);
		var result = value1 + value2;

		Assert.That(result, Is.EqualTo(new BigDecimal(-60)));
	}

	[Test]
	public void BigPositiveMinusSmallPositiveEndsPositive_Int()
	{
		var value1 = new BigDecimal(100);
		var value2 = new BigDecimal(55);
		var result = value1 - value2;

		Assert.That(result, Is.EqualTo(new BigDecimal(45)));
	}

	[Test]
	public void SmallPositiveMinusBigPositiveEndsNegative_Int()
	{
		var value1 = new BigDecimal(35);
		var value2 = new BigDecimal(100);
		var result = value1 - value2;

		Assert.That(result, Is.EqualTo(new BigDecimal(-65)));
	}

	[Test]
	public void CanMultiplyUInt64Max()
	{
		var uint64Max = UInt64.MaxValue;

		// The following assert only works because UInt64.Max multiplied by 100 overflows.
		Assert.That(uint64Max * 100, Is.LessThan(UInt64.MaxValue));

		var bigDecimal = new BigDecimal(uint64Max);
		var bigDecimal2 = bigDecimal * new BigDecimal(100);

		// Here it should be greater than, as we expect BigDecimal to deal with huge values.
		Assert.That(bigDecimal2, Is.GreaterThan(bigDecimal));

		Assert.That(bigDecimal2.ToString(), Is.EqualTo(uint64Max.ToString() + "00"));

		var bigDecimal3 = bigDecimal2 / new BigDecimal(100);
		Assert.That(bigDecimal3, Is.EqualTo(bigDecimal));
	}

	[Test]
	public void MultiplyByLessThan1MeansDivision()
	{
		var fiftySeven = new BigDecimal(57);
		var half = new BigDecimal(5, 1); // this means 0.5
		var result = fiftySeven * half;
		var twentyEightAndHalf = new BigDecimal(285, 1);
		Assert.That(result, Is.EqualTo(twentyEightAndHalf));
	}

	[Test]
	public void MultiplyNegativeByLessThan1MeansDivision()
	{
		var fiftySeven = new BigDecimal(-57);
		var half = new BigDecimal(5, 1); // this means 0.5
		var result = fiftySeven * half;
		var twentyEightAndHalf = new BigDecimal(-285, 1);
		Assert.That(result, Is.EqualTo(twentyEightAndHalf));
	}

	[Test]
	public void SucceedWhereDecimalFails()
	{
		// TODO: I am not sure how to make this an actual test.
		// Debugging it, I see my version can have more precision
		// if asked to... not sure what to say by the average case.
		decimal x = 1m;
		decimal y = ulong.MaxValue;
		decimal result = x / y;
		var result1 = result.ToString();

		BigDecimal mx = BigDecimal.One;
		BigDecimal my = new BigDecimal(ulong.MaxValue);
		
		BigDecimal mresult = BigDecimal.Divide(mx, my, 100);
		// It's important to note that we can say the number of decimal digits we want.
		// I didn't see an equivalent on the .NET decimal class.

		var result2 = mresult.ToString();
		
		Assert.That(result2.StartsWith(result1));
	}

	[Test]
	public void Test7777777()
	{
		var a = new BigDecimal(1234567, 3);
		var b = new BigDecimal(6543210, 3);
		var result = a + b;

		var expectedResult = new BigDecimal(7777777, 3);
		Assert.That(result, Is.EqualTo(expectedResult));

		var test = new BigDecimal(5, 3);
		test += new BigDecimal(8);
		Console.WriteLine(test);
	}

	[Test]
	public static void AssertDecimalCannotMultiplyUInt64Max_100Times()
	{
		Assert.Throws<OverflowException>
		(
			() =>
			{
				var ulongMaxAsDecimal = ulong.MaxValue;
				var result = 1m;
				for (int i=0; i<100; i++)
					result *= ulongMaxAsDecimal;
			}
		);
	}

	[Test]
	public static void AssertBigDecimalCanMultiplyUInt64Max_100Times()
	{
		var ulongMaxAsDecimal = new BigDecimal(ulong.MaxValue);
		var result = BigDecimal.One;
		for (int i = 0; i < 100; i++)
			result *= ulongMaxAsDecimal;

		var bigIntegerUInt64MaxValue = new BigInteger(ulong.MaxValue);
		var bigIntegerResult = BigInteger.Pow(bigIntegerUInt64MaxValue, 100);

		var toString1 = result.ToString();
		var toString2 = bigIntegerResult.ToString();

		Assert.That(toString2, Is.EqualTo(toString1));
	}

	[Test]
	public void Pow10_DecimalVsBigDecimal()
	{
		// I wanted to use decimal, but there's no Pow in decimal at this moment.
		double value1 = 2.5;
		double result1 = System.Math.Pow(value1, 10);

		var value2 = new BigDecimal(25, 1);
		var result2 = value2.Power(10);

		var result1String = result1.ToString();
		var result2String = result2.ToString();
		Assert.That(result1String, Is.EqualTo(result2String));
	}

	[Test]
	public static void SubtractRemovesUnnecessaryZeroes()
	{
		var v1 = new BigDecimal(1555, 2);
		var v2 = new BigDecimal(2555, 2);
		var r = v1 - v2;
		Assert.That(r.ToString(), Is.EqualTo("-10"));
	}

	[Test]
	public void CompareGreaterThanLessThanAndTheirEquals()
	{
		var v1 = new BigDecimal(12345);    // Effectively 12345
		var v2 = new BigDecimal(56789, 1); // Effectively 5678.9

		// Used just to avoid warnings of comparing a variable to itself.
		var v1Copy = v1;
		var v2Copy = v2;

		// Greater / less than
		Assert.That(v2, Is.LessThan(v1));
		Assert.That(v2, Is.LessThanOrEqualTo(v1));
		Assert.That(v1, Is.GreaterThan(v2));
		Assert.That(v1, Is.GreaterThanOrEqualTo(v2));

		// Equalities
		Assert.That(v1, Is.GreaterThanOrEqualTo(v1Copy));
		Assert.That(v2, Is.LessThanOrEqualTo(v2Copy));

		// Greater and less than operators
		Assert.IsTrue(v2 < v1);
		Assert.IsTrue(v2 <= v1);
		Assert.IsTrue(v1 > v2);
		Assert.IsTrue(v1 >= v2);

		// Equality through operators
		Assert.IsTrue(v1 <= v1Copy);
		Assert.IsTrue(v2 >= v2Copy);
	}

	[Test]
	public void Minus2_Plus_Minus3_Equals_Minus5()
	{
		var v1 = new BigDecimal(-2);
		var v2 = new BigDecimal(-3);
		var r = v1 + v2;

		Assert.That(r, Is.EqualTo(new BigDecimal(-5)));
		Assert.That(r.ToString(), Is.EqualTo("-5"));
	}

	[Test]
	public void Minus2_Minus_Minus3_Equals1()
	{
		var v1 = new BigDecimal(-2);
		var v2 = new BigDecimal(-3);
		var r = v1 - v2;

		Assert.That(r, Is.EqualTo(BigDecimal.One));
		Assert.That(r.ToString(), Is.EqualTo("1"));
	}

	[Test]
	public void DoubleByDivisionByZeroDotFive()
	{
		var zeroDotZeroFive = new BigDecimal(5, 2);
		var fiftySeven = new BigDecimal(57);
		var result = fiftySeven / zeroDotZeroFive;

		Assert.That(result, Is.EqualTo(new BigDecimal(1140)));
	}

	[Test]
	public void RemainderWorks()
	{
		var a = new BigDecimal(100);
		var b = new BigDecimal(3);
		var remainder = BigDecimal.Remainder(a, b, 2);
		
		Assert.That(remainder, Is.EqualTo(new BigDecimal(1, 2)));
	}

	[Test]
	public void OperatorModWorks()
	{
		var a = new BigDecimal(100);
		var b = new BigDecimal(3);
		var remainder = BigDecimal.Remainder(a, b, BigDecimal.DefaultFloatDigitCount);;

		Assert.That(remainder, Is.EqualTo(new BigDecimal(1, BigDecimal.DefaultFloatDigitCount)));
	}

	[Test]
	public void PositivePower()
	{
		var baseValue = new BigDecimal(5);
		var powerValue = baseValue.Power(3);
		Assert.That(new BigDecimal(125), Is.EqualTo(powerValue));
	}

	[Test]
	public void NegativePower()
	{
		var baseValue = new BigDecimal(5);
		var result = baseValue.Power(-2);
		Assert.That(result, Is.EqualTo(new BigDecimal(4, 2)));
	}

	[Test]
	public void Power0()
	{
		var value = new BigDecimal(1234567, 2);
		var value2 = value.Power(0);
		Assert.That(value2, Is.EqualTo(BigDecimal.One));
	}

	[Test]
	public void NegativeValuesPowerPositiveExponents()
	{
		var twoNegative = new BigDecimal(-2);

		var result1 = twoNegative.Power(BigInteger.One);
		var result2 = twoNegative.Power(new BigInteger(2));
		var result3 = twoNegative.Power(new BigInteger(3));

		Assert.That(result1, Is.EqualTo(new BigDecimal(-2)));
		Assert.That(result2, Is.EqualTo(new BigDecimal(4)));
		Assert.That(result3, Is.EqualTo(new BigDecimal(-8)));
	}
}
