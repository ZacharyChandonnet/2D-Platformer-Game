using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// Auteur : #TP4 ZACHARY 
/// Classe qui sert à afficher le score et les points du joueur à la fin de la partie.
/// Les points sont définis selon les joyaux ramassés, les niveaux réussi ainsi que le temps restant.
/// De plus, on affiche le top 3 des meilleurs joueurs sur la machine locale
/// </summary>

public class AffichageScore : MonoBehaviour
{
    [Header("Affichage")]
    [SerializeField] public SOSauvegarde _affichage;

    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI _texteArgent;
    [SerializeField] private TextMeshProUGUI _textePtsArgent;
    [SerializeField] private TextMeshProUGUI _texteNiveau;
    [SerializeField] private TextMeshProUGUI _textePtsNiveau;
    [SerializeField] private TextMeshProUGUI _texteVie;
    [SerializeField] private TextMeshProUGUI _textePtsVie;
    [SerializeField] private TextMeshProUGUI _texteScoreFinal;
    [SerializeField] private TextMeshProUGUI[] _tTexteNomJoueur;
    [SerializeField] private TextMeshProUGUI _texteTopScore;

    [Header("Entrée")]
    [SerializeField] private TMP_InputField _inputNomJoueur;

    [Header("Constantes")]
    // #TP4 ZACHARY
    // On utilise const car le multiplicateur ne peut pas changer
    // On évite le hard code
    private const int PTSPARNIVEAU = 1200;
    private const int PTSPARVIE = 100;
    private const int PTSPARARGENT = 50;

    [SerializeField] private SOPerso _donneesPerso;
    public SOPerso DonneesPerso => _donneesPerso;

    void Start()
    {
        _affichage.TraiterFichier();
        StartCoroutine(AfficherInfosProgressivement());
    }

    void Update()
    {
        // #TP4 ZACHARY
        // Permet d'activer ou de désactiver le input field si le joueur se trouve dans le top3
        if (_affichage.scoreFinPartieDansTop3) _inputNomJoueur.interactable = true;
        else { _inputNomJoueur.interactable = false; }
    }

    /// <summary>
    /// #TP4 ZACHARY
    /// Méthode qui permet de changer le nom du joueur 
    /// Cela est fonctionnel seulement si celui-ci est dans le top 3 car  
    /// le panneau est fonctionnel seulement dans le top3
    /// </summary>
    /// <param name="nouveauNom"></param>
    public void ChangerNomJoueur(string nouveauNom)
    {
        // Vérifier si le score du joueur est dans le top 3
        if (_affichage.scoreFinPartieDansTop3)
        {
            // Mettre à jour le texte du nom du joueur avec le nouveau nom
            _tTexteNomJoueur[_affichage.JoueurIndex].text = nouveauNom;
        }
    }

    /// <summary>
    /// #TP4 ZACHARY 
    /// Méthode qui permet d'afficher les résultats un par un à chaque seconde
    /// </summary>
    /// <returns></returns>
    private IEnumerator AfficherInfosProgressivement()
    {
        // Affichage du niveau
        yield return new WaitForSeconds(1); // Attendre 1 seconde
        AfficherNiveau();

        // Affichage de la vie
        yield return new WaitForSeconds(1); // Attendre 1 seconde
        AfficherVie();

        // Affichage de l'argent
        yield return new WaitForSeconds(1); // Attendre 1 seconde
        AfficherArgent();

        // Affichage du score final
        yield return new WaitForSeconds(1); // Attendre 1 seconde
        AfficherScoreFinal();
    }

    /// <summary>
    /// #TP4 ZACHARY 
    /// Méthodes qui s'occupent des affichages
    /// </summary>
    /// <returns></returns>

    // #TP4 ZACHARY
    // Affichage des niveaux
    private void AfficherNiveau()
    {
        _texteNiveau.text = $"{_donneesPerso.niveau}";
        Debug.Log("Niveau complété : " + _donneesPerso.niveau);
        _textePtsNiveau.text = $"{PTSPARNIVEAU * _donneesPerso.niveau} pts";
    }

    // #TP4 ZACHARY
    // Affichage des vies
    private void AfficherVie()
    {
        _texteVie.text = $"{_donneesPerso.vie}";
        _textePtsVie.text = $"{PTSPARVIE * _donneesPerso.vie} pts";
        Debug.Log("Nb vies restantes : " + _donneesPerso.vie);
    }

    // #TP4 ZACHARY
    // Affichage de l'argent
    private void AfficherArgent()
    {
        _texteArgent.text = $"{_donneesPerso.argent}";
        _textePtsArgent.text = $"{PTSPARARGENT * _donneesPerso.argent} pts";
        Debug.Log("Joyaux ramassés : " + _donneesPerso.argent);
    }

    // #TP4 ZACHARY
    // Affichage du score final
    private void AfficherScoreFinal()
    {
        int scoreFinal = _donneesPerso.niveau * PTSPARNIVEAU + _donneesPerso.vie * PTSPARVIE + _donneesPerso.argent * PTSPARARGENT;
        _texteScoreFinal.text = $"{scoreFinal}";
        Debug.Log("Niveau Score Final : " + scoreFinal);

        _affichage.AfficherTopScores(_texteTopScore, scoreFinal); // Passer le score final en tant que deuxième argument
    }


}

