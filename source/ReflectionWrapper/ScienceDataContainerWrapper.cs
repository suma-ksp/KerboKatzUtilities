namespace KerboKatz.ReflectionWrapper
{
    public class ScienceDataContainerWrapper
        : ScienceDataContainerWrapper<IScienceDataContainer>
    {
        public ScienceDataContainerWrapper(IScienceDataContainer baseObject)
            : base(baseObject)
        {
        }
    }

    public class ScienceDataContainerWrapper<T>
        where T : IScienceDataContainer
    {
        public ScienceDataContainerWrapper(T baseObject)
        {
            BaseObject = baseObject;
        }

        public T BaseObject { get; }

        public ScienceData[] GetData() => BaseObject.ReflectionExecute(e => e.GetData());

        public int GetScienceCount() => BaseObject.ReflectionExecute(e => e.GetScienceCount());

        public void ReturnData(ScienceData data) => BaseObject.ReflectionExecute(e => e.ReturnData(data));

        public void DumpData(ScienceData data) => BaseObject.ReflectionExecute(e => e.DumpData(data));

        public bool IsRerunnable() => BaseObject.ReflectionExecute(e => e.IsRerunnable());
    }
}