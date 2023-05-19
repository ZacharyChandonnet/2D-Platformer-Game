using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auteur : Albert Jannard
/// Classe qui soccupe de l'objet Clef
/// </summary>
public class Clef : MonoBehaviour
{    
    [SerializeField] private AudioClip clip;// TP#4 Albert Jannard
    string texteEvent ="";
    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        
        // #TP3 Albert 
        Perso perso = other.gameObject.GetComponent<Perso>();
        if (perso != null){
            bool estActif = true;
            perso.peuxSortir = true;
            gameObject.SetActive(false);
            GestAudio.instance.JouerEffetSonore(clip);// TP#4 Albert Jannard
            texteEvent +="<color=green>";
            GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenB, estActif);// TP#4 Albert Jannard
            texteEvent +="La musique de l'événement B est activée</color>";
            Debug.Log(texteEvent);
        }
    }
}
