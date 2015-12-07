using System;
using System.Collections.Generic;

using deimors.ruleengine.api;

namespace deimors.ruleengine.impl {
	public class RuleEngine<T> : IRuleEngine<T> {
		#region Internal data
		private Dictionary<IRule<T>, ICollection<IRule<T>>> attacks = new Dictionary<IRule<T>, ICollection<IRule<T>>>();

		private ICollection<IRule<T>> undecRules = new HashSet<IRule<T>>();
		private ICollection<IRule<T>> inRules = new HashSet<IRule<T>>();
		private ICollection<IRule<T>> outRules = new HashSet<IRule<T>>();
		
		private ICollection<IRule<T>> newInRules = new HashSet<IRule<T>>();
		private ICollection<IRule<T>> newOutRules = new HashSet<IRule<T>>();
		#endregion

		#region IRuleEngine<T> implementation
		public void AddRule(IRule<T> rule) {
			if (attacks.ContainsKey(rule)) {
				throw new ArgumentException(rule + " already added");
			}

			attacks[rule] = new HashSet<IRule<T>>();
		}

		public void AddRules(params IRule<T>[] rules) {
			foreach (IRule<T> rule in rules) {
				AddRule(rule);
			}
		}

		public void AddAttack(IRule<T> from, IRule<T> to) {
			if (!attacks.ContainsKey(from)) {
				throw new ArgumentException(from + " not in rules");
			}

			if (!attacks.ContainsKey(to)) {
				throw new ArgumentException(to + " not in rules");
			}

			attacks[from].Add(to);
		}

		public void AddAttacks(IRule<T> from, params IRule<T>[] toRules) {
			foreach (IRule<T> to in toRules) {
				AddAttack(from, to);
			}
		}

		public void RunJustifiedActions(T data) {
			undecRules.Clear();
			inRules.Clear();
			outRules.Clear();

			AddSatisfiedRules(data, undecRules);

			do {
				newInRules.Clear();
				newOutRules.Clear();

				AddUndecAttackedByIn(newOutRules);

				foreach (IRule<T> newOutRule in newOutRules) {
					undecRules.Remove(newOutRule);
					outRules.Add(newOutRule);
				}

				AddUndecAllAttacksOut(newInRules);

				foreach (IRule<T> newInRule in newInRules) {
					undecRules.Remove(newInRule);
					inRules.Add(newInRule);
				}
			} while (newOutRules.Count > 0 || newInRules.Count > 0);

			foreach (IRule<T> rule in inRules) {
				rule.Action(data);
			}
		}
		#endregion

		#region Internal methods
		private void AddUndecAttackedByIn(ICollection<IRule<T>> targets) {
			foreach (IRule<T> inRule in inRules) {
				foreach (IRule<T> target in attacks[inRule]) {
					if (undecRules.Contains(target)) {
						targets.Add(target);
					}
				}
			}
		}

		private void AddUndecAllAttacksOut(ICollection<IRule<T>> targets) {
			foreach (IRule<T> undecRule in undecRules) {
				if (NoAttackersIn(undecRule, inRules) && NoAttackersIn(undecRule, undecRules)) {
					targets.Add(undecRule);
				}
			}
		}

		private void AddSatisfiedRules(T data, ICollection<IRule<T>> satisfied) {
			foreach (IRule<T> rule in attacks.Keys) {
				if (rule.Condition(data)) {
					satisfied.Add(rule);
				}
			}
		}

		private bool NoAttackersIn(IRule<T> rule, ICollection<IRule<T>> rules) {
			foreach (IRule<T> fromRule in attacks.Keys) {
				if (attacks[fromRule].Contains(rule) && rules.Contains(fromRule)) {
					return false;
				}
			}

			return true;
		}
		#endregion
	}
}