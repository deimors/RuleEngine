using System;

using deimors.ruleengine.api;

namespace deimors.ruleengine.impl {
	public class Rule<T> : IRule<T> {
		private Predicate<T> condition;
		private Action<T> action;

		public Rule(Predicate<T> condition, Action<T> action) {
			this.condition = condition;
			this.action = action;
		}

		#region IRule<T> implementation
		public bool Condition(T data) {
			return condition(data);
		}

		public void Action(T data) {
			action(data);
		}
		#endregion
	}
}