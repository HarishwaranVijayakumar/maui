using Microsoft.Maui.Accessibility;
using Xunit;

namespace Tests
{
	public class SemanticScreenReader_Tests
	{
		[Fact]
		public void SemanticScreenReader_Announce_DoesNotThrow_On_NetStandard()
		{
			// Test that calling Announce doesn't throw an exception on NetStandard
			// This should not throw NotImplementedInReferenceAssemblyException
			var exception = Record.Exception(() => SemanticScreenReader.Announce("Test announcement"));
			Assert.Null(exception);
		}

		[Fact]
		public void SemanticScreenReader_Default_IsNotNull()
		{
			// Ensure the Default implementation is available
			var defaultInstance = SemanticScreenReader.Default;
			Assert.NotNull(defaultInstance);
		}

		[Fact]
		public void SemanticScreenReader_Default_Announce_DoesNotThrow()
		{
			// Test that calling Default.Announce doesn't throw an exception
			// This tests the specific scenario from the issue where it would throw
			// NullReferenceException when called early in app lifecycle
			var exception = Record.Exception(() => SemanticScreenReader.Default.Announce("Test announcement"));
			Assert.Null(exception);
		}
	}
}