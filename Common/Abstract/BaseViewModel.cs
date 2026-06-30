using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.ViewModels
{
    public abstract class BaseViewModel
    {
        public BaseViewModel() 
        {
            InitializeObjects();
        }

        public abstract void InitializeObjects();
    }
}
