using HellTap.PoolKit;

namespace Project.Scripts.Shared
{
    public static class Pools
    {
        public static Pool ElementsPool
        {
            get
            {
                if (!_elementsPool)
                    _elementsPool = PoolKit.FindPool("Elements");

                return _elementsPool;
            }
        }
        
        public static Pool UiPool
        {
            get
            {
                if (!_uiPool)
                    _uiPool = PoolKit.FindPool("Ui");

                return _uiPool;
            }
        }
        
        private static Pool _elementsPool;
        private static Pool _uiPool;
    }
}