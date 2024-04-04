// Licensed under CPOL: https://en.wikipedia.org/wiki/Code_Project_Open_License
// 
// Author: Paulo Francisco Zemek. August, 04, 2023.

using NUnit.Framework;
using System.Globalization;
using System.Numerics;

namespace Pfz.Math.Tests;

public sealed class BigRationalTests
{
	[Test]
	public void CompressFromTheStart()
	{
		var rational = new BigRational(100, 50);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(2)));
		Assert.That(rational.Denominator, Is.EqualTo(BigInteger.One));

		Assert.That(rational.ToBigDecimal(), Is.EqualTo(BigDecimal.Two));
	}

	[Test]
	public void CompressFromTheStart_NegativeNominator()
	{
		var rational = new BigRational(-100, 50);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(-2)));
		Assert.That(rational.Denominator, Is.EqualTo(BigInteger.One));

		Assert.That(rational.ToBigDecimal(), Is.EqualTo(new BigDecimal(-2)));
	}

	[Test]
	public void CompressEvenNumbers()
	{
		var rational = new BigRational(62, 30);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(31)));
		Assert.That(rational.Denominator, Is.EqualTo(new BigInteger(15)));

		var newRational = new BigRational(rational.Numerator - 1, rational.Denominator);
		Assert.That(newRational.Numerator, Is.EqualTo(new BigInteger(2)));
		Assert.That(newRational.Denominator, Is.EqualTo(BigInteger.One));
	}

	[Test]
	public void CompressNegativeEvenNumbers()
	{
		var rational = new BigRational(-62, 30);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(-31)));
		Assert.That(rational.Denominator, Is.EqualTo(new BigInteger(15)));

		var newRational = new BigRational(rational.Numerator + 1, rational.Denominator);
		Assert.That(newRational.Numerator, Is.EqualTo(new BigInteger(-2)));
		Assert.That(newRational.Denominator, Is.EqualTo(BigInteger.One));
	}

	[Test]
	public void AddWorks()
	{
		var rational = new BigRational(5, 3);
		rational.Add(1);
		var bigDecimal = rational.ToBigDecimal();
		Assert.That(bigDecimal, Is.EqualTo(new BigDecimal(266666666, 8)));
	}

	[Test]
	public void SubtractWorks()
	{
		var rational = new BigRational(5, 3);
		rational.Subtract(1);
		var bigDecimal = rational.ToBigDecimal();
		Assert.That(bigDecimal, Is.EqualTo(new BigDecimal(66666666, 8)));
	}

	[Test]
	public void CompareSomewhatEqual()
	{
		var rational1 = new BigRational(5, 3);
		var rational2 = new BigRational(50, 30);

		var compareResult = rational1.CompareTo(rational2);
		Assert.That(compareResult, Is.EqualTo(0));
	}

	[Test]
	public void CompareLessThan()
	{
		var rational1 = new BigRational(5, 3);
		var rational2 = new BigRational(51, 30);

		var compareResult = rational1.CompareTo(rational2);
		Assert.That(compareResult, Is.LessThan(0));
	}

	[Test]
	public void ManyDivisionsThenASingleMultiplication()
	{
		var rational = new BigRational(100);
		for (int i=0; i<30; i++)
			rational.Divide(2);

		rational.Add(2);
		var result = rational.ToBigDecimal(0);

		Assert.That(result, Is.EqualTo(BigDecimal.Two));
	}

	[Test]
	public void PositivePower()
	{
		var rational = new BigRational(11, 5);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(11)));
		Assert.That(rational.Denominator, Is.EqualTo(new BigInteger(5)));
		
		rational.Power(3);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(1331)));
		Assert.That(rational.Denominator, Is.EqualTo(new BigInteger(125)));
	}

	[Test]
	public void Power0Gives1And1()
	{
		var rational = new BigRational(10, 5);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(2)));
		Assert.That(rational.Denominator, Is.EqualTo(new BigInteger(1)));

		rational.Power(0);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(1)));
		Assert.That(rational.Denominator, Is.EqualTo(new BigInteger(1)));
	}

	[Test]
	public void NegativePower()
	{
		var rational = new BigRational(11, 5);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(11)));
		Assert.That(rational.Denominator, Is.EqualTo(new BigInteger(5)));

		var bigDecimal = rational.ToBigDecimal();
		var str = bigDecimal.ToString();
		var decimalParsed = decimal.Parse(str, CultureInfo.InvariantCulture);
		var decimalPow = 1.0m / (decimalParsed * decimalParsed * decimalParsed);

		rational.Power(-3);
		Assert.That(rational.Numerator, Is.EqualTo(new BigInteger(125)));
		Assert.That(rational.Denominator, Is.EqualTo(new BigInteger(1331)));

		string decimalAsStr = decimalPow.ToString();
		var bigDecimalResult = rational.ToBigDecimal();
		string bigDecimalAsStr = bigDecimalResult.ToString();
		Assert.That(decimalAsStr.Substring(0, 5), Is.EqualTo(bigDecimalAsStr.Substring(0, 5)));
	}

	[Test]
	public void AddAnotherRational()
	{
		var rational1 = new BigRational(7, 5);
		var rational2 = new BigRational(3, 2);
		rational1.Add(rational2);

		var result = rational1.ToBigDecimal();
		Assert.That(result, Is.EqualTo(new BigDecimal(29, 1)));
	}

	[Test]
	public void SubtractAnotherRational()
	{
		var rational1 = new BigRational(7, 5);
		var rational2 = new BigRational(3, 2);
		rational1.Subtract(rational2);

		var result = rational1.ToBigDecimal();
		Assert.That(result, Is.EqualTo(new BigDecimal(-1, 1)));
	}

	[Test]
	public void MultiplyAnotherRational()
	{
		var rational1 = new BigRational(7, 5);
		var rational2 = new BigRational(3, 2);
		rational1.Multiply(rational2);

		var result = rational1.ToBigDecimal();
		Assert.That(result, Is.EqualTo(new BigDecimal(21, 1)));
	}

	[Test]
	public void DivideAnotherRational()
	{
		var rational1 = new BigRational(7, 5);
		var rational2 = new BigRational(3, 2);
		rational1.Divide(rational2);

		var result = rational1.ToBigDecimal();
		Assert.That(result, Is.EqualTo(new BigDecimal(93333333, 8)));
	}
}
