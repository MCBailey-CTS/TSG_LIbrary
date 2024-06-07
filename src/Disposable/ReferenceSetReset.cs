using System;
using System.Collections.Generic;
using NXOpen.Assemblies;

namespace TSG_Library.Disposable
{
    public class ReferenceSetReset : IDisposable
    {
        private readonly IDictionary<Component, string> _compStringDictionary;


        public ReferenceSetReset(IEnumerable<Component> components)
        {
            _compStringDictionary = new Dictionary<Component, string>();

            foreach (var component in components)
            {
                // Checks to see if the {component} is already in the {_compStringDictionary}.
                // If it is we can ignore it.
                if(_compStringDictionary.ContainsKey(component))
                    continue;

                // If we get here then we want to add the {component} and it's current reference set to the {_compStringDictionary}.
                _compStringDictionary.Add(component, component.ReferenceSet);
            }
        }

        public ReferenceSetReset(params Component[] components)
            : this((IEnumerable<Component>)components)
        {
        }

        public void Dispose()
        {
            foreach (var keyComponent in _compStringDictionary.Keys)
            {
                keyComponent._ReferenceSet(_compStringDictionary[keyComponent]);
                keyComponent.RedisplayObject();
            }
        }
    }
}