using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerboKatz
{
    using System.Reflection;
    using UniLinq;

    public static partial class Utilities
    {
        public static partial class Science
        {
            private static MethodInfo _getScienceValueGenericMethod;

            public static void addToExperimentCount(ref Dictionary<string, int> experimentCount, ScienceSubject scienceSubject)
            {
                if (experimentCount.ContainsKey(scienceSubject.id))
                {
                    experimentCount[scienceSubject.id] = experimentCount[scienceSubject.id] + 1;
                }
                else
                {
                    experimentCount.Add(scienceSubject.id, 1);
                }
            }

            public static float GetScienceValue(Dictionary<string, int> experimentCount, ScienceExperiment experiment,
                ScienceSubject currentScienceSubject,
                Func<ScienceExperiment, ScienceSubject, float> GetScienceValue = null,
                Func<ScienceExperiment, ScienceSubject, float> GetNextScienceValue = null)
            {
                //Call via generic method to account for "new" modifiers in custom implementations of ScienceExperiment
                var args = new object[]
                {
                    experimentCount,
                    experiment,
                    currentScienceSubject,
                    GetScienceValue,
                    GetNextScienceValue
                };

                var m = _getScienceValueGenericMethod ?? (_getScienceValueGenericMethod =
                            typeof(Science).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                .First(e => e.IsGenericMethod && e.Name == nameof(GetScienceValue) && e.GetParameters().Length == args.Length));
                return (float)m.MakeGenericMethod(experiment?.GetType() ?? typeof(ScienceExperiment)).Invoke(null, args);
            }

            public static float GetScienceValue<T>(Dictionary<string, int> experimentCount, T experiment, ScienceSubject currentScienceSubject, Func<ScienceExperiment, ScienceSubject, float> GetScienceValue = null, Func<ScienceExperiment, ScienceSubject, float> GetNextScienceValue = null)
            where T : ScienceExperiment
            {
                float currentScienceValue;
                if (experimentCount.ContainsKey(currentScienceSubject.id))
                {
                    if (GetNextScienceValue == null)
                        GetNextScienceValue = Science.GetNextScienceValue;
                    currentScienceValue = GetNextScienceValue(experiment, currentScienceSubject);
                    if (experimentCount[currentScienceSubject.id] >= 2)//taken from scienceAlert to get somewhat accurate science values after the second experiment
                        currentScienceValue = currentScienceValue / Mathf.Pow(4f, experimentCount[currentScienceSubject.id] - 1);
                }
                else
                {
                    if (GetScienceValue == null)
                        GetScienceValue = Science.GetScienceValue;
                    currentScienceValue = GetScienceValue(experiment, currentScienceSubject);
                }
                return currentScienceValue;
            }

            private static float GetScienceValue<T>(T experiment, ScienceSubject currentScienceSubject)
                where T : ScienceExperiment
            {
                return ResearchAndDevelopment.GetScienceValue(experiment.baseValue * experiment.dataScale, currentScienceSubject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
            }

            private static float GetNextScienceValue<T>(T experiment, ScienceSubject currentScienceSubject)
                where T : ScienceExperiment
            {
                return ResearchAndDevelopment.GetNextScienceValue(experiment.baseValue * experiment.dataScale, currentScienceSubject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
            }
        }
    }
}