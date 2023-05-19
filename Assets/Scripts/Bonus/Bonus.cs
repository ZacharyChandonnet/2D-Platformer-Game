using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auteur : Zachary Chandonnet
/// Classe qui soccupe de l'objet bonus potion
/// Elle permet de donner au personnage un saut additionel de 10 pendant 4 secondes
/// </summary>

public class Bonus : MonoBehaviour
{   
    [Header("Audio")]
    [SerializeField] AudioClip sonPotion; // Son de la potion #Synthèse_ALBERT
    
    /// <summary>
    /// #synthese_ZACHARY
    /// Constantes qui permettent de modifier les attributs du personnage
    /// On évite le hardcoding en utilisant des constantes
    /// </summary>
    private const float bonusForceSaut = 1.5f;
    private const float bonusVitessePersonnage = 0.25f;
    private const float bonusDuration = 5f;

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
                perso.forceSaut += bonusForceSaut;
                perso.vitessePersonnage += bonusVitessePersonnage;
                gameObject.SetActive(false);
                Debug.Log($"+{bonusForceSaut} saut & +{bonusVitessePersonnage} vitesse");
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
            perso.forceSaut -= bonusForceSaut;
            perso.vitessePersonnage -= bonusVitessePersonnage;
            Debug.Log("bonus désactivé (saut & vitesse)");
        }
    }
}
