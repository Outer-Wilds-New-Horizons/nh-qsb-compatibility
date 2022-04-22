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
        string GetCurrentStarSystem();
        
        UnityEvent<string> GetChangeStarSystemEvent();

        bool ChangeCurrentStarSystem(string name);
    }
}
