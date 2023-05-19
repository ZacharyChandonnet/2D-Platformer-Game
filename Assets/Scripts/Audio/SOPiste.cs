using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #TP4 ALBERT
/// ScriptableObject qui contient les informations d'une piste musicale, telles que le type de piste, le clip à jouer,
/// l'état initial et actuel, la source audio qui jouera le clip et les méthodes pour initialiser la source audio et
/// ajuster le volume en fonction de l'état actif.
/// Auteurs du code: Albert Jannard 
/// Auteur des commentaires: Albert Jannard
/// </summary>

// On crée un menu pour créer un ScriptableObject de type SOPiste.
[CreateAssetMenu(menuName = "Piste musicale", fileName = "DonneesPiste")]
public class SOPiste : ScriptableObject
{
// Le type de la piste
[SerializeField] TypePiste _type;
// Le clip à jouer
[SerializeField] AudioClip _clip;

// Permet de choisir l'état initial
[SerializeField] bool _estActifParDefaut;

// C'est l'état actuel
[SerializeField] bool _estActif;

// La source audio qui jouera le clip
AudioSource _source;

// Getter pour la source audio
public AudioSource source => _source;

// Getter pour le type de la piste
public TypePiste type => _type;

// Getter pour le clip à jouer
public AudioClip clip => _clip;


// Propriété qui permet d'accéder et de modifier l'état actif de la piste.
public bool estActif 
{ 
    get => _estActif; 
    set 
    {
        _estActif = value; 
        AjusterVolume();
    }
}

/// <summary>
/// #TP4 ALBERT
/// Initialise la source audio avec le clip, l'état initial et joue le clip.
/// </summary>
/// <param name="source">La source audio à initialiser.</param>
public void Initialiser(AudioSource source)
{
    _source = source;
    _source.clip = _clip;
    _source.loop = true;
    _source.playOnAwake = false;
    _source.Play();
    _estActif = _estActifParDefaut;
    AjusterVolume(); 
}

/// <summary>
/// #TP4 ALBERT
/// Ajuste le volume de la source audio en fonction de l'état actif.
/// </summary>
public void AjusterVolume()
{
    if(_estActif) 
        _source.volume = GestAudio.instance.volumeMinMusiqueRef;
    else 
        _source.volume = 0;
}
}