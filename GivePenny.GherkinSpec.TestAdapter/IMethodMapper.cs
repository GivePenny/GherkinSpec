using GivePenny.GherkinSpec.Model;
using System.Reflection;

namespace GivePenny.GherkinSpec.TestAdapter
{
    public interface IMethodMapper
    {
        IMethodMapping GetMappingFor(IStep step, Assembly testAssembly);
    }
}
