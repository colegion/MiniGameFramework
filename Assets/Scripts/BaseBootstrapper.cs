using System.Collections;
using System.Linq;
using Controllers;
using Helpers;
using Interfaces;
using Pool;
using UnityEngine;
using Grid = GridSystem.Grid;

public abstract class BaseBootstrapper : MonoBehaviour
{
    public abstract IEnumerator InitializeDependencies();
}