using System.Collections.Generic;

public class CreateMision : Reaction
{
    List<(string, string)>
        subquests1 = new List<(string, string)>
        {
            ("SQ1", "Recolectar tres productos, limón, papa y ajo"),
            ("SQ2", "Hablar con NPC")
        };

    List<(string, string)>
        subquests2 = new List<(string, string)>
        {
            ("SQ1", "Buscar las notas en las tres fuentes"),
            ("SQ2", "Abre el baúl con el código")
        };

    List<(string, string)>
        subquests3 = new List<(string, string)>
        {
            ("SQ1", "Realiza el puzzle para desenmascarar a Lombardi"),
        };

    protected override void React()
    {
        if (GameController.Instance.isActiveMision)
        {
            GameController.Instance.mision++;
            GameController.Instance.stateToTask = 0;
            GameController.Instance.isActiveMision = false;
            switch (GameController.Instance.mision)
            {
                case 1:
                    QuestManager.Instance.CrearMision("Q1", "Misión 1", subquests1);
                    break;
                case 2:
                    QuestManager.Instance.CrearMision("Q2", "Misión 2", subquests2);
                    break;
                case 3:
                    QuestManager.Instance.CrearMision("Q3", "Misión 3", subquests3);
                    break;
            }
        }
    }
}
