# Custom Binding Attribute Types

This feature intends to allow the benefits of GherkinSpec (e.g. running test cases in parallel) to be leveraged when using binding attributes from other frameworks (or even entirely custom binding attributes).

## Usage

To specify the binding attribute types that should be used by GherkinSpec when binding Gherkin steps to C# methods, [`BindingTypesAttribute`](../GherkinSpec.TestModel/BindingTypesAttribute.cs) may be used.

When using this attribute, the `assembly` target should be specified. See [CustomBindingAttributesUnitTests.AssemblyAttributes](../GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests/AssemblyAttributes.cs) and the referenced types for an example of the intended usage.

When specifying custom binding attribute types, a custom implementation of [`IStepAttributeMatchExpressionSelector`](../GherkinSpec.TestModel/IStepAttributeMatchExpressionSelector.cs) must also be provided to allow GherkinSpec to select the regular expression string from the binding attribute.
See [CustomStepAttributeMatchExpressionSelector](../GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests/CustomStepAttributeMatchExpressionSelector.cs) for the implementation used in the above CustomBindingAttributesUnitTests example.