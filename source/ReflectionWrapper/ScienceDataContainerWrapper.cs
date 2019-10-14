using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerboKatz.ReflectionWrapper
{
    public class ScienceDataContainerWrapper : ScienceDataContainerWrapper<IScienceDataContainer>
    {
        public ScienceDataContainerWrapper(IScienceDataContainer instance) 
            : base(instance)
        {
        }
    }
    public class ScienceDataContainerWrapper<T> : ReflectionHelper<T>
        where T: IScienceDataContainer
    {
        public ScienceDataContainerWrapper(T instance) : base(instance)
        {
        }
        public ScienceData[] GetData() => GetMethodResult(e => e.GetData());
        public int GetScienceCount() => GetMethodResult(e => e.GetScienceCount());
        public void ReturnData(ScienceData data) => ExecuteMethod(e => e.ReturnData(data));
        public void DumpData(ScienceData data) => ExecuteMethod(e => e.DumpData(data));
        public bool IsRerunnable() => GetMethodResult(e => e.IsRerunnable());
    }
}
