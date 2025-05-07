namespace MyFramework.Utilities.FSM
{
    public interface IStateParams{}
    
    public interface IState
    {
        public void OnEnter(IStateParams stateParams);
        public void OnUpdate();
        public void OnExit(IStateParams stateParams);
    }
    
    public class EmptyState : IState
    {
        public void OnEnter(IStateParams stateParams)
        {
            // Do nothing
        }

        public void OnUpdate()
        {
            // Do nothing
        }

        public void OnExit(IStateParams stateParams)
        {
            // Do nothing
        }
    }
}