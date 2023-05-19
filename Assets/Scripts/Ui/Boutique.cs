using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>à
/// Code provient des capsules
/// Modifier par Albert Jannard
/// </summary>

public class Boutique : MonoBehaviour
{


    [Header("Données Personnage")]
    [SerializeField] private SOPerso _donneesPerso;
    public SOPerso donneesPerso => _donneesPerso;

    [Header("Champs Texte")]
    [SerializeField] private TextMeshProUGUI _champNiveau;
    [SerializeField] private TextMeshProUGUI _champArgent;
    [SerializeField] private TextMeshProUGUI _champShuriken;
    [SerializeField] private TextMeshProUGUI _champPotion;
    [SerializeField] private TextMeshProUGUI _champPotionDoubleSaut;

    [Header("Panneau Niveau Bonus")]
    [SerializeField] private GameObject _panneauNiveauBonus;

    // Singleton
    static Boutique _instance;
    static public Boutique instance => _instance;

    void Awake()
    {

        _panneauNiveauBonus.SetActive(false);
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        MettreAJourInfos();

        // #synthese_ZACHARY
        // On affiche un panel qui avise le joueur qu'il a un niveau bonus lorsque celui-ci quitte la boutique
        // Cela permet de l'afficher à chaque fois juste avant de commencer le niveau bonus
        if (_donneesPerso.niveau % 3 == 2)
        {
            _panneauNiveauBonus.SetActive(true);
            StartCoroutine(CligonterPanel(_panneauNiveauBonus, 30f)); // On fait cligonter le panel pendant 30 secondes
        }


        _donneesPerso.evenementMiseAJour.AddListener(MettreAJourInfos);
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="duree"></param>
    /// <returns></returns>
    IEnumerator CligonterPanel(GameObject panel, float duree)
    {
        float tempsEcouler = 0f;
        // On fait cligonter le panel pendant 30 secondes
        while (tempsEcouler < duree)
        {
            panel.SetActive(!panel.activeSelf);
            yield return new WaitForSeconds(0.99f); // On attend 0.99 secondes pour que le panel ne soit pas toujours actif
            tempsEcouler += 0.2f;
        }
        panel.SetActive(false);
    }


    private void MettreAJourInfos()
    {
        _champArgent.text = _donneesPerso.argent + " $";
        _champNiveau.text = "Niveau: " + _donneesPerso.niveau;
        _champShuriken.text = "" + _donneesPerso.nbShuriken;
        _champPotion.text = "" + _donneesPerso.nbPotions;
        _champPotionDoubleSaut.text = "" + _donneesPerso.nbPotionsDoubleSaut;
    }

    public bool PeutAcheter(SOObjet objet)
    {
        if (!donneesPerso.ObjetsPossedes.ContainsKey(objet))
        {
            donneesPerso.ObjetsPossedes.Add(objet, 0);
        }

        int objetsPossedes = donneesPerso.ObjetsPossedes[objet];
        return objetsPossedes < objet.nbMaxAchat;
    }



    void OnApplicationQuit()
    {
        _donneesPerso.Initialiser();
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        _donneesPerso.evenementMiseAJour.RemoveAllListeners();

    }
}