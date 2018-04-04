using System.Collections;

using uPromise;

namespace Framework
{
    public interface IPopupWindow
    {
        Promise In();
        Promise Out();
    }
}