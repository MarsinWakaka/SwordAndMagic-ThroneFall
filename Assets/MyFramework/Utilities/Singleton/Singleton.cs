namespace MyFramework.Utilities.Singleton
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
    
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    
        // 防止外部实例化
        protected Singleton() {}
    }
}