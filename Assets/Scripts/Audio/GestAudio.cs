using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #TP4 ALBERT
/// Cette classe gère la lecture des effets sonores et des pistes musicales dans une scène Unity. 
/// Elle crée des sources audio pour chaque piste musicale.
/// Permet de jouer des effets sonores avec une variation de pitch aléatoire.
/// Permet de modifier l'état de lecture des pistes musicales avec une transition de volume en douceur.
/// Auteurs du code: Albert Jannard 
/// Auteur des commentaires: Albert Jannard
/// </summary>
public class GestAudio : MonoBehaviour
{
    [Header("Effets sonores")]
    private AudioClip _clipSaut; // #synthese_ALBERT
    public AudioClip SonDeSaut { get; set; }

    private AudioClip _clipAtterrissage; // #synthese_ALBERT
    public AudioClip SonDAtterrissage { get; set; }

    [Header("Paramètres des effets sonores")]
    [SerializeField] float _pitchMinEffetsSonores = 0.9f;
    [SerializeField] float _pitchMaxEffetsSonores = 1.1f;

    [Header("Paramètres de transition de la musique")]
    [SerializeField] float _volumeMinMusiqueRef = 1f;
    [SerializeField] float _transitionDuration = 2f;
    public float volumeMinMusiqueRef => _volumeMinMusiqueRef;

    [Header("Pistes audio")]
    [SerializeField] SOPiste[] _tPistes;
    public SOPiste[] tPistes => _tPistes;

    [Header("Sources audio")]
    AudioSource _sourceEffetsSonores;
    static GestAudio _instance;
    static public GestAudio instance => _instance;

    [Header("Paramètres des sources audio")]
    [SerializeField] int _maxSourcesAudioSimultanees = 5;
    private AudioSource[] _sourcesEffetsSonores;

    [Header("Cooldown des effets sonores")]
    private float _dernierSautTemp;
    private float _dernierAtterrissageTemp;
    private float _cooldownSaut = 0.5f;
    private float _cooldownAtterrissage = 1.3f;

    void Awake()
    {
        // Si l'instance du gestionnaire audio est nulle, on la définit à cette instance.
        if (_instance == null) _instance = this;
        else
        {
            Debug.Log("Un gestionnaire audio existe déjà, donc celui sur la scène sera détruit");
            Destroy(gameObject);
            return;
        }

        _sourcesEffetsSonores = new AudioSource[_maxSourcesAudioSimultanees];
        for (int i = 0; i < _maxSourcesAudioSimultanees; i++)
        {
            _sourcesEffetsSonores[i] = gameObject.AddComponent<AudioSource>();
        }

        DontDestroyOnLoad(gameObject);
        _sourceEffetsSonores = gameObject.AddComponent<AudioSource>();
        CreerLesSourcesMusicales();
    }

    /// <summary>
    /// #TP4 ALBERT
    /// Fonction qui crée une source audio pour chaque piste audio dans la liste "_tPistes" et initialise chaque source audio avec la piste audio correspondante en utilisant la méthode "Initialiser" de la piste audio.
    /// </summary>
    void CreerLesSourcesMusicales()
    {
        foreach (SOPiste piste in _tPistes)
        {
            // On ajoute une source audio à l'objet et on l'initialise avec la piste audio correspondante.
            AudioSource source = gameObject.AddComponent<AudioSource>();
            piste.Initialiser(source);
        }
    }

    /// <summary>
    /// #TP4 ALBERT Modifier:#Synthese_ALBERT
    /// Joue l'effet sonore spécifié en utilisant une source audio libre. Le son de saut et le son d'atterrissage sont soumis à un temps de récupération pour éviter les lectures en boucle.
    /// </summary>
    /// <param name="clip">L'effet sonore à jouer</param>
    public void JouerEffetSonore(AudioClip clip)
    {
        if (clip == SonDeSaut && Time.time - _dernierSautTemp < _cooldownSaut)
        {
            return;
            // Si l'effet sonore à jouer est celui du saut et le temps écoulé depuis le dernier saut est inférieur au temps de récupération (_cooldownSaut),
            // on arrête l'exécution de la fonction pour attendre le cooldown avant de rejouer le son de saut.
        }
        else if (clip == SonDAtterrissage && Time.time - _dernierAtterrissageTemp < _cooldownAtterrissage)
        {
            return;
            // Si l'effet sonore à jouer est celui de l'atterrissage et le temps écoulé depuis le dernier atterrissage est inférieur au temps de récupération (_cooldownAtterrissage),
            // on arrête l'exécution de la fonction pour attendre le cooldown avant de rejouer le son d'atterrissage.
        }


        AudioSource sourceLibre = ObtenirSourceAudioLibre();
        // Obtient une source audio libre à partir de la méthode "ObtenirSourceAudioLibre"

        if (sourceLibre != null)
        {
            sourceLibre.pitch = Random.Range(_pitchMinEffetsSonores, _pitchMaxEffetsSonores);
            // Définit le pitch de la source audio avec une valeur aléatoire entre "_pitchMinEffetsSonores" et "_pitchMaxEffetsSonores"

            sourceLibre.PlayOneShot(clip);
            // Joue l'effet sonore spécifié en utilisant la méthode "PlayOneShot" de la source audio

            sourceLibre.volume = 1.3f;
            // Définit le volume de la source audio à 1.3 (valeur fixe)

            if (clip == SonDeSaut)
            {
                _dernierSautTemp = Time.time;
                // Met à jour le temps du dernier déclenchement de l'effet sonore de saut avec le temps actuel
            }
            else if (clip == SonDAtterrissage)
            {
                _dernierAtterrissageTemp = Time.time;
                // Met à jour le temps du dernier déclenchement de l'effet sonore d'atterrissage avec le temps actuel
            }
        }
    }


    /// <summary>
    /// #TP4 ALBERT
    /// Fonction qui permet de changer l'état de lecture d'une piste audio en utilisant une coroutine pour gérer la transition de volume en douceur, et elle recherche la piste audio correspondante en fonction de son type dans une liste de pistes prédéfinies.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="estActif"></param>
    public void ChangerEtatLecturePiste(TypePiste type, bool estActif)
    {
        foreach (SOPiste piste in _tPistes)
        {
            if (piste.type == type)
            {
                StartCoroutine(CoroutineChangerEtatLecturePiste(piste, estActif));
                return;
            }
        }
    }

    /// <summary>
    /// #TP4 ALBERT
    /// Coroutine qui gère la transition de volume d'une piste audio. 
    /// Si "estActif" est vrai, la coroutine augmente progressivement le volume de la piste audio jusqu'à une valeur maximale définie. 
    /// Si "estActif" est faux, elle diminue progressivement le volume de la piste audio jusqu'à une valeur minimale définie, puis marque la piste comme désactivée. 
    /// </summary>
    /// <param name="piste"></param>
    /// <param name="estActif"></param>
    /// <returns></returns>
    IEnumerator CoroutineChangerEtatLecturePiste(SOPiste piste, bool estActif)
    {
        if (estActif)
        {
            // Fade in
            piste.estActif = true;
            float volumeInitial = piste.source.volume;
            float volumeFinal = _volumeMinMusiqueRef;
            float tempsInitial = Time.time;
            float tempsFinal = tempsInitial + _transitionDuration;
            while (Time.time < tempsFinal)
            {
                float pourcentage = (Time.time - tempsInitial) / _transitionDuration;
                float nouveauVolume = Mathf.Lerp(volumeInitial, volumeFinal, pourcentage);
                piste.source.volume = nouveauVolume;
                yield return null;
            }
            piste.source.volume = volumeFinal;
        }
        else
        {
            // Fade out
            float volumeInitial = piste.source.volume;
            float volumeFinal = 0f;
            float tempsInitial = Time.time;
            float tempsFinal = tempsInitial + _transitionDuration;
            while (Time.time < tempsFinal)
            {
                float pourcentage = (Time.time - tempsInitial) / _transitionDuration;
                float nouveauVolume = Mathf.Lerp(volumeInitial, volumeFinal, pourcentage);
                piste.source.volume = nouveauVolume;
                yield return null;
            }
            piste.estActif = false;
            piste.source.volume = volumeFinal;
        }
    }

    /// <summary>
    /// #Synthèse_ALBERT
    /// Fonction qui permet de trouver une source audio libre dans la liste "_sourcesEffetsSonores" et de la retourner.
    /// </summary>
    private AudioSource ObtenirSourceAudioLibre()
    {
        foreach (AudioSource source in _sourcesEffetsSonores)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null;
    }
}
