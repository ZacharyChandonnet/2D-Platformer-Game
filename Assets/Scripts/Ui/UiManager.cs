using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI _texteArgent;
    [SerializeField] private TextMeshProUGUI _texteNiveau;
    [SerializeField] private TextMeshProUGUI _texteVie;
    [SerializeField] private TextMeshProUGUI _texteTemps;
    [SerializeField] private TextMeshProUGUI _texteShuriken;
    [SerializeField] private TextMeshProUGUI _textePotion;
    [SerializeField] private TextMeshProUGUI _textePotionSaut;

    [Header("Données Personnage")]
    [SerializeField] private SOPerso _donneesPerso;
    public SOPerso donneesPerso => _donneesPerso;


    private void Start()
    {
        StartCoroutine(CompteAReboursCoroutine());
    }

    void Update()
    {
        MiseAJourArgent();
        MiseAJourNiveau();
        MiseAJourVie();
        MiseAJourShuriken();
        MiseAJourPotion();
        MiseAJourPotionDoubleSaut();

        if (donneesPerso.vie == 0)
        {
            SceneManager.LoadScene("Fin");
        }
    }

    /// <summary>
    /// Fonction qui met à jour le texte du temps
    /// </summary>
    /// <returns></returns>
    private IEnumerator CompteAReboursCoroutine()
    {
        // #synthese_ZACHARY
        // On vérifie si le niveau est un niveau bonus, si oui on met le temps à 30 secondes, sinon on met le temps à 60 secondes
        bool estNiveauBonus = _donneesPerso.niveau % 3 == 0;
        float tempsRestant = estNiveauBonus ? 30f : 240f;

        while (tempsRestant > 0)
        {
            tempsRestant -= Time.deltaTime;
            int secondes = Mathf.FloorToInt(tempsRestant % 240);
            _texteTemps.text = secondes.ToString() + " s";
            yield return null;
        }

        // #synthese_ZACHARY
        // On vérifie si le niveau est un niveau bonus, si oui on charge la boutique, sinon on charge la scène de fin
        if (estNiveauBonus) SceneManager.LoadScene("Boutique");
        else SceneManager.LoadScene("Fin");
    }

     void MiseAJourArgent()
    {
        _texteArgent.text = donneesPerso.argent.ToString();
    }

     void MiseAJourNiveau()
    {
        _texteNiveau.text = donneesPerso.niveau.ToString();
    }

     void MiseAJourVie()
    {
        _texteVie.text = donneesPerso.vie.ToString();
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui met à jour le texte du nombre de shurikens
    /// </summary>
     void MiseAJourShuriken()
    {
        _texteShuriken.text = donneesPerso.nbShuriken.ToString();
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui met à jour le texte du nombre de potions
    /// </summary>
     void MiseAJourPotion()
    {
        _textePotion.text = donneesPerso.nbPotions.ToString();
    }
    /// <summary>
    /// #synthese_ALBERT
    /// Fonction qui met à jour le texte du nombre de potions de double saut 
    /// </summary>
     void MiseAJourPotionDoubleSaut()
    {
        _textePotionSaut.text = donneesPerso.nbPotionsDoubleSaut.ToString();
    }
}
