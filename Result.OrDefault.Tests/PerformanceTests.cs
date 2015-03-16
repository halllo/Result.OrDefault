using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ResultOrDefault.Tests
{
	[TestClass]
	public class PerformanceTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Assert.Inconclusive();
		}

		[TestMethod]
		public void PerformanceTrainHelper()
		{
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.MeM().MeP.ValueP).Returns("ja");

			for (int i = 0; i < 1000000; i++)
			{
				Result.OrDefault(() => mock.Object.MeP.MeM().MeP.ValueP);
			}
		}

		[TestMethod]
		public void PerformanceNoTrainHelper()
		{
			var mock = new Mock<I>();
			mock.Setup(i => i.MeP.MeM().MeP.ValueP).Returns("ja");

			for (int i = 0; i < 1000000; i++)
			{
				var value = mock.Object.MeP.MeM().MeP.ValueP;
			}
		}

		[TestMethod]
		public void PerformanceNoTrainHelperKoray()
		{
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
