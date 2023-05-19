using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auteur : Albert Jannard #Synthèse_ALBERT (Modifier pour le s)
/// Classe qui soccupe de l'objet bonus potion
/// Elle permet de donner au personnage une force de frappe de +1 pendant 5 secondes
/// </summary>

public class Bonus2 : MonoBehaviour
{

    [Header("Audio")]
    [SerializeField] AudioClip sonPotion; // Son de la potion #Synthèse_ALBERT
    /// <summary>
    /// #synthese_ZACHARY
    /// Constantes qui permettent de modifier les attributs du personnage
    /// On évite le hardcoding en utilisant des constantes
    /// </summary>
    private const float bonusDuration = 5f;

    [SerializeField] SOPerso _donneesPerso; // On retrouve le shuriken #Synthèse_ALBERT

    private void Awake()
    {
        DesactiverBonus();
    }
    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui permet de détecter la collision avec le personnage
    /// Si le personnage est détecté, on active le bonus et on désactive l'objet
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.gameObject.GetComponent<Perso>();
        
        if (perso != null)
        {
            perso.potionActive = true;
            // On vérifie si la potion est active
            if (perso.potionActive)
            {
                //Changer la valeur de siBonusActif à true dans le script Shuriken
                _donneesPerso.dommageShuriken *= 2;

                gameObject.SetActive(false);
                // Debug.Log($"+{bonusForceFrappe} frappe");
                Invoke("DesactiverBonus", bonusDuration); // On désactive le bonus après 5 secondes
                GestAudio.instance.JouerEffetSonore(sonPotion); // On joue le son de la potion #Synthèse_ALBERT
            }
        }
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui permet de désactiver les bonus du personnage
    /// Avant, le bonus était actif pendant toute la partie
    /// </summary>
    void DesactiverBonus()
    {
        // On retrouve le personnage
        Perso perso = FindObjectOfType<Perso>();
        
        // On désactive les bonus
        if (perso != null)
        {
            _donneesPerso.dommageShuriken = 10;
            Debug.Log("bonus désactivé (frappe)");
        }
    }
}
