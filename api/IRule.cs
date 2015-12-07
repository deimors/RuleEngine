namespace deimors.ruleengine.api {
	public interface IRule<T> {
		bool Condition(T data);
		void Action(T data);
	}
}