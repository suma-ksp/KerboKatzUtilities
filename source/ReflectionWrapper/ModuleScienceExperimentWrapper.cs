namespace KerboKatz.ReflectionWrapper
{
    public class ModuleScienceExperimentWrapper
        : ModuleScienceExperimentWrapper<ModuleScienceExperiment>
    {
        public ModuleScienceExperimentWrapper(ModuleScienceExperiment experiment) : base(experiment)
        {
        }
    }

    public class ModuleScienceExperimentWrapper<T> : ScienceDataContainerWrapper<T>
        where T : ModuleScienceExperiment
    {
        public ModuleScienceExperimentWrapper(T experiment)
            : base(experiment)
        {
        }

        public bool useStaging
        {
            get => BaseObject.ReflectionExecute(e => e.useStaging);
            set => BaseObject.ReflectionAssign(e => e.useStaging, value);
        }

        public string experimentID => BaseObject.ReflectionExecute(e => e.experimentID);
        public ScienceExperiment experiment => BaseObject.ReflectionExecute(e => e.experiment);

        public bool Inoperable => BaseObject.ReflectionExecute(e => e.Inoperable);

        public bool Deployed => BaseObject.ReflectionExecute(e => e.Deployed);
        public bool resettable => BaseObject.ReflectionExecute(e => e.resettable);
        public bool rerunnable => BaseObject.ReflectionExecute(e => e.rerunnable);

        public void OnActive() => BaseObject.ReflectionExecute(e => e.OnActive());

        public void DeployExperiment() => BaseObject.ReflectionExecute(e => e.DeployExperiment());

        public void ResetExperiment() => BaseObject.ReflectionExecute(e => e.ResetExperiment());
    }
}