using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auteur : Albert Jannard
/// Classe qui soccupe de l'objet joyaux
/// Elle permet de gagner x nb d'argent au perso
/// </summary>
public class Joyaux : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioClip joyauxSound;
    [Header("Paramètres")]
    [SerializeField]
    [Range(0, 50)]
    private int _valeurDeBase = 20;
    
    public int valeurDeBase { get => _valeurDeBase; set => _valeurDeBase = Mathf.Clamp(value, 0, int.MaxValue); }

    // Autres membres et méthodes de votre script
    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // #TP3 Albert 
        Perso perso = other.gameObject.GetComponent<Perso>();
        if (perso != null)
        {
            perso.AugmenterArgent(valeurDeBase);
            gameObject.SetActive(false);
            GestAudio.instance.JouerEffetSonore(joyauxSound);
        }
    }
}
