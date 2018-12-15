using GherkinSpec.Model;
using System.Reflection;

namespace GherkinSpec.TestAdapter.Binding
{
    public interface IStepBinder
    {
        IStepBinding GetBindingFor(IStep step, Assembly testAssembly);
    }
}
