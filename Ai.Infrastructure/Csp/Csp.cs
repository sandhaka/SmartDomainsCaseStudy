using System;
using System.Collections.Generic;
using System.Linq;
using Ai.Infrastructure.Csp.Model;
using Ai.Infrastructure.Csp.Resolvers;
using Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch;
using Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch.Parametric;

namespace Ai.Infrastructure.Csp
{
    /// <summary>
    /// Constraint Satisfaction Problem Resolver
    /// </summary>
    /// <typeparam name="T">Domain base type</typeparam>
    public class Csp<T>
        where T : CspValue
    {
        private readonly CspModel<T> _model;
        private IResolver<T> _resolver;
        private IArcConsistency<T> _arcConsistency;
        private int _nAssigns;

        internal Csp(
            IDictionary<string, IEnumerable<T>> domains,
            IDictionary<string, IEnumerable<string>> relations,
            IEnumerable<Func<string, T, string, T, bool>> constraints
        )
        {
            var variables = domains.Select(d => new Variable<T>(d.Key)).ToList();

            _model = new CspModel<T>(
                variables,
                domains.Select(d => new Domain<T>(d.Key, d.Value)).ToList(),
                relations.Select(d => new Relations<T>(d.Key, variables.Where(v => d.Value.Contains(v.Key)))).ToList(),
                constraints.Select(d => new Constraint<T>(d)).ToList()
            );
        }

        #region [ Model wrappers ]

        internal CspModel<T> Model => _model;

        public bool Resolved => _model.Resolved;

        /// <summary>
        /// Remove a variable assigment
        /// </summary>
        /// <param name="variableKey">Key</param>
        public Csp<T> RemoveAssignment(string variableKey)
        {
            _model.Revoke(variableKey);
            return this;
        }

        /// <summary>
        /// Variable assigment
        /// </summary>
        /// <param name="variableKey">Key</param>
        /// <param name="value">Value</param>
        public Csp<T> AddAssignment(string variableKey, T value)
        {
            _model.Assign(variableKey, value);
            _nAssigns++;
            return this;
        }

        /// <summary>
        /// Auto assign each variable with the first value taken from his domain
        /// </summary>
        public void AutoAssignment() => _model.AutoAssign();

        /// <summary>
        /// Shrink domain to the current assignment value
        /// </summary>
        /// <param name="key">Key</param>
        public void ShrinkDomainToAssignment(string key) => _model.ShrinkDomainToAssignment(key);

        /// <summary>
        /// Sort domain variables and perform the auto assignment
        /// </summary>
        public void SortBeforeAutoAssignment() => _model.SortAndAutoAssign();

        /// <summary>
        /// Random assignment variable value from current domain
        /// </summary>
        public void RandomAssignment() => _model.RandomAssign();

        /// <summary>
        /// Compute number of conflicts by given value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value to test</param>
        /// <returns>Number of conflicts</returns>
        public int Conflicts(string key, T value) => _model.Conflicts(key, value);

        /// <summary>
        /// To anonymous model
        /// </summary>
        /// <returns>Json data</returns>
        public string ShowModelAsJson() => _model.ToJson();

        #endregion

        #region [ Api ]

        /// <summary>
        /// Total assignment made
        /// </summary>
        public int NumberOfTotalAssignments => _nAssigns;

        /// <summary>
        /// Get the list of pruned variable domain values
        /// </summary>
        public Dictionary<string, T> Pruned => _model.PrunedDomainValues.ToDictionary(d => d.Key, d => d.Value);

        /// <summary>
        /// Current model status (variable list)
        /// </summary>
        public Dictionary<string, T> Status => _model.Status();

        /// <summary>
        /// Set AC3 algorithm as resolver,
        /// AC3 enforce arc consistency until a valid configuration is reached (only legal values in the domains)
        /// </summary>
        public Csp<T> UseAc3AsResolver()
        {
            _arcConsistency = new Ac3<T>();
            return this;
        }

        /// <summary>
        /// Set BackTracking as resolver,
        /// BT search iteratively a solution by assign and check conflicts systematically
        /// </summary>
        /// <param name="selectStrategyType">Variable selection strategy</param>
        /// <param name="domainOrderingStrategyType">Domain values ordering strategy</param>
        /// <param name="infStrategyType">Inference strategy</param>
        public Csp<T> UseBackTrackingSearchResolver(
            string selectStrategyType = "",
            string domainOrderingStrategyType = "",
            string infStrategyType = "")
        {
            var infType = Type.GetType(infStrategyType) ?? typeof(NoInference<T>);
            var domainOrdType = Type.GetType(domainOrderingStrategyType) ?? typeof(UnorderedDomainValues<T>);
            var selectType = Type.GetType(selectStrategyType) ?? typeof(FirstUnassignedVariable<T>);

            _resolver = new BackTrackingSearch<T>(
                (ISelectUnassignedVariableStrategy<T>) Activator.CreateInstance(selectType),
                (IDomainValuesOrderingStrategy<T>) Activator.CreateInstance(domainOrdType),
                (IInferenceStrategy<T>) Activator.CreateInstance(infType));

            return this;
        }

        /// <summary>
        /// Use Hill-Climbing based resolver
        /// </summary>
        public Csp<T> UseMinConflictsResolver()
        {
            _resolver = new MinConflicts<T>();
            return this;
        }

        /// <summary>
        /// Execute resolver
        /// </summary>
        /// <param name="whenResolved">On resolved callback (on success)</param>
        /// <returns>Resolved</returns>
        public bool Resolve(Action whenResolved = null)
        {
            var resolved = _resolver?.Resolve(this) ??
                           throw new InvalidOperationException("A resolver must be set");

            if (resolved)
            {
                whenResolved?.Invoke();
            }

            return resolved;
        }

        /// <summary>
        /// Enforce arc consistency
        /// </summary>
        /// <param name="whenEnsured">On arc consistency resolved callback</param>
        /// <returns>Reached</returns>
        public bool PropagateArcConsistency(Action whenEnsured = null)
        {
            var propagated = _arcConsistency?.Propagate(this) ??
                             throw new InvalidOperationException("An arc consistency method must be set");

            if (propagated)
            {
                whenEnsured?.Invoke();
            }

            return propagated;
        }

        #endregion
    }
}