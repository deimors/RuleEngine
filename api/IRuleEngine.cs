using System.Collections.Generic;

namespace deimors.ruleengine.api {
	public interface IRuleEngine<T> {
		void AddRule(IRule<T> rule);
		void AddRules(params IRule<T>[] rules);

		void AddAttack(IRule<T> from, IRule<T> to);
		void AddAttacks(IRule<T> from, params IRule<T>[] toRules);

		void RunJustifiedActions(T data);
	}
}