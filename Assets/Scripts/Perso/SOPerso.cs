using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Code provient des capslule
/// Modifier par Albert Jannard
/// Scriptable Object qui contient les données du perso
/// </summary>

[CreateAssetMenu(fileName = "Perso", menuName = "Perso")]
public class SOPerso : ScriptableObject
{
    [SerializeField] private Dictionary<SOObjet, int> _objetsPossedes;
    public Dictionary<SOObjet, int> ObjetsPossedes => _objetsPossedes;
    [Header("Événement Bonus")]
    private UnityEvent _bonusActivated = new UnityEvent();
    public UnityEvent bonusActivated => _bonusActivated;

    [Header("Paramètres Initiaux")]
    [SerializeField][Range(1, 5)] private int _niveauIni = 1;
    [SerializeField][Range(0, 500)] private int _argentIni = 100;
    [SerializeField][Range(1, 10)] private int _vieIni = 5;

    [Header("Paramètres Actuels")]
    [SerializeField][Range(1, 100)] private int _niveau = 1;
    [SerializeField][Range(0, 500)] private int _argent = 100;
    [SerializeField][Range(0, 10)] private int _vie = 5;

    private int _vieMax = 10;
    public int vieMax { get => _vieMax; set => _vieMax = value; }

    [Header("Paramètres Shuriken")]
    [SerializeField][Range(0, 45)] private int _nbShurikenMax = 35;
    public int nbShurikenMax => _nbShurikenMax;
    [SerializeField][Range(0, 15)] private int _nbShurikenInit = 10;
    private int _nbShuriken;

    public int nbShuriken { get => _nbShuriken; set => _nbShuriken = value; }

    [Header("Paramètres Potions De Vie")]
    [SerializeField][Range(0, 5)] private int _nbPotionsMax = 5;
    public int nbPotionsMax => _nbPotionsMax;
    [SerializeField] private int _nbPotions = 0;
    public int nbPotions { get => _nbPotions; set => _nbPotions = value; }

    // Potions de double saut #synthese_ALBERT 
    [Header("Paramètres Potions De double saut")] // #synthese_ALBERT
    // [SerializeField][Range(0, 5)] private int _nbPotionsDoubleSautMax = 5; // Nombre maximum de potions de double saut
    private int _nbPotionsDoubleSaut; // Nombre de potions de double saut restantes
    public int nbPotionsDoubleSaut { get => _nbPotionsDoubleSaut; set => _nbPotionsDoubleSaut = value; }
    
    private int _dommageShuriken = 10;
    public int dommageShuriken
    {
        get => _dommageShuriken;
        set => _dommageShuriken = value;
    }
    // Getters et Setters du niveau
    public int niveau
    {
        get => _niveau;
        set
        {
            _niveau = Mathf.Clamp(value, 1, int.MaxValue);
            _evenementMiseAJour.Invoke();
        }
    }

    // Getters et Setters de l'argent
    public int argent
    {
        get => _argent;
        set
        {
            _argent = Mathf.Clamp(value, 0, int.MaxValue);
            _evenementMiseAJour.Invoke();
        }
    }

    // Getters et Setters de la vie
    public int vie
    {
        get => _vie;
        set
        {
            _vie = Mathf.Clamp(value, 0, int.MaxValue);
            _evenementMiseAJour.Invoke();
        }
    }

    // Evenement qui permet de mettre à jour les variables du perso
    private UnityEvent _evenementMiseAJour = new UnityEvent();
    public UnityEvent evenementMiseAJour => _evenementMiseAJour;

    // Liste des objets du perso
    List<SOObjet> _lesObjets = new List<SOObjet>();

    // Facteur de prix
    float _facteurPrixIni = 1f;

    // Getters et Setters du facteur de prix
    float _facteurPrix = 1f;
    public float facteurPrix
    {
        get => _facteurPrix;
        set
        {
            _facteurPrix = value;
            _evenementMiseAJour.Invoke();
        }
    }

    // Facteur de prix si le perso a droit au rabais
    float _facteurPrixSiRabais = 0.9f;

    /// <summary>
    /// Fonction qui initialise les variables du perso
    /// </summary>
    public void Initialiser()
    {
        _facteurPrix = _facteurPrixIni;
        _argent = _argentIni;
        _niveau = _niveauIni;
        _vie = _vieIni; // #Synthèse_ALBERT
        _nbShuriken = _nbShurikenInit; // #Synthèse_ALBERT
        _nbPotions = 0; // #Synthèse_ALBERT
        _lesObjets.Clear();
        _nbPotionsDoubleSaut = 0; // #Synthèse_ALBERT
    }

    /// <summary>
    /// Auteur : Albert Jannard 
    /// Modifier pour #Synthèse_ALBERT
    /// Fonction qui permet d'acheter un objet
    /// </summary>
    /// <param name="donneesObjet"></param>
    public void Acheter(SOObjet donneesObjet)
    {

        Debug.Log("Achat de " + donneesObjet.nom);
        argent -= donneesObjet.prix;
        if (donneesObjet.donneDroitRabais)
        {
            facteurPrix = _facteurPrixSiRabais;
        }
        _lesObjets.Add(donneesObjet);

        if (donneesObjet.nom == "Shuriken")
        {
            IncrementerObjet(ref _nbShuriken, donneesObjet.nbMaxAchat);
        }
        else if (donneesObjet.nom == "Potion de santé")
        {
            IncrementerObjet(ref _nbPotions, donneesObjet.nbMaxAchat);
        }
        else if (donneesObjet.nom == "Potion double saut")
        {
            IncrementerObjet(ref _nbPotionsDoubleSaut, donneesObjet.nbMaxAchat);
        }

        OnValidate();
        AfficherInventaire();
    }

    /// <summary>
    /// Auteur : Albert Jannard
    /// Cette fonction est utilisée pour augmenter le compteur d'objets jusqu'à un maximum spécifié.
    /// </summary>
    /// <param name="objetCount">Le compteur actuel d'objets.</param>
    /// <param name="maxAchat">Le nombre maximum d'objets pouvant être achetés.</param>
    private void IncrementerObjet(ref int objetCount, int maxAchat)
    {
        // Vérifie si le compteur d'objets est inférieur au maximum d'achats possible.
        if (objetCount <= maxAchat)
        {
            // Incrémente le compteur d'objets.
            objetCount++;
            _evenementMiseAJour.Invoke();
        }
    }
    /// <summary>
    /// fonction qui permet d'afficher l'inventaire du perso
    /// </summary>
    private void AfficherInventaire()
    {
        string inventaire = "";
        foreach (SOObjet objet in _lesObjets)
        {
            if (inventaire != "") inventaire += ", ";
            inventaire += objet.nom;
        }
        Debug.Log("Inventaire du perso: " + inventaire);
    }

    /// <summary>
    /// fonction qui permet de mettre à jour les variables du perso
    /// </summary>
    private void OnValidate()
    {
        _evenementMiseAJour.Invoke();
    }
    private void OnEnable()
    {
        _objetsPossedes = new Dictionary<SOObjet, int>();
    }
}
