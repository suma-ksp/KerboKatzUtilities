using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerboKatz.ReflectionWrapper
{
    public class ModuleScienceExperimentReflectionWrapper 
        : ModuleScienceExperimentReflectionWrapper<ModuleScienceExperiment>
    {
        public ModuleScienceExperimentReflectionWrapper(ModuleScienceExperiment experiment) : base(experiment)
        {
        }
    }
    public class ModuleScienceExperimentReflectionWrapper<T> : ScienceDataContainerWrapper<T>
        where T: ModuleScienceExperiment
    {
        public readonly ModuleScienceExperiment baseExperiment;
        public string experimentID => GetMemberWithReflection(e => e.experimentID);
        public ScienceExperiment experiment => GetMemberWithReflection(e => e.experiment);

        public bool Inoperable => GetMemberWithReflection(e => e.Inoperable);

        public bool Deployed => GetMemberWithReflection(e => e.Deployed);
        public bool resettable => GetMemberWithReflection(e => e.resettable);
        public bool rerunnable => GetMemberWithReflection(e => e.rerunnable);

        public bool useStaging
        {
            get => GetMemberWithReflection(e => e.useStaging);
            set => SetMemberWithReflection(e => e.useStaging, value);
        }

        public ModuleScienceExperimentReflectionWrapper(T experiment)
            : base(experiment)
        {
            baseExperiment = experiment;
        }

        public void OnActive() => ExecuteMethod(e => e.OnActive());
        public void DeployExperiment() => ExecuteMethod(e => e.DeployExperiment());
        public void ResetExperiment() => ExecuteMethod(e => e.ResetExperiment());
    }
}
