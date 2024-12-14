using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Utils.UI
{
    public interface IComponentValidated
    {
        /// <summary>
        /// Code to execute when control is validated
        /// </summary>
        /// <returns><c>true</c> if all data is validated, else <c>false</c></returns>
        bool ValidateData();
    }
}
