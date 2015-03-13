using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Globalization;

namespace Result.OrDefault
{
	[TestClass]
	public class Tests
	{
		public interface I
		{
			string ValueP { get; }
			string ValueM();
			I MeP { get; }
			I MeM();
		}


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
		public void MethodWithParameter()
		{
			double value = 4.3;
			Assert.AreEqual("4.3", Result.OrDefault(() => value.ToString(CultureInfo.InvariantCulture)));
		}


		[TestMethod]
		public void MethodThrowsTest()
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
		public void PropertyThrowsTest()
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
		public void MethodPropertyVariationsTest()
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
		public void FirstCallIsAMethodTest()
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
		public void FirstCallIsAPropertyTest()
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
		public void PerformanceTrainHelperTest()
		{
			Assert.Inconclusive();
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.MeM().MeP.ValueP).Returns("ja");

			for (int i = 0; i < 1000000; i++)
			{
				Result.OrDefault(() => mock.Object.MeP.MeM().MeP.ValueP);
			}
		}
		[TestMethod]
		public void PerformanceNoTrainHelperTest()
		{
			Assert.Inconclusive();
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.MeM().MeP.ValueP).Returns("ja");

			for (int i = 0; i < 1000000; i++)
			{
				var value = mock.Object.MeP.MeM().MeP.ValueP;
			}
		}
		[TestMethod]
		public void PerformanceNoTrainHelperKorayTest()
		{
			Assert.Inconclusive();
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.MeM().MeP.ValueP).Returns("ja");

			for (int i = 0; i < 1000000; i++)
			{
				if (mock.Object.MeP == null)
				{
					continue;
				}

				var meM = mock.Object.MeP.MeM();
				if (meM == null)
				{
					continue;
				}

				var meP = meM.MeP;
				if (meP == null)
				{
					continue;
				}

				var valueP = meP.ValueP;
				if (valueP == null)
				{
					continue;
				}

				var value = valueP;
			}
		}
	}
}
