using UnityEngine;

[System.Serializable]
public class Transition
{
    //Decisión a evaluar
    public Decision decision;

    //Acción a realizar en caso de que se cumpla la decisión.
    public State trueState;

    //Acción a realizar en caso de que no se cumpla la decisión
    public State falseState;
}
