using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auteur: Albert Jannard 
/// Commentaire: Albert Jannard 
/// Ce script est utilisé pour activer les bonnus 
/// Active la particule de l'activateur 
/// #Synthèse_ALBERT
/// </summary>
public class Activateur : MonoBehaviour
{
    [SerializeField] private SOPerso _donnees; // Données du personnage
    private ParticleSystem particles; // Système de particules de l'activateur 
    [SerializeField] private AudioClip _sonActivateur; // Son de saut #TP4_ALBERT
    private void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>(); // Obtenez le système de particules enfant du personnage
        particles.Stop(); // Arrêtez le système de particules au début de la partie
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.gameObject.GetComponent<Perso>(); 
        if (perso != null)
        {
            _donnees.bonusActivated.Invoke(); // Activez les bonus
            particles.Play(); // Lancez le système de particules lors d'une collision #Synthèse_ALBERT
            GestAudio.instance.JouerEffetSonore(_sonActivateur); // TP#4 Albert Jannard
        }
    }
}
