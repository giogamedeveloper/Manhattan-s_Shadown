using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class TranslateManager : MonoBehaviour
{
    [Header("References text Buttons")]
    // public TextMeshProUGUI[] _startText;
    //
    // [SerializeField] string[] _texts;
    public string defaultLanguage = "Spanish";

    public Dictionary<string, string> texts;

    private static TranslateManager _instance;
    public static TranslateManager Instance => _instance;

    public static Action OnLanguageChanged;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLenguage();
    }

    private void LoadLenguage()
    {
        string systemLanguage = Application.systemLanguage.ToString();
        TextAsset textAsset = Resources.Load<TextAsset>(systemLanguage);
        if (textAsset == null)
        {
            textAsset = Resources.Load<TextAsset>(defaultLanguage);
        }

        //Creamos una variable de tipo XmlDocument para gestionar la lectura del XML
        XmlDocument xmlDoc = new XmlDocument();
        //Cargamos el XML desde el fichero de texto
        xmlDoc.LoadXml(textAsset.text);
        //Llamamos al método que carga los textos y sus "Keys" para el idioma
        LoadTexts(xmlDoc);
    }

    void LoadTexts(XmlDocument xmlDoc)
    {
        //Inicializamos el diccionario
        texts = new Dictionary<string, string>();
        //Recuperamos el bloque con el idioma seleccionado
        XmlElement element = xmlDoc.DocumentElement["lang"];
        if (element == null)
        {
            Debug.LogError("No se encontró el elemento 'lang' en el XML.");
            return;
        }
        foreach (XmlNode node in element.ChildNodes)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                XmlElement xmlItem = (XmlElement)node;
                string key = xmlItem.GetAttribute("key");
                string value = xmlItem.InnerText;
                if (!string.IsNullOrEmpty(key))
                {
                    if (!texts.ContainsKey(key))
                    {
                        texts.Add(key, value);
                    }
                    else
                    {
                        Debug.LogWarning($"Clave duplicada en XML: {key}. Se omite o reemplaza según configuración.");
                        // Opcional: reemplazar valor
                        // texts[key] = value;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Devuelve el tecto que coincida con la key proporcionada
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetText(string key)
    {
        //Si no existe la clave indicada
        if (!texts.ContainsKey(key))
        {
            //mostramos un warning y retornamos la key tal cual
            Debug.LogWarning("La key" + key + "no existe.");
            return key;
        }
        //Si existe, devolvemos el texto correspondiente directamente
        return texts[key];
    }

    public void ChangeLanguage(string lenguage)
    {
        string systemLanguage = Application.systemLanguage.ToString();
        TextAsset textAsset = Resources.Load<TextAsset>(lenguage);
        if (textAsset == null)
        {
            textAsset = Resources.Load<TextAsset>(lenguage);
        }
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);
        LoadTexts(xmlDoc);
        OnLanguageChanged?.Invoke();
    }
}
