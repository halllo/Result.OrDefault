
namespace ResultOrDefault.Tests
{
	public interface I
	{
		string ValueP { get; }
		string ValueM();
		I MeP { get; }
		I MeM();
	}
}
