using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PanneauObjet : MonoBehaviour
{
    [Header("LES DONNÉES")]
    [SerializeField] SOObjet _donnees;
    [SerializeField] SOPerso _donneesPerso;
    public SOObjet donnees => _donnees;
    public SOPerso donneesPerso => _donneesPerso;

    [Header("LES CONTENEURS")]
    [SerializeField] TextMeshProUGUI _champNom;
    [SerializeField] TextMeshProUGUI _champPrix;
    [SerializeField] TextMeshProUGUI _champDescription;
    [SerializeField] Image _image;
    [SerializeField] CanvasGroup _canvasGroup;

    void Start()
    {
        MettreAJourInfos();
        Boutique.instance.donneesPerso.evenementMiseAJour.AddListener(MettreAJourInfos);
    }

    private void MettreAJourInfos()
    {
        _champNom.text = _donnees.nom;
        _champPrix.text = _donnees.prix + " $";
        _champDescription.text = _donnees.description;
        _image.sprite = _donnees.sprite;
        GererDispo();
    }

    private void GererDispo()
    {
        bool aAsserArgent = Boutique.instance.donneesPerso.argent >= _donnees.prix;
        bool peutAcheter = Boutique.instance.PeutAcheter(_donnees); // Ajout de la variable 'peutAcheter' en utilisant la fonction 'PeutAcheter' de la classe 'Boutique'

        if (aAsserArgent && peutAcheter)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.alpha = 1;
        }
        else
        {
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0.5f;
        }

        // Vérifie si le nombre d'objets possédés a atteint le maximum d'achats
        if (donneesPerso.ObjetsPossedes[_donnees] >= _donnees.nbMaxAchat)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0.5f;
            _donnees.disponible = false; // Définit la disponibilité de l'objet sur 'false'
        }
    }

    public void Acheter()
    {
        Boutique.instance.donneesPerso.Acheter(_donnees);
    }
}
