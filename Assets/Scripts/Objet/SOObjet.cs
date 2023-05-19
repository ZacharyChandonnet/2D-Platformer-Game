using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Code provien des capsules et a été modifié par Albert
[CreateAssetMenu(fileName = "Objet", menuName = "Objet boutique")]
public class SOObjet : ScriptableObject
{
    [Header("LES DONNÉES")]
    [SerializeField] string _nom = "Objet";
    [SerializeField][Tooltip("Image de l'icône à afficher")] Sprite _sprite;
    [SerializeField][Range(0, 200)] int _prixDeBase = 30;
    [SerializeField][TextArea] string _description;
    [SerializeField][Tooltip("Cet objet donne-t-il droit au rabais?")] bool _donneDroitRabais = false;

    [SerializeField][Range(0, 100)] int _nbMaxAchat = 5; 

    public int nbMaxAchat { get => _nbMaxAchat; set => _nbMaxAchat = value; }

    public string nom { get => _nom; set => _nom = value; }
    public Sprite sprite { get => _sprite; set => _sprite = value; }
    [SerializeField] private bool _disponible = false;
    public bool disponible {get => _disponible; set => _disponible = value;}
    public int prix
    { 
        get
        {
            float facteur = 1f;
            if(Boutique.instance != null) facteur = Boutique.instance.donneesPerso.facteurPrix;
            int prix = Mathf.RoundToInt(_prixDeBase * facteur);
            return prix;
            
        } 
    }
    public string description { get => _description; set => _description = value; }
    public bool donneDroitRabais { get => _donneDroitRabais; set => _donneDroitRabais = value; }
}