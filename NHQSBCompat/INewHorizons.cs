using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace NHQSBCompat
{
    public interface INewHorizons
    {
        UnityEvent<string> GetChangeStarSystemEvent();

        bool ChangeCurrentStarSystem(string name);
    }
}
