using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Auteur du code: Albert Jannard 
/// Auteur des commentaire: Albert Jannard 
/// </summary>
public class Porte : MonoBehaviour
{
  [Header("Navigation")]
    [SerializeField] private SONavigation _nav;

    [Header("Audio")]
    [SerializeField] private AudioClip clip;

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
 
    void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.gameObject.GetComponent<Perso>();
        if (perso != null){
            if(perso.peuxSortir) _nav.AllerSceneSuivante();
            GestAudio.instance.JouerEffetSonore(clip);// TP#4 Albert Jannard
        }
    }
}
