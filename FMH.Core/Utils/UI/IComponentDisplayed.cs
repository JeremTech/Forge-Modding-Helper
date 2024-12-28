using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Utils.UI
{
    public interface IComponentDisplayed
    {
        /// <summary>
        /// Code to execute when control is displayed
        /// </summary>
        /// <param name="args">Arguments</param>
        void OnComponentDisplayed(params object[] args);
    }
}
