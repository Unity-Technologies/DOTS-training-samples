using NUnit.Framework;
using Unity.Platforms;

class BasicTests
{
	[Test]
	public void VerifyCanReferenceEditorBuildTarget()
	{
		Assert.IsNotNull(typeof(EditorBuildTarget));
	}
}
