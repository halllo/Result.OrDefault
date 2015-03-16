using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Globalization;
using System.Linq;

namespace ResultOrDefault.Tests
{
	[TestClass]
	public class Tests
	{
		[TestMethod]
		public void DefaultTest()
		{
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.ValueM()).Returns("ja");
			Assert.AreEqual("ja", Result.OrDefault(() => mock.Object.MeP.ValueM(), defaultValue: "nein"));
			Assert.AreEqual("nein", Result.OrDefault(() => mock.Object.MeP.MeP.ValueM(), defaultValue: "nein"));
			try
			{
				var value = mock.Object.MeP.MeP.ValueM();
				Assert.Fail("NullReferenceException expected");
			}
			catch (NullReferenceException expected)
			{
			}
		}


		[TestMethod]
		public void MethodWithConstantAndClosureParameter()
		{
			var replacement = string.Empty;
			var value = "hallo";
			Assert.AreEqual("hao", Result.OrDefault(() => value.Replace("l", replacement)));
		}


		[TestMethod]
		public void MethodThrowingException()
		{
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.ValueM()).Throws<DivideByZeroException>();
			try
			{
				Result.OrDefault(() => mock.Object.MeP.ValueM());
				Assert.Fail("DivideByZeroException expected");
			}
			catch (DivideByZeroException expected)
			{
			}
		}


		[TestMethod]
		public void PropertyThrowingException()
		{
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.ValueP).Throws<DivideByZeroException>();
			try
			{
				Result.OrDefault(() => mock.Object.MeP.ValueP);
				Assert.Fail("DivideByZeroException expected");
			}
			catch (DivideByZeroException expected)
			{
			}
		}


		[TestMethod]
		public void MethodPropertyVariations()
		{
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.MeM().MeP.ValueP).Returns("ja");
			Assert.IsNull(Result.OrDefault(() => mock.Object.ValueP));
			Assert.IsNull(Result.OrDefault(() => mock.Object.ValueM()));
			Assert.IsNull(Result.OrDefault(() => mock.Object.MeP.ValueP));
			Assert.IsNull(Result.OrDefault(() => mock.Object.MeP.ValueM()));
			Assert.IsNull(Result.OrDefault(() => mock.Object.MeP.MeP.ValueP));
			Assert.IsNull(Result.OrDefault(() => mock.Object.MeP.MeP.ValueM()));
			Assert.IsNull(Result.OrDefault(() => mock.Object.MeP.MeM().ValueP));
			Assert.IsNull(Result.OrDefault(() => mock.Object.MeP.MeM().ValueM()));
			Assert.AreEqual("ja", Result.OrDefault(() => mock.Object.MeP.MeM().MeP.ValueP));
		}


		[TestMethod]
		public void FirstCallIsAMethod()
		{
			Assert.AreEqual("ja", Result.OrDefault(() => MyMethod().ValueM(), defaultValue: "nein"));
		}
		private I MyMethod()
		{
			var mock = new Mock<I>();
			mock.Setup(i => i.ValueM()).Returns("ja");
			return mock.Object;
		}


		[TestMethod]
		public void StaticMethod()
		{
			Assert.IsNull(Result.OrDefault(() => MyStaticMethod().ToString()));
		}
		private static string MyStaticMethod()
		{
			return null;
		}


		[TestMethod]
		public void FirstCallIsAProperty()
		{
			Assert.AreEqual("ja", Result.OrDefault(() => MyProperty.ValueM(), defaultValue: "nein"));
		}
		private I MyProperty
		{
			get
			{
				var mock = new Mock<I>();
				mock.Setup(i => i.ValueM()).Returns("ja");
				return mock.Object;
			}
		}


		[TestMethod]
		public void StaticProperty()
		{
			Assert.IsNull(Result.OrDefault(() => MyStaticMethod().ToString()));
		}
		private static string MyStaticProperty
		{
			get
			{
				return null;
			}
		}


		[TestMethod]
		public void Raises_exception_when_parameters_are_used_in_the_expression__1()
		{
			var value = 4.3;
			Assert.AreEqual("4.3", Result.OrDefault(() => value.ToString(CultureInfo.InvariantCulture)));
		}


		[TestMethod]
		public void Other_cases_raising_exceptions__2()
		{
			Assert.AreEqual("\0", Result.OrDefault(() => "abc".Trim().FirstOrDefault(char.IsControl).ToString(CultureInfo.InvariantCulture)));
			Assert.AreEqual("\0", Result.OrDefault(() => "abc".Trim().FirstOrDefault(char.IsControl).ToString()));

			Assert.AreEqual("aaa", Result.OrDefault(() => new string('a', 3).Trim()));
			Assert.AreEqual(3, Result.OrDefault(() => new string('a', 3).Length));

			Assert.IsNull(Result.OrDefault(() => ((string)null).Trim()));
			Assert.AreEqual(-1, Result.OrDefault(() => ((string)null).Length, defaultValue: -1));
			Assert.IsNull(Result.OrDefault(() => ((string)null).ToString(CultureInfo.InvariantCulture)));
		}
	}
}
