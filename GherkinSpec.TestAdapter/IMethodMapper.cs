using GherkinSpec.Model;
using System.Reflection;

namespace GherkinSpec.TestAdapter
{
    public interface IMethodMapper
    {
        IMethodMapping GetMappingFor(IStep step, Assembly testAssembly);
    }
}
