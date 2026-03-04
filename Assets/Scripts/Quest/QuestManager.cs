using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public GameObject questPanel; // Panel que contiene las quests
    public GameObject questPrefab; // Prefab para mostrar cada quest
    public GameObject subQuestPrefab; // Prefab para mostrar cada subquest
    [SerializeField] float timeToShow;

    [SerializeField]
    private Animator _canvasUIQuestAnimator;

    public List<Quest> quests;
    private bool showQuests = false;
    private Coroutine _showHideCoroutine;

    private static QuestManager _instance;
    public static QuestManager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        quests ??= new List<Quest>();
        LoadQuests();
    }

    public void CrearMision(string questID, string title, List<(string subQuestID, string description)> subQuestsData)
    {
        // Crear la nueva quest
        Quest nuevaQuest = ScriptableObject.CreateInstance<Quest>();
        nuevaQuest.title = title;
        nuevaQuest.questID = questID;
        nuevaQuest.subQuests = new List<SubQuest>();
        nuevaQuest.isCompleted = false;

        // Agregar las subquests
        foreach (var subQData in subQuestsData)
        {
            nuevaQuest.subQuests.Add(new SubQuest
            {
                subQuestID = subQData.subQuestID,
                description = subQData.description,
                isCompleted = false
            });
        }

        // Añadirla a la lista de quests
        quests.Add(nuevaQuest);
        PopulateQuestUI(); // Actualizar UI
        SaveQuests();
    }

    public void ShowHideCanvas()
    {
        showQuests = !showQuests;
        // Actualizar la visibilidad y la interacción
        canvasGroup.gameObject.SetActive(showQuests);
        canvasGroup.alpha = showQuests ? 1 : 0;
        canvasGroup.blocksRaycasts = showQuests;
        canvasGroup.interactable = showQuests;

        // Mostrar u ocultar las quests tras una demora
        if (_showHideCoroutine != null)
            StopCoroutine(_showHideCoroutine);
        _showHideCoroutine = StartCoroutine(DelayToShowQuest(showQuests));

        // Animación UIS
        _canvasUIQuestAnimator.SetBool("ShowQuest", showQuests);

    }

    IEnumerator DelayToShowQuest(bool show)
    {
        yield return new WaitForSeconds(timeToShow);
        questPanel.SetActive(show);
        if (show)
        {
            PopulateQuestUI(); // Rellenar UI cuando se muestre
        }
    }

    void PopulateQuestUI()
    {
        // Limpiar la UI antes de rellenar
        foreach (Transform child in questPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Crear cada quest y sus subquests
        foreach (var quest in quests)
        {
            // Instanciar y asignar título
            GameObject qGO = Instantiate(questPrefab, questPanel.transform);
            TextMeshProUGUI qText = qGO.GetComponent<TextMeshProUGUI>();
            qText.text = quest.title;
            // Cambiar color si está completada
            qText.color = quest.isCompleted ? Color.gray : Color.black;

            foreach (var subQ in quest.subQuests)
            {
                // Instanciar subquest
                GameObject sGO = Instantiate(subQuestPrefab, qGO.transform);
                TextMeshProUGUI sText = sGO.GetComponent<TextMeshProUGUI>();
                sText.text = (subQ.isCompleted ? "<color=#008000>✓</color> " : "<color=#FF0000>⨉</color> ") + subQ.description;

                // Agregar botón para marcar como completo
                Button btn = sGO.GetComponentInChildren<Button>();
                if (btn != null)
                {
                    SubQuest subQRef = subQ; // Para evitar closures
                    btn.onClick.RemoveAllListeners(); // Limpiar listeners existentes
                    btn.onClick.AddListener(() =>
                    {
                        subQRef.isCompleted = true;
                        CheckQuestCompletion(quest);
                        PopulateQuestUI();
                        SaveQuests();
                    });
                }
            }
        }
    }

    void CheckQuestCompletion(Quest quest)
    {
        if (quest.subQuests.All(sq => sq.isCompleted))
        {
            quest.isCompleted = true; // Marcar la quest como completada
            // Aquí podrías agregar más acciones al completar una quest
        }
    }

    public void MarkSubQuestCompleted(string questID, string subQuestID)
    {
        var quest = quests.FirstOrDefault(q => q.questID == questID);
        if (quest != null)
        {
            var subQ = quest.subQuests.FirstOrDefault(sq => sq.subQuestID == subQuestID);
            if (subQ != null)
            {
                subQ.isCompleted = true;
                CheckQuestCompletion(quest);
                PopulateQuestUI(); // Actualiza UI
                SaveQuests(); // Guarda estado
            }
        }
    }

    void SaveQuests()
    {
        QuestDataWrapper wrapper = new QuestDataWrapper
        {
            quests = quests.Select(q => new QuestData
            {
                questID = q.questID,
                title = q.title,
                isCompleted = q.isCompleted,
                subQuests = q.subQuests.Select(sq => new SubQuestData
                {
                    subQuestID = sq.subQuestID,
                    description = sq.description,
                    isCompleted = sq.isCompleted
                }).ToList()
            }).ToList()
        };

        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("questsData", json);
        PlayerPrefs.Save();
    }

    void LoadQuests()
    {
        if (PlayerPrefs.HasKey("questsData"))
        {
            string json = PlayerPrefs.GetString("questsData");
            var data = JsonUtility.FromJson<QuestDataWrapper>(json);
            if (data == null || data.quests == null) return;
            quests.Clear();
            foreach (var qData in data.quests)
            {
                Quest q = ScriptableObject.CreateInstance<Quest>();
                q.questID = qData.questID;
                q.title = qData.title;
                q.isCompleted = qData.isCompleted;
                q.subQuests = new List<SubQuest>();
                foreach (var sqData in qData.subQuests)
                {
                    q.subQuests.Add(new SubQuest()
                    {
                        subQuestID = sqData.subQuestID,
                        description = sqData.description,
                        isCompleted = sqData.isCompleted
                    });
                }
                quests.Add(q);
            }
            // Después de cargar, actualiza la UI
            PopulateQuestUI();
        }
    }

    public void MarKAllAsNoCompleted()
    {
        foreach (var quest in quests)
        {
            quest.isCompleted = false; // Marcar la quest como no completada
            foreach (var subQ in quest.subQuests)
            {
                subQ.isCompleted = false; // Marcar cada subquest como no completada
            }
        }
        PopulateQuestUI(); // Actualizar la interfaz de usuario
        SaveQuests(); // Guardar el estado
    }
}
