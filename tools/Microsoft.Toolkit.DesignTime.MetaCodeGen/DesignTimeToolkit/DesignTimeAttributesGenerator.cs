using System;

using Microsoft.CodeAnalysis;

namespace Microsoft.Toolkit.DesignTime
{
	public abstract class DesignTimeAttributesGenerator : IDesignTimeAttributesGenerator
	{
		public virtual void Execute(GeneratorExecutionContext context)
		{
			throw new NotImplementedException();
		}

		public virtual void Initialize(GeneratorInitializationContext context)
		{
			throw new NotImplementedException();
		}
	}
}