using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "Navigation", menuName = "Navigation")]
public class SONavigation : ScriptableObject
{
    [SerializeField] private SOPerso _donneesPerso;
    bool estActif = false;
    public void Jouer()
    {
        _donneesPerso.Initialiser();
        AllerSceneSuivante();
    }

    public void SortirBoutique()
    {
        // #TP4 ZACHARY
        // On augmente de niveau lorsqu'on quitte la boutique.
        _donneesPerso.niveau++;
        // Debug.Log("niveau : " + _donneesPerso.niveau);
        AllerScenePrecedente();
        
    }

    public void AllerSceneSuivante()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenB, estActif);// TP#4 Albert Jannard

    }
    public void AllerScenePrecedente()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void AllerMenuPrincipal()
    {
        // #TP4 ZACHARY
        // On charge la scène du menu principal après la fin de la partie
        SceneManager.LoadScene("Menu");
        _donneesPerso.Initialiser();
    }
    
    public void AllerTutoriel()
    {
        // #Synthese Vincent
        // On charge la scène du tutoriel
        SceneManager.LoadScene("Tutoriel");
        _donneesPerso.Initialiser();
    }
    public void AllerControles()
    {
        // #Synthese Vincent
        // On charge la scène des controles
        SceneManager.LoadScene("Controles");
        _donneesPerso.Initialiser();
    }
    public void AllerGenerique()
    {
        // #Synthese Vincent
        // On charge la scène des crédits
        SceneManager.LoadScene("Credit");
        _donneesPerso.Initialiser();
    }
}
